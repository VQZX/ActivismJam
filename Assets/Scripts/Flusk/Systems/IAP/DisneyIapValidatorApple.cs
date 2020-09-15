#if UNITY_IOS && USE_PRIME31_IAP && USE_IAP
using System;
using System.IO;
using Disney.DI;
using Prime31;
using UnityEngine;

namespace TFBGames.Systems.IAP
{
	/// <summary>
	/// IAP validator for Disney. Can only validate one item at a time, and uses Prime 31 with current
	/// implementation
	/// </summary>
	public class DisneyIapValidatorApple<TIapItem> : IIapValidator<TIapItem, IAPProduct, StoreKitTransaction>
		where TIapItem: class, IIapItem<IAPProduct>
	{
		private TIapItem validatingItem;
		private StoreKitTransaction transaction;
		private Action<bool, string> validationCallback;
		private bool refreshing;

		/// <summary>
		/// Initialize and register P31 events
		/// </summary>
		public DisneyIapValidatorApple(IAPEnvironment environment)
		{
			StoreKitManager.refreshReceiptSucceededEvent += OnReceiptRefreshed;
			StoreKitManager.refreshReceiptFailedEvent += OnReceiptFailed;

			InAppPurchase.Init(string.Empty, environment);
		}


		public void TryValidate(TIapItem item, StoreKitTransaction transactionToValidate, Action<bool, string> callback)
		{
			if (validatingItem != null)
			{
				throw new InvalidOperationException("Still validating another purchase.");
			}

			validatingItem = item;
			validationCallback = callback;
			transaction = transactionToValidate;

			DoValidate();
		}

		private void OnReceiptFailed(string obj)
		{
			// Fail validation
			Fail("Could not refresh receipt");
		}

		private void OnReceiptRefreshed()
		{
			// Go back to validation
			refreshing = true;
			DoValidate();
		}

		private void DoValidate()
		{
			// Load receipt
			string receiptPath = StoreKitBinding.getAppStoreReceiptLocation().Replace( "file://", string.Empty );

			if (!File.Exists(receiptPath))
			{
				// Try refresh and validate again
				if (!refreshing)
				{
					Debug.Log("[VALIDATION] No receipt file, refreshing");
					StoreKitBinding.refreshReceipt();
					return;
				}
				else
				{
					Fail("Couldn't get a valid receipt.");
					return;
				}
			}

			byte[] receiptData = File.ReadAllBytes( receiptPath );

			// Perform actual validation
			InAppPurchase.VerifyPurchase(receiptData, transaction.productIdentifier,
										 transaction.transactionIdentifier, OnVerified);

			// Reset
			refreshing = false;
		}

		/// <summary>
		/// Callback from verification plugin
		/// </summary>
		private void OnVerified(bool success, int code, string name, string message, string transactionId)
		{
			if (success)
			{
				Succeed();
			}
			else if ((code == 923) || (code == 922)) // Already consumed
			{
				Debug.Log("[VALIDATION] Treating 923 and 922 (ALREADY CONSUMED) as a success");
				Succeed();
			}
			else
			{
				Fail(string.Format("Failed verification with {0} ({1})\n{2}", name, code, message));
			}
		}

		private void Succeed()
		{
			Debug.Log("[VALIDATION] Validation success");
			validatingItem = null;

			if (validationCallback != null)
			{
				validationCallback(true, string.Empty);
			}
		}

		private void Fail(string error)
		{
			Debug.LogFormat("[VALIDATION] Failed to verify transaction: {0}", error);
			validatingItem = null;

			if (validationCallback != null)
			{
				validationCallback(false, error);
			}
		}
	}
}
#endif
