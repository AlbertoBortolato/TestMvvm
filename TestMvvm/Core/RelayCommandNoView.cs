using System;
using System.Diagnostics;

namespace TestMvvm.Core
{
	public class RelayCommandNoView : ICommandNoView
	{
		private readonly Action<object> _execute;

		private readonly Predicate<object> _canExecute;

		private event EventHandler _canExecuteChanged;

		public event EventHandler CanExecuteChanged
		{
			add
			{
				this._canExecuteChanged += value;
			}
			remove
			{
				this._canExecuteChanged -= value;
			}
		}

		public RelayCommandNoView(Action<object> execute) : this(execute, null)
		{
		}

		public RelayCommandNoView(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}
			this._execute = execute;
			this._canExecute = canExecute;
		}

		public virtual void RaiseCanExecuteChanged()
		{
			EventHandler canExecuteChanged = this._canExecuteChanged;
			if (canExecuteChanged != null)
			{
				canExecuteChanged(this, new EventArgs());
			}
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return this._canExecute == null || this._canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			this._execute(parameter);
		}
	}
}
