using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flusk.Systems.IAP
{
	public abstract class IapCatalogue<TIapItem, TIapProduct> : ScriptableObject, IList<TIapItem>
		where TIapItem :IIapItem<TIapProduct>
	{
		/// <summary>
		/// Collection of all available items
		/// </summary>
		[SerializeField]
		protected List<TIapItem> items;

		#region IList
		public IEnumerator<TIapItem> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(TIapItem item)
		{
			throw new InvalidOperationException("BabySuite is read only");
		}

		public void Clear()
		{
			throw new InvalidOperationException("BabySuite is read only");
		}

		public bool Contains(TIapItem item)
		{
			return items.Contains(item);
		}

		public void CopyTo(TIapItem[] array, int arrayIndex)
		{
			items.CopyTo(array, arrayIndex);
		}

		public bool Remove(TIapItem item)
		{
			throw new InvalidOperationException("BabySuite is read only");
		}

		public int Count { get { return items.Count; } }
		public bool IsReadOnly { get { return true; } }
		public int IndexOf(TIapItem item)
		{
			return items.IndexOf(item);
		}

		public void Insert(int index, TIapItem item)
		{
			throw new InvalidOperationException("BabySuite is read only");
		}

		public void RemoveAt(int index)
		{
			throw new InvalidOperationException("BabySuite is read only");
		}

		public TIapItem this[int index]
		{
			get { return items[index]; }
			set { throw new InvalidOperationException("BabySuite is read only"); }
		}
		#endregion
	}
}
