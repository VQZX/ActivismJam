using System;
using Flusk.Patterns;
using UnityEngine;

#if USE_IAP
using System.Linq;
#endif

namespace Flusk.Systems.IAP
{
	public enum IapResult
	{
		Success,
		VerificationFailed,
		Failed
	}

	/// <summary>
	/// A base IAP manager
	/// </summary>
	/// <typeparam name="TIapItem">The game IAP item we'll be buying</typeparam>
	/// <typeparam name="TIapCatalogue">The game's IAP catalogue</typeparam>
	/// <typeparam name="TIapProduct">The store's product object</typeparam>
	/// <typeparam name="TTransaction">The store's transaction object</typeparam>
	public abstract class IapManager<TIapItem, TIapCatalogue, TIapProduct, TTransaction> :
		PersistentSingleton<IapManager<TIapItem, TIapCatalogue, TIapProduct, TTransaction>>
		where TIapItem: class, IIapItem<TIapProduct>
		where TIapCatalogue : IapCatalogue<TIapItem, TIapProduct>
		where TTransaction: class
	{
		/// <summary>
		/// Our IAP validator
		/// </summary>
		public static IIapValidator<TIapItem, TIapProduct, TTransaction> Validator { get; set; }

		/// <summary>
		/// How long before validation times out in seconds
		/// </summary>
		[SerializeField]
		protected float validationTimeout = 5;

		protected Action<TIapItem, IapResult, string> buyCallback;
		protected Action<bool> restoreCompleteCallback;

		private float validationStartTime;

		/// <summary>
		/// Gets our IAP catalogue
		/// </summary>
		public abstract TIapCatalogue Catalogue { get; protected set; }

		/// <summary>
		/// Gets whether the IAP system is ready
		/// </summary>
		public abstract bool IapReady { get; }

		/// <summary>
		/// Gets whether IAP is supported on this device
		/// </summary>
		public abstract bool IapSupported { get; }

		/// <summary>
		/// Gets the item that is currently being purchased
		/// </summary>
		public TIapItem BuyingItem { get; protected set; }

		/// <summary>
		/// Gets whether we're currently in the process of restoring purchases
		/// </summary>
		public bool Restoring { get; protected set; }

		/// <summary>
		/// Gets whether or not we're waiting on validation
		/// </summary>
		public bool Validating { get; protected set; }

		/// <summary>
		/// Gets the last transaction processed. Null for restorations
		/// </summary>
		public TTransaction LastTransaction { get; protected set; }


		/// <summary>
		/// Check for validation timeout
		/// </summary>
		protected virtual void Update()
		{
			if (Validating &&
				(Time.unscaledTime > validationStartTime + validationTimeout))
			{
				Debug.Log("[IAP] Timing out validation");

				// Timeout validation
				ValidationCallback(false, "Validation timed out.");
			}
		}


		/// <summary>
		/// Abstract method to retrieve last transaction info
		/// </summary>
		/// <returns></returns>
		public abstract IapReceiptInfo GetLastTransactionReceiptInfo();

		/// <summary>
		/// Gets an IAP item by its id
		/// </summary>
		public TIapItem GetItem(string id)
		{
			for (int i = 0; i < Catalogue.Count; ++i)
			{
				TIapItem item = Catalogue[i];
				if (item.IapId == id)
				{
					return item;
				}
			}
			return null;
		}

		/// <summary>
		/// Purchase an item by name
		/// </summary>
		public virtual void BuyItem(string name, Action<TIapItem, IapResult, string> callback)
		{
			TIapItem item = GetItem(name);

			if (item != null)
			{
				BuyItem(item, callback);
			}
			else
			{
				Debug.LogErrorFormat("[IAP] Couldn't find item with name {0}", name);
			}
		}

		/// <summary>
		/// Purchase an item
		/// </summary>
		public virtual void BuyItem(TIapItem item, Action<TIapItem, IapResult, string> callback)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			if (BuyingItem != null)
			{
				Debug.LogError("[IAP] We tried to buy an item while another purchase was being processed.");
				callback(item, IapResult.Failed, "Other purchase in progress.");
				return;
			}

			if (!IapSupported)
			{
				Debug.LogError("[IAP] IAP not supported");
				callback(item, IapResult.Failed, "IAP not supported.");
				return;
			}

			if (!IapReady)
			{
				Debug.LogError("[IAP] IAP not ready yet. No products retrieved");
				callback(item, IapResult.Failed, "No products available.");
				return;
			}

			buyCallback = callback;
			BuyingItem = item;

			BuyItem();
		}

