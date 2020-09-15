#if USE_PRIME31_IAP && (UNITY_IOS || UNITY_ANDROID || UNITY_TVOS)
using Prime31;
using UnityEngine;

namespace TFBGames.Systems.IAP
{
	[SerializeField]
	public abstract class P31IapItem :IIapItem<IAPProduct>
	{
		public abstract string IapId { get; }
		public abstract bool Purchased { get; }

		public string LocalisedPrice
		{
			get { return Product == null ? "$0.99" : Product.price; }
		}

		public string CurrencyCode
		{
			get { return Product == null ? "USD" : Product.currencyCode; }
		}

		public string Title
		{
			get { return Product == null ? "PlaceholderTitle" : Product.title; }
		}


		public IAPProduct Product { get; set; }
	}
}
#endif
