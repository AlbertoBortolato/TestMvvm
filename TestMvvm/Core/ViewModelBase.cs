using System;
using System.ComponentModel;
using System.Diagnostics;

namespace TestMvvm.Core
{
	public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public virtual string DisplayName
		{
			get;
			protected set;
		}

		protected virtual bool ThrowOnInvalidPropertyName
		{
			get;
			private set;
		}

		[Conditional("DEBUG"), DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				string message = "Invalid property name: " + propertyName;
				if (this.ThrowOnInvalidPropertyName)
				{
					throw new Exception(message);
				}
			}
		}

		public virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
			{
				PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
				propertyChanged(this, e);
			}
		}

		public void Dispose()
		{
			this.OnDispose();
		}

		protected virtual void OnDispose()
		{
		}
	}
}
