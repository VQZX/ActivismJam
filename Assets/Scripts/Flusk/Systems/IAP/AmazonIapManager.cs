#if USE_AMAZON_IAP && UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using com.amazon.device.iap.cpt;
using UnityEngine;

namespace TFBGames.Systems.IAP
{
	public abstract class AmazonIapManager<TIapItem, TIapCatalogue> :
		IapManager<TIapItem, TIapCatalogue, ProductData, PurchaseResponse>
		where TIapItem : AmazonIapItem
		where TIapCatalogue : IapCatalogue<TIapItem, ProductData>
	{
		private const string SuccessfulResult = "SUCCESSFUL";
		private const string AlreadyOwnedResult = "ALREADY_PURCHASED";
		private const string FulfilledResult = "FULFILLED";
		private const float ResumeTimeout = 2.0f;

		private bool iapReady;

		/// <summary>
		/// Gets whether the IAP system is ready
		/// </summary>
		public override bool IapReady
		{
			get { return iapReady; }
		}

		private bool iapSupported;

		private float timeout = -1;

		/// <summary>
		/// Gets whether IAP is supported on this device
		/// </summary>
		public override bool IapSupported
		{
			get { return iapSupported; }
		}

		/// <summary>
		/// Abstract method to retrieve last transaction info
		/// </summary>
		/// <returns></returns>
		public override IapReceiptInfo GetLastTransactionReceiptInfo()
		{
			// Not implemented for AMZ yet. Should modify IapReceiptInfo a bit for this, I think
			Debug.LogWarning("[IAP] AMZ receipts not implemented");
			return IapReceiptInfo.Empty;
		}

		/// <summary>
		/// Register events
		/// </summary>
		protected virtual void OnEnable()
		{
			IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;

			iapService.AddGetProductDataResponseListener(ReceivedProductData);
			iapService.AddPurchaseResponseListener(PurchaseResultReceived);
			iapService.AddGetPurchaseUpdatesResponseListener(PurchaseUpdatesReceived);

			// Request purchase update info from store
			var reset = new ResetInput {Reset = false};
			iapService.GetPurchaseUpdates(reset);
		}

		/// <summary>
		/// Deregister events
		/// </summary>
		protected virtual void OnDisable()
		{
			IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;

			iapService.RemoveGetProductDataResponseListener(ReceivedProductData);
			iapService.RemovePurchaseResponseListener(PurchaseResultReceived);
			iapService.RemoveGetPurchaseUpdatesResponseListener(PurchaseUpdatesReceived);
		}

		/// <summary>
		/// On Amazon, if you lose focus, we need to reset some states 
		/// </summary>
		protected virtual void OnApplicationPause(bool pauseStatus)
		{
			if (!Validating && BuyingItem != null && !pauseStatus)
			{
				// Start timeout timer upon resumption
				timeout = ResumeTimeout;
			}
		}


		/// <summary>
		/// Tick the timeout timer
		/// </summary>
		protected override void Update()
		{
			base.Update();
			if (!Validating && BuyingItem != null && timeout > 0.0f) // Waiting for purchase?
			{
				timeout -= Time.deltaTime;
				if (timeout <= 0)
				{
					// Fail the purchase. The Amazon plugin stops giving us callbacks in these events, because meh
					FailPurchase(BuyingItem, "App suspended");

					IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;

					// Request purchase update info from store to see if stuff happened in the background
					var reset = new ResetInput {Reset = false};
					iapService.GetPurchaseUpdates(reset);
				}
			}
			else
			{
				timeout = -1;
			}
		}


		/// <summary>
		/// Called to perform logic of negotiating with the store to buy <see cref="IapManager{TIapItem,TIapCatalogue,TIapProduct,TTransaction}.BuyingItem"/>
		/// </summary>
		protected override void BuyItem()
		{
			IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;

			// Construct object passed to operation as input
			var request = new SkuInput
			{
				Sku = BuyingItem.IapId
			};

			iapService.Purchase(request);
		}

		/// <summary>
		/// Internal restore
		/// </summary>
		protected override void RestorePurchases() { }

		/// <summary>
		/// Called to retrieve product list from the store
		/// </summary>
		protected override void LoadProducts(string[] productIds)
		{
			IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;

			// Construct object passed to operation as input
			var request = new SkusInput();

			// Create list of SKU strings
			var skus = new List<string>(productIds);
			request.Skus = skus;

			iapService.GetProductData(request);
		}

		/// <summary>
		/// Received purchase updates from AMZ
		/// </summary>
		private void PurchaseUpdatesReceived(GetPurchaseUpdatesResponse purchaseUpdates)
		{
			if (purchaseUpdates.Status == SuccessfulResult)
			{
				// Just in case this is set somehow, we clear it so RestoreItem below
				// doesn't invoke it.
				Action<TIapItem, IapResult, string> prevBuyCallback = buyCallback;
				buyCallback = null;
				
				IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;

				for (var i = 0; i < purchaseUpdates.Receipts.Count; i++)
				{
					PurchaseReceipt receipt = purchaseUpdates.Receipts[i];

					TIapItem correspondingItem = GetItem(receipt.Sku);

					if ((correspondingItem != null) && !correspondingItem.Purchased)
					{
						// Fulfill item
						// Construct object passed to operation as input
						var request = new NotifyFulfillmentInput
						{
							ReceiptId = receipt.ReceiptId,
							FulfillmentResult = FulfilledResult
						};

						// Notify IAP service of fulfillment
						iapService.NotifyFulfillment(request);
						
						RestoreItem(correspondingItem);
					}
				}

				buyCallback = prevBuyCallback;

				// Paging?
				if (purchaseUpdates.HasMore)
				{
					var reset = new ResetInput {Reset = false};

					AmazonIapV2Impl.Instance.GetPurchaseUpdates(reset);
				}
				else
				{
					SavePurchases();
				}
			}
		}

		/// <summary>
		/// Event for receiving product data from the store
		/// </summary>
		private void ReceivedProductData(GetProductDataResponse productData)
		{
			Debug.Log("[IAP] Received product data");

			if (productData.Status == SuccessfulResult)
			{
				iapReady = true;
				iapSupported = true;

				foreach (KeyValuePair<string, ProductData> mapItem in productData.ProductDataMap)
				{
					TIapItem correspondingItem = GetItem(mapItem.Key);
					if (correspondingItem != null)
					{
						correspondingItem.Product = mapItem.Value;
					}
				}
			}
			else
			{
				iapSupported = false;
			}
		}

		/// <summary>
		/// Event for receiving purchase results after trying to buy an item
		/// </summary>
		private void PurchaseResultReceived(PurchaseResponse purchaseResponse)
		{
			if ((purchaseResponse.Status == SuccessfulResult) ||
				(purchaseResponse.Status == AlreadyOwnedResult))
			{
				IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;

				// Construct object passed to operation as input
				var request = new NotifyFulfillmentInput
				{
					ReceiptId = purchaseResponse.PurchaseReceipt.ReceiptId,
					FulfillmentResult = FulfilledResult
				};

				// Notify IAP service of fulfillment
				iapService.NotifyFulfillment(request);

				PurchaseItem(BuyingItem, purchaseResponse);
			}
			else
			{
				FailPurchase(BuyingItem, purchaseResponse.Status);
			}
		}
	}
}
#endif
