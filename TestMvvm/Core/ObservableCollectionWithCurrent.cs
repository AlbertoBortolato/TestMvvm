using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Input;

namespace TestMvvm.Core
{
	public class ObservableCollectionWithCurrent<T> : ObservableCollection<T>
	{
		private LinkedList<T> _copyList;

		private RelayCommand _copyCommand;

		private RelayCommand _cutCommand;

		private RelayCommand _pasteCommand;

		private RelayCommand _moveUpCommand;

		private RelayCommand _moveDownCommand;

		private RelayCommand _addBeforeCommand;

		private RelayCommand _addAfterCommand;

		private RelayCommand _deleteCommand;

		public ICollectionView DefaultView
		{
			get
			{
				return CollectionViewSource.GetDefaultView(this);
			}
		}

		public T CurrentItem
		{
			get
			{
				if (this.DefaultView.CurrentItem == null)
				{
					this.DefaultView.MoveCurrentToNext();
				}
				return (T)((object)this.DefaultView.CurrentItem);
			}
			set
			{
				this.MoveCurrentTo(value);
			}
		}

		public int CurrentPosition
		{
			get
			{
				return this.DefaultView.CurrentPosition;
			}
		}

		public LinkedList<T> CopyList
		{
			get
			{
				if (this._copyList == null)
				{
					this._copyList = new LinkedList<T>();
				}
				return this._copyList;
			}
			private set
			{
				this._copyList = value;
			}
		}

		public ICommand CopyCommand
		{
			get
			{
				if (this._copyCommand == null)
				{
					this._copyCommand = new RelayCommand(delegate(object param)
					{
						if (this.CurrentItem != null)
						{
							if (this.CopyList.Contains(this.CurrentItem))
							{
								this.CopyList.Remove(this.CurrentItem);
							}
							while (this.CopyList.Count > 15)
							{
								this.CopyList.RemoveLast();
							}
							this.CopyList.AddFirst(this.CurrentItem);
						}
					}, (object param) => param is T);
				}
				return this._copyCommand;
			}
		}

		public ICommand CutCommand
		{
			get
			{
				if (this._cutCommand == null)
				{
					this._cutCommand = new RelayCommand(delegate(object param)
					{
						if (this.CurrentItem != null)
						{
							if (this.CopyList.Contains(this.CurrentItem))
							{
								this.CopyList.Remove(this.CurrentItem);
							}
							while (this.CopyList.Count > 15)
							{
								this.CopyList.RemoveLast();
							}
							this.CopyList.AddFirst(this.CurrentItem);
							base.Remove(this.CurrentItem);
						}
					}, (object param) => param is T);
				}
				return this._cutCommand;
			}
		}

		public ICommand PasteCommand
		{
			get
			{
				if (this._pasteCommand == null)
				{
					this._pasteCommand = new RelayCommand(checked(delegate(object param)
					{
						if (this.CurrentItem != null)
						{
							T t = this.CopyList.First<T>();
							if (t != null)
							{
								int currentPosition = this.CurrentPosition;
								base.Add(t);
								if (currentPosition + 1 < base.Count)
								{
									this.MoveItem(base.IndexOf(t), currentPosition + 1);
								}
								else
								{
									this.MoveItem(base.IndexOf(t), currentPosition - 1);
								}
							}
						}
					}), (object param) => this.CopyList.Count > 0);
				}
				return this._pasteCommand;
			}
		}

		public ICommand MoveUpCommand
		{
			get
			{
				if (this._moveUpCommand == null)
				{
					this._moveUpCommand = new RelayCommand(delegate(object param)
					{
						if (this.CurrentItem != null)
						{
							this.MoveItem(this.CurrentPosition, checked(this.CurrentPosition - 1));
						}
					}, (object param) => param is T && base.IndexOf(this.CurrentItem) > 0);
				}
				return this._moveUpCommand;
			}
		}

		public ICommand MoveDownCommand
		{
			get
			{
				if (this._moveDownCommand == null)
				{
					this._moveDownCommand = checked(new RelayCommand(delegate(object param)
					{
						if (this.CurrentItem != null)
						{
							this.MoveItem(this.CurrentPosition, this.CurrentPosition + 1);
						}
					}, (object param) => param is T && base.IndexOf(this.CurrentItem) < base.Count - 1));
				}
				return this._moveDownCommand;
			}
		}

