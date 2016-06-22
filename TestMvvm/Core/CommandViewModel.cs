using System;
using System.Windows.Input;

namespace TestMvvm.Core
{
	public class CommandViewModel : ViewModelBase
	{
		public ICommand Command
		{
			get;
			private set;
		}

		public CommandViewModel(string displayName, ICommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			base.DisplayName = displayName;
			this.Command = command;
		}
	}
}
