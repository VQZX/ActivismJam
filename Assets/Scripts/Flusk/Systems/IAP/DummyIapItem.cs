namespace Flusk.Systems.IAP
{
	public abstract class DummyIapItem : IIapItem<string>
	{
		/// <summary>
		/// IAP item id
		/// </summary>
		public virtual string IapId
		{
			get { return "Placeholder"; }
		}

		/// <summary>
		/// Gets from game data whether this item has been purchased
		/// </summary>
		public abstract bool Purchased { get; }

		/// <summary>
		/// Retrieve localised price
		/// </summary>
		public string LocalisedPrice
		{
			get { return "$ ?.??"; }
		}

		/// <summary>
		/// Retrieve currency code
		/// </summary>
		public string CurrencyCode
		{
			get { return "$"; }
		}

		/// <summary>
		/// Retrieve title
		/// </summary>
		public string Title
		{
			get { return "PlaceholderTitle"; }
		}

		/// <summary>
		/// The IAP product as represented by IAP framework
		/// </summary>
		public string Product
		{
			get { return "Placeholder"; }
		}
	}
}
