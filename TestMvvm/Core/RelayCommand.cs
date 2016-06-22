using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TestMvvm.Core
{
	public class RelayCommand : ICommand
	{
		private readonly Action<object> _execute;

		private readonly Predicate<object> _canExecute;

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		public RelayCommand(Action<object> execute) : this(execute, null)
		{
		}

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}
			this._execute = execute;
			this._canExecute = canExecute;
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
