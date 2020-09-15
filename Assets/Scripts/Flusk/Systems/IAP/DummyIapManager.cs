namespace Flusk.Systems.IAP
{
	public abstract class DummyIapManager<TIapItem, TIapCatalogue> : IapManager<TIapItem, TIapCatalogue, string, string>
		where TIapItem : DummyIapItem
		where TIapCatalogue : IapCatalogue<TIapItem, string>
	{
		public override TIapCatalogue Catalogue
		{
			get { throw new System.NotImplementedException(); }
			protected set { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Gets whether the IAP system is ready
		/// </summary>
		public override bool IapReady
		{
			get { return true; }
		}

		/// <summary>
		/// Gets whether IAP is supported on this device
		/// </summary>
		public override bool IapSupported
		{
			get { return true; }
		}

		/// <summary>
		/// Called to perform logic of negotiating with the store to buy <see cref="IapManager{TIapItem,TIapCatalogue,TIapProduct,TTransaction}.BuyingItem"/>
		/// </summary>
		protected override void BuyItem()
		{
			// Empty transaction
			PurchaseItem(BuyingItem, string.Empty);
		}

		/// <summary>
		/// Internal restore
		/// </summary>
		protected override void RestorePurchases()
		{
		}

		/// <summary>
		/// Called to retrieve product list from the store
		/// </summary>
		protected override void LoadProducts(string[] productIds)
		{
		}

		public override IapReceiptInfo GetLastTransactionReceiptInfo()
		{
			return IapReceiptInfo.Empty;
		}
	}
}
