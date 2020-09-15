namespace Flusk.Systems.IAP
{
	public interface IIapItem<T>
	{
		/// <summary>
		/// IAP item id
		/// </summary>
		string IapId { get; }

		/// <summary>
		/// Gets from game data whether this item has been purchased
		/// </summary>
		bool Purchased { get; }

		/// <summary>
		/// Retrieve localised price
		/// </summary>
		string LocalisedPrice { get; }

		/// <summary>
		/// Retrieve currency code
		/// </summary>
		string CurrencyCode { get; }

		/// <summary>
		/// Retrieve title
		/// </summary>
		string Title { get; }

		/// <summary>
		/// The IAP product as represented by IAP framework
		/// </summary>
		T Product { get; }
	}
}
