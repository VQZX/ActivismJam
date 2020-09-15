#if UNITY_ANDROID && USE_PRIME31_IAP && USE_IAP
using System;
using System.Collections.Generic;
using Disney.DI;
using Prime31;
using TFBGames.Build;
using UnityEngine;

namespace TFBGames.Systems.IAP
{
	public class DisneyIapValidatorGoogle<TIapItem> : MonoBehaviour,
		IIapValidator<TIapItem, IAPProduct, GooglePurchase>
		where TIapItem: class, IIapItem<IAPProduct>
	{
		private TIapItem validatingItem;
		private Action<bool, string> validationCallback;

		/// <summary>
		/// Initialize this validator
		/// </summary>
		public virtual void Initialize(IAPEnvironment environment)
		{
			InAppPurchase.Init(gameObject.name, environment);
		}

		public void TryValidate(TIapItem item, GooglePurchase transactionToValidate, Action<bool, string> callback)
		{
			if (validatingItem != null)
			{
				throw new InvalidOperationException("Still validating another purchase.");
			}

			validatingItem = item;
			validationCallback = callback;

			Dictionary<string, object> purchaseData = new Dictionary<string, object>();

			BuildConfig config;

			Debug.Log("[VALIDATOR] Validating purchase");

			if (BuildConfig.TryGetInstance(out config) && config.Platform == TargetPlatform.Amazon)
			{
				// TODO: Implement for AMZ
				purchaseData.Add(Constants.PLATFORM, "amazon");
				purchaseData.Add(Constants.USER_ID, "...");
				purchaseData.Add(Constants.PURCHASE_TOKEN_AMAZON, "...");
				InAppPurchase.VerifyPurchase(purchaseData);
			}
			else
			{
				// Alternatively, for Google Play
				purchaseData.Add(Constants.PLATFORM, Constants.PLATFORM_GOOGLEPLAY);
				purchaseData.Add(Constants.APP_ID, Application.identifier);
				purchaseData.Add(Constants.SIGNED_DATA, transactionToValidate.originalJson);
				purchaseData.Add(Constants.SIGNATURE, transactionToValidate.signature);
				InAppPurchase.VerifyPurchase(purchaseData);
			}
		}

		/// <summary>
		/// Callback from library when a result is returned
		/// </summary>
		/// <param name="responseFromLibrary"></param>
		public void Receive(string responseFromLibrary)
		{
			Debug.Log("[VALIDATOR] String response: " + responseFromLibrary);

			IDictionary<string, object> result = InAppPurchase.VerificationResult(responseFromLibrary);

			// Success?
			object rawCode;

			if (result.TryGetValue(Constants.STATUS_CODE, out rawCode))
			{
				// Integer from json?
				var code = rawCode as int?;
				if ((code.HasValue) &&
					(code.Value == Constants.STATUS_SUCCESS))
				{
					Succeed();
					return;
				}

				// String from json?
				var codeStr = rawCode as string;
				int parsedStr;
				if ((codeStr != null) &&
					int.TryParse(codeStr, out parsedStr) &&
					(parsedStr == Constants.STATUS_SUCCESS))
				{
					Succeed();
					return;
				}
			}

			// Try get an error message
			object rawMessage;
			var error = "UNKNOWN ERROR";
			if (result.TryGetValue(Constants.STATUS_MESSAGE, out rawMessage))
			{
				var message = rawMessage as string;
				if (message != null)
				{
					error = message;
				}
			}

			Fail(error);
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
