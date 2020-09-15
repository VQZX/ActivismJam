#if USE_AMAZON_IAP && UNITY_ANDROID && !UNITY_EDITOR
using com.amazon.device.iap.cpt;
using UnityEngine;

namespace TFBGames.Systems.IAP
{
	[SerializeField]
	public abstract class AmazonIapItem : IIapItem<ProductData>
	{
		public abstract string IapId { get; }
		public abstract bool Purchased { get; }

		public string LocalisedPrice
		{
			get { return Product == null ? "$0.99" : Product.Price; }
		}

		/// <summary>
		/// This is invalid on Amazon platforms
		/// </summary>
		public string CurrencyCode
		{
			get { return null; }
		}

		public string Title
		{
			get { return Product == null ? "PlaceholderTitle" : Product.Title; }
		}


		public ProductData Product { get; set; }
	}
}
#endif
