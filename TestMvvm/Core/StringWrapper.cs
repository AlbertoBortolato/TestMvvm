using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TestMvvm.Core
{
	[XmlType(AnonymousType = true, Namespace = "http://new.webservice.namespace")]
	[Serializable]
	public class StringWrapper : INotifyPropertyChanged
	{
		private string _value;

		private string _oldValue;

		public event PropertyChangedEventHandler PropertyChanged;

		[XmlText]
		public string Value
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
		public string PreviousValue
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

		public StringWrapper()
		{
			this._value = "";
		}

		public StringWrapper(string s)
		{
			this._value = s;
		}

		public override string ToString()
		{
			return this.Value;
		}

		public bool Equals(string s)
		{
			int hashCode = s.GetHashCode();
			int hashCode2 = this.Value.GetHashCode();
			return hashCode == hashCode2;
		}

		public bool Equals(StringWrapper sw)
		{
			int hashCode = sw.Value.GetHashCode();
			int hashCode2 = this.Value.GetHashCode();
			return hashCode == hashCode2;
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
