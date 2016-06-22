using System;

namespace TestMvvm.Core
{
	public interface ICommandNoView
	{
		event EventHandler CanExecuteChanged;

		bool CanExecute(object parameter);

		void Execute(object parameter);
	}
}
