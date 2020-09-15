using System;

namespace Flusk.Systems.IAP
{
	public interface IIapValidator<in TIapItem, TIapProduct, in TTransaction>
		where TIapItem: class, IIapItem<TIapProduct>
	{
		void TryValidate(TIapItem item, TTransaction transactionToValidate, Action<bool, string> callback);
	}
}