		public ICommand AddBeforeCommand
		{
			get
			{
				if (this._addBeforeCommand == null)
				{
					this._addBeforeCommand = new RelayCommand(delegate(object param)
					{
						if (this.CurrentItem != null)
						{
							int currentPosition = this.CurrentPosition;
							T item = this.AddNew();
							this.MoveItem(base.IndexOf(item), currentPosition);
						}
					}, (object param) => param is T);
				}
				return this._addBeforeCommand;
			}
		}

		public ICommand AddAfterCommand
		{
			get
			{
				if (this._addAfterCommand == null)
				{
					this._addAfterCommand = new RelayCommand(delegate(object param)
					{
						if (this.CurrentItem != null)
						{
							int currentPosition = this.CurrentPosition;
							T item = this.AddNew();
							this.MoveItem(base.IndexOf(item), checked(currentPosition + 1));
						}
					}, (object param) => param is T);
				}
				return this._addAfterCommand;
			}
		}

		public ICommand DeleteCommand
		{
			get
			{
				if (this._deleteCommand == null)
				{
					this._deleteCommand = new RelayCommand(delegate(object param)
					{
						if (this.CurrentItem != null)
						{
							this.RemoveCurrent();
						}
					}, (object param) => param is T);
				}
				return this._deleteCommand;
			}
		}

		public ObservableCollectionWithCurrent(List<T> list) : base(list)
		{
		}

		public ObservableCollectionWithCurrent(IEnumerable<T> collection) : base(collection)
		{
		}

		public ObservableCollectionWithCurrent()
		{
		}

		public T AddNew()
		{
			Type typeFromHandle = typeof(T);
			ConstructorInfo constructor = typeFromHandle.GetConstructor(new Type[0]);
			T t = (T)((object)constructor.Invoke(new object[0]));
			base.Add(t);
			this.MoveCurrentTo(t);
			return t;
		}

		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T current in collection)
			{
				if (!base.Contains(current))
				{
					base.Add(current);
				}
			}
		}

		public void RemoveCurrent()
		{
			T currentItem = this.CurrentItem;
			if (base.IndexOf(currentItem) > 0)
			{
				this.MovePrevious();
			}
			else
			{
				this.MoveNext();
			}
			base.Remove(currentItem);
		}

		public void RemoveRange(IEnumerable<T> collection)
		{
			foreach (T current in collection)
			{
				if (base.Contains(current))
				{
					base.Remove(current);
				}
			}
		}

		public void MoveFirst()
		{
			this.DefaultView.MoveCurrentToFirst();
		}

		public void MovePrevious()
		{
			this.DefaultView.MoveCurrentToPrevious();
		}

		public void MoveNext()
		{
			this.DefaultView.MoveCurrentToNext();
		}

		public void MoveLast()
		{
			this.DefaultView.MoveCurrentToLast();
		}

		public void MoveCurrentTo(T item)
		{
			bool flag = this.DefaultView.MoveCurrentTo(item);
			Console.Out.Write(flag.ToString());
			if (!flag)
			{
				this.MoveCurrentToIndexOf(item);
			}
		}

		public void MoveCurrentToPosition(int pos)
		{
			this.DefaultView.MoveCurrentToPosition(pos);
		}

		public void MoveCurrentToIndexOf(T item)
		{
			int position = base.IndexOf(item);
			bool flag = this.DefaultView.MoveCurrentToPosition(position);
			Console.Out.Write(flag.ToString());
		}

		public void Sort(string property, ListSortDirection direction)
		{
			this.DefaultView.SortDescriptions.Add(new SortDescription(property, direction));
		}

		public new T MoveItem(int oldIndex, int newIndex)
		{
			base.Move(oldIndex, newIndex);
			T currentItem = base.Items[newIndex];
			this.CurrentItem = currentItem;
			return this.CurrentItem;
		}

		public void SetPasteCommand(RelayCommand pasteCommand)
		{
			this._pasteCommand = pasteCommand;
		}
	}
}
