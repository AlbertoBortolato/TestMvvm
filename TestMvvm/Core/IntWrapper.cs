using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TestMvvm.Core
{
	[XmlType(AnonymousType = true, Namespace = "http://new.webservice.namespace")]
	[Serializable]
	public class IntWrapper : INotifyPropertyChanged
	{
		private int _value;

		private int _oldValue;

		public event PropertyChangedEventHandler PropertyChanged;

		[XmlText]
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this.PreviousValue = this._value;
				this._value = value;
				this.OnPropertyChanged("Value");
			}
		}

		[XmlIgnore]
		public int PreviousValue
		{
			get
			{
				return this._oldValue;
			}
			set
			{
				this._oldValue = value;
				this.OnPropertyChanged("PreviousValue");
			}
		}

		public IntWrapper()
		{
			this._value = 0;
		}

		public IntWrapper(int s)
		{
			this._value = s;
		}

		public IntWrapper(double s)
		{
			this._value = (int)s;
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}

		public bool Equals(IntWrapper iw)
		{
			int hashCode = iw.GetHashCode();
			int hashCode2 = this.GetHashCode();
			return hashCode == hashCode2;
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if ((int)obj != 0)
			{
				int hashCode = ((int)obj).GetHashCode();
				int hashCode2 = this.GetHashCode();
				result = (hashCode == hashCode2);
			}
			else if (obj is IntWrapper)
			{
				int hashCode3 = (obj as IntWrapper).GetHashCode();
				int hashCode4 = this.GetHashCode();
				result = (hashCode3 == hashCode4);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
