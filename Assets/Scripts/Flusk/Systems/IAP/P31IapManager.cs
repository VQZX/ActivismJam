#if USE_PRIME31_IAP && (UNITY_IOS || UNITY_ANDROID || UNITY_TVOS)
using System.Collections.Generic;
using UnityEngine;
using TFBGames.Build;
using Prime31;
using P31IAP = Prime31.IAP;

#if UNITY_IOS || UNITY_TVOS
using TransactionType = Prime31.StoreKitTransaction;
using System.IO;
#elif UNITY_ANDROID
using System;
using TransactionType = Prime31.GooglePurchase;
#endif

namespace TFBGames.Systems.IAP
{
	public abstract class P31IapManager<TIapItem, TIapCatalogue> :
		IapManager<TIapItem, TIapCatalogue, IAPProduct, TransactionType>
		where TIapItem : P31IapItem
		where TIapCatalogue : IapCatalogue<TIapItem, IAPProduct>
	{
		#region Fields

		private bool initialized;

#if UNITY_ANDROID
		private string[] deferredProducts;
#endif

		#endregion


		#region Properties

		private bool iapSupported;

		/// <summary>
		/// Gets whether IAP is supported on this device.
		/// </summary>
		public override bool IapSupported
		{
			get { return iapSupported; }
		}

		private bool iapReady;

		/// <summary>
		/// Gets whether the IAP system is ready. True once products are loaded
		/// </summary>
		public override bool IapReady
		{
			get { return iapReady; }
		}

		#endregion

		public override IapReceiptInfo GetLastTransactionReceiptInfo()
		{
			if (LastTransaction == null)
			{
				return IapReceiptInfo.Empty;
			}

#if UNITY_IOS
			// Load receipt
			string receiptPath = StoreKitBinding.getAppStoreReceiptLocation().Replace("file://", string.Empty);

			if (!File.Exists(receiptPath))
			{
				return IapReceiptInfo.Empty;
			}

			byte[] receipt = File.ReadAllBytes(receiptPath);

			return new IapReceiptInfo(System.Convert.ToBase64String(receipt), string.Empty, LastTransaction.transactionIdentifier);
#elif UNITY_ANDROID
			return new IapReceiptInfo(LastTransaction.originalJson, LastTransaction.signature, string.Empty);
#else
			return IapReceiptInfo.Empty;
#endif
		}

		protected override void LoadProducts(string[] productIds)
		{
			Initialize();

			if (!iapSupported)
			{
#if UNITY_ANDROID
				Debug.LogWarning("[IAP] IAP not supported or initialized yet! Deferred");
				deferredProducts = productIds;
				return;
#else
				Debug.LogError("[IAP] IAP not supported.");
				return;
#endif
			}

#if UNITY_IOS || UNITY_TVOS
			P31IAP.requestProductData(productIds, null, OnProductDataReceived);
#elif UNITY_ANDROID
			P31IAP.requestProductData(null, productIds, OnProductDataReceived);
#endif // UNITY_IOS/UNITY_ANDROID
		}

		protected virtual void Initialize()
		{
			if (!initialized)
			{
#if USE_IAP && !UNITY_EDITOR
				BuildConfig bc;
				if (BuildConfig.TryGetInstance(out bc))
				{
					P31IAP.init(bc.GooglePublicKey);
				}
				else
				{
					Debug.LogError("[IAP] Failed to init IAP - couldn't find build config");
				}

#if (UNITY_IOS || UNITY_TVOS)
				iapSupported = StoreKitBinding.canMakePayments();
#endif
#else
				iapSupported = true;
				iapReady = true;
#endif

				initialized = true;
			}
		}

		protected virtual void OnEnable()
		{
			// Register IOS events
#if USE_IAP && !UNITY_EDITOR
#if (UNITY_IOS || UNITY_TVOS)
			StoreKitManager.restoreTransactionsFinishedEvent += OnRestoreComplete;
			StoreKitManager.restoreTransactionsFailedEvent += OnRestoreFailed;
#elif UNITY_ANDROID
			// Register android init events
			GoogleIABManager.billingSupportedEvent += AndroidSupported;
			GoogleIABManager.billingNotSupportedEvent += AndroidNotSupported;
#endif
#endif
		}

		protected virtual void OnDisable()
		{
#if !UNITY_EDITOR && USE_IAP
#if (UNITY_IOS || UNITY_TVOS)
			StoreKitManager.restoreTransactionsFinishedEvent -= OnRestoreComplete;
			StoreKitManager.restoreTransactionsFailedEvent -= OnRestoreFailed;
#elif UNITY_ANDROID
			GoogleIABManager.billingSupportedEvent += AndroidSupported;
			GoogleIABManager.billingNotSupportedEvent += AndroidNotSupported;
#endif
#endif
		}

#if (UNITY_IOS || UNITY_TVOS) && !UNITY_EDITOR && USE_IAP
		protected virtual void OnRestoreComplete()
		{
			if (restoreCompleteCallback != null)
			{
				restoreCompleteCallback(true);
			}
			Restoring = false;
			SavePurchases();
		}
		
		protected virtual void OnRestoreFailed(string error)
		{
			Debug.LogErrorFormat("[IAP] Failed to restore transactions:\n{0}", error);
			
			if (restoreCompleteCallback != null)
			{
				restoreCompleteCallback(false);
			}
			Restoring = false;
		}
#endif

		/// <summary>
		/// Callback for every item that is restored when restoring purchases
		/// </summary>
		protected virtual void OnItemRestored(string itemId)
		{
			TIapItem item = GetItem(itemId);

			if (item != null)
			{
				RestoreItem(item);
			}
		}

		/// <summary>
		/// Callback fired when an item is purchased
		/// </summary>
		protected virtual void OnStoreReturnResult(bool success, TransactionType transaction, string error)
		{
			if (!success)
			{
				FailPurchase(BuyingItem, error);
			}
			else
			{
				PurchaseItem(BuyingItem, transaction);
			}
		}

		protected virtual void OnProductDataReceived(List<IAPProduct> products)
		{
			//early out if null
			if (products == null)
			{
				Debug.LogWarning("[IAP] Received empty product data");
				return;
			}
			
			iapReady = true;
			
			Debug.LogFormat("[IAP] Processing product data for {0} items", products.Count);

			foreach (IAPProduct product in products)
			{
				TIapItem correspondingItem = GetItem(product.productId);
				if (correspondingItem != null)
				{
					correspondingItem.Product = product;
				}
			}

#if UNITY_ANDROID
			// Just in case this is set somehow, we clear it so RestoreItem below
			// doesn't invoke it.
			Action<TIapItem, IapResult, string> prevBuyCallback = buyCallback;
			buyCallback = null;

			foreach (GooglePurchase purchase in P31IAP.androidPurchasedItems)
			{
				if (purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
				{
					TIapItem correspondingItem = GetItem(purchase.productId);

					if ((correspondingItem != null) && !correspondingItem.Purchased)
					{
						RestoreItem(correspondingItem);
					}
				}
			}

			buyCallback = prevBuyCallback;

			SavePurchases();
#endif
		}

		/// <summary>
		/// Called to perform logic of negotiating with the store to buy <see cref="BuyingItem"/>
		/// </summary>
		protected override void BuyItem()
		{
#if UNITY_EDITOR || !USE_IAP
			OnStoreReturnResult(true, null, string.Empty);
#else
			P31IAP.purchaseNonconsumableProduct(BuyingItem.IapId, OnStoreReturnResult);
#endif
		}

		/// <summary>
		/// Internal restore
		/// </summary>
		protected override void RestorePurchases()
		{
#if USE_IAP
#if UNITY_IOS || UNITY_TVOS
			StoreKitBinding.finishPendingTransactions();
#endif
			P31IAP.restoreCompletedTransactions(OnItemRestored);
#endif
		}


#if USE_IAP && !UNITY_EDITOR && UNITY_ANDROID
		private void AndroidNotSupported(string error)
		{
			Debug.LogFormat("[IAP] Billing not supported {0}", error);
			iapSupported = false;
		}

		private void AndroidSupported()
		{
			iapSupported = true;

			if (deferredProducts != null)
			{
				Debug.LogFormat("[IAP] Querying deferred products");
				LoadProducts(deferredProducts);
				deferredProducts = null;
			}
		}
#endif
	}
}
#endif