		/// <summary>
		/// Called to perform logic of negotiating with the store to buy <see cref="BuyingItem"/>
		/// </summary>
		protected abstract void BuyItem();

		/// <summary>
		/// Restore purchases
		/// </summary>
		/// <remarks>
		/// Callback is fired for all items restored
		/// </remarks>
		public virtual void RestorePurchases(Action<TIapItem, IapResult, string> callback, Action<bool> restoreComplete)
		{
			if (BuyingItem != null)
			{
				Debug.LogError("[IAP] We tried to buy an item while another purchase was being processed.");
				restoreComplete(false);
				return;
			}

			if (!IapSupported)
			{
				Debug.LogError("[IAP] IAP not supported");
				restoreComplete(false);
				return;
			}

			if (!IapReady)
			{
				Debug.LogError("[IAP] IAP not ready yet. No products retrieved");
				restoreComplete(false);
				return;
			}

			buyCallback = callback;
			restoreCompleteCallback = restoreComplete;
			BuyingItem = null;

#if USE_IAP
			Restoring = true;
			RestorePurchases();
#else
			// Just award literally all items
			for (var i = 0; i < Catalogue.Count; i++)
			{
				TIapItem item = Catalogue[i];
				RestoreItem(item);
			}

			restoreComplete(true);
#endif
		}

		/// <summary>
		/// Internal restore
		/// </summary>
		protected abstract void RestorePurchases();

		/// <summary>
		/// Internal method to actually complete the process of restoring an item.
		/// </summary>
		/// <remarks>Works the same as <see cref="PurchaseItem"/> but does not also attempt to optionally validate the purchase</remarks>
		protected void RestoreItem(TIapItem item)
		{
			// Register purchased item
			OnPurchased(item);

			LastTransaction = null;

			if (buyCallback != null)
			{
				buyCallback(item, IapResult.Success, string.Empty);
			}
		}

		/// <summary>
		/// Internal method to actually complete the process of buying an item
		/// </summary>
		protected void PurchaseItem(TIapItem item, TTransaction transaction)
		{
			// Register purchased item
			OnPurchased(item);

			LastTransaction = transaction;

			// If we should validate this purchase
			if (Validator != null)
			{
				DoValidation(item, transaction);
			}
			else
			{
				if (buyCallback != null)
				{
					buyCallback(item, IapResult.Success, string.Empty);
				}
				BuyingItem = null;
				SavePurchases();
			}
		}

		/// <summary>
		/// Call when a purchase fails
		/// </summary>
		/// <param name="item">The item whose purchase has failed</param>
		/// <param name="error">The error received</param>
		protected void FailPurchase(TIapItem item, string error)
		{
			Debug.Assert(item == BuyingItem);

			if (buyCallback != null)
			{
				buyCallback(item, IapResult.Failed, error);
			}

			BuyingItem = null;
		}

		/// <summary>
		/// Commit new purchase to save data
		/// </summary>
		protected abstract void SavePurchases();

		/// <summary>
		/// Called when an item has successfully been purchased, to award it in the game
		/// </summary>
		/// <param name="item">The item that was purchased</param>
		protected abstract void OnPurchased(TIapItem item);

		/// <summary>
		/// Called to retrieve product list from the store
		/// </summary>
		protected abstract void LoadProducts(string[] productIds);

		/// <summary>
		/// Perform validation
		/// </summary>
		private void DoValidation(TIapItem item, TTransaction transaction)
		{
			if (Validator == null)
			{
				throw new InvalidOperationException("Trying to validate a purchase with no validator");
			}

			Validating = true;
			validationStartTime = Time.unscaledTime;
			Validator.TryValidate(item, transaction, ValidationCallback);
		}

		/// <summary>
		/// Callback returned from validator
		/// </summary>
		private void ValidationCallback(bool success, string message)
		{
			if (!Validating)
			{
				Debug.LogWarning("Validator returned callback but we weren't waiting for validation");
				return; // Our validation probably timed out
			}

			if (buyCallback != null)
			{
				buyCallback(BuyingItem, success ? IapResult.Success : IapResult.VerificationFailed, message);
			}
			BuyingItem = null;
			Validating = false;
			SavePurchases();
		}
	}
}
