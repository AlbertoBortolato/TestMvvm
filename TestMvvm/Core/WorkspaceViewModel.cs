using System;
using System.Windows.Input;

namespace TestMvvm.Core
{
	public abstract class WorkspaceViewModel : ViewModelBase
	{
		private RelayCommand _closeCommand;

		private bool _isBusy;

		private string _busyMessage;

		public event EventHandler RequestClose;

		public ICommand CloseCommand
		{
			get
			{
				if (this._closeCommand == null)
				{
					this._closeCommand = new RelayCommand(delegate(object param)
					{
						this.OnRequestClose();
					});
				}
				return this._closeCommand;
			}
		}

		public bool IsBusy
		{
			get
			{
				return this._isBusy;
			}
			set
			{
				if (value != this._isBusy)
				{
					this._isBusy = value;
					if (this._isBusy)
					{
						this.WaitCursorActivate();
					}
					else
					{
						this.WaitCursorDeactivate();
					}
					this.OnPropertyChanged("IsBusy");
				}
			}
		}

		public string BusyMessage
		{
			get
			{
				if (string.IsNullOrEmpty(this._busyMessage))
				{
					this._busyMessage = "...";
				}
				return this._busyMessage;
			}
			set
			{
				if (this._busyMessage != value)
				{
					this._busyMessage = value;
					this.OnPropertyChanged("BusyMessage");
				}
			}
		}

		private void OnRequestClose()
		{
			EventHandler requestClose = this.RequestClose;
			if (requestClose != null)
			{
				requestClose(this, EventArgs.Empty);
			}
		}

		private void WaitCursorActivate()
		{
			Mouse.OverrideCursor = Cursors.Wait;
		}

		private void WaitCursorDeactivate()
		{
			Mouse.OverrideCursor = null;
		}
	}
}
