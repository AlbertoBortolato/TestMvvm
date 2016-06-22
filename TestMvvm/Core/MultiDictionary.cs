using System;
using System.Collections.Generic;

namespace TestMvvm.Core
{
	public class MultiDictionary<T, K> : Dictionary<T, List<K>>
	{
		private void EnsureKey(T key)
		{
			if (!base.ContainsKey(key))
			{
				base[key] = new List<K>(1);
				return;
			}
			if (base[key] == null)
			{
				base[key] = new List<K>(1);
			}
		}

		public void AddValue(T key, K newItem)
		{
			this.EnsureKey(key);
			base[key].Add(newItem);
		}

		public void AddValues(T key, IEnumerable<K> newItems)
		{
			this.EnsureKey(key);
			base[key].AddRange(newItems);
		}

		public bool RemoveValue(T key, K value)
		{
			if (!base.ContainsKey(key))
			{
				return false;
			}
			base[key].Remove(value);
			if (base[key].Count == 0)
			{
				base.Remove(key);
			}
			return true;
		}

		public bool RemoveAllValue(T key, Predicate<K> match)
		{
			if (!base.ContainsKey(key))
			{
				return false;
			}
			base[key].RemoveAll(match);
			if (base[key].Count == 0)
			{
				base.Remove(key);
			}
			return true;
		}
	}
}
