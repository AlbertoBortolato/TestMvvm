using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace TestMvvm.Core
{
	public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private const string CountString = "Count";

		private const string IndexerName = "Item[]";

		private const string KeysName = "Keys";

		private const string ValuesName = "Values";

		private IDictionary<TKey, TValue> _Dictionary;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		protected IDictionary<TKey, TValue> Dictionary
		{
			get
			{
				return this._Dictionary;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return this.Dictionary.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return this.Dictionary.Values;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.Dictionary[key];
			}
			set
			{
				this.Insert(key, value, false);
			}
		}

		public int Count
		{
			get
			{
				return this.Dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.Dictionary.IsReadOnly;
			}
		}

		public ObservableDictionary()
		{
			this._Dictionary = new Dictionary<TKey, TValue>();
		}

		public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
		{
			this._Dictionary = new Dictionary<TKey, TValue>(dictionary);
		}

		public ObservableDictionary(IEqualityComparer<TKey> comparer)
		{
			this._Dictionary = new Dictionary<TKey, TValue>(comparer);
		}

		public ObservableDictionary(int capacity)
		{
			this._Dictionary = new Dictionary<TKey, TValue>(capacity);
		}

		public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		{
			this._Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
		}

		public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			this._Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
		}

		public void Add(TKey key, TValue value)
		{
			this.Insert(key, value, true);
		}

		public bool ContainsKey(TKey key)
		{
			return this.Dictionary.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue tValue;
			this.Dictionary.TryGetValue(key, out tValue);
			bool flag = this.Dictionary.Remove(key);
			if (flag)
			{
				this.OnCollectionChanged();
			}
			return flag;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.Dictionary.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this.Insert(item.Key, item.Value, true);
		}

		public void Clear()
		{
			if (this.Dictionary.Count > 0)
			{
				this.Dictionary.Clear();
				this.OnCollectionChanged();
			}
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.Dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.Dictionary.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return this.Remove(item.Key);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.Dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Dictionary.GetEnumerator();
		}

		public void AddRange(IDictionary<TKey, TValue> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (items.Count > 0)
			{
				if (this.Dictionary.Count > 0)
				{
					if (items.Keys.Any((TKey k) => this.Dictionary.ContainsKey(k)))
					{
						throw new ArgumentException("An item with the same key has already been added.");
					}
					using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = items.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<TKey, TValue> current = enumerator.Current;
							this.Dictionary.Add(current);
						}
						goto IL_8F;
					}
				}
				this._Dictionary = new Dictionary<TKey, TValue>(items);
				IL_8F:
				this.OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray<KeyValuePair<TKey, TValue>>());
			}
		}

		private void Insert(TKey key, TValue value, bool add)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue tValue;
			if (!this.Dictionary.TryGetValue(key, out tValue))
			{
				this.Dictionary[key] = value;
				this.OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
				return;
			}
			if (add)
			{
				throw new ArgumentException("An item with the same key has already been added.");
			}
			if (object.Equals(tValue, value))
			{
				return;
			}
			this.Dictionary[key] = value;
			this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, tValue));
		}

		private void OnPropertyChanged()
		{
			this.OnPropertyChanged("Count");
			this.OnPropertyChanged("Item[]");
			this.OnPropertyChanged("Keys");
			this.OnPropertyChanged("Values");
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void OnCollectionChanged()
		{
			this.OnPropertyChanged();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
		{
			this.OnPropertyChanged();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
			}
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
		{
			this.OnPropertyChanged();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
			}
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
		{
			this.OnPropertyChanged();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
			}
		}
	}
}
