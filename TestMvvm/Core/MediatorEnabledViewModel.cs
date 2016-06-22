using System;

namespace TestMvvm.Core
{
	public class MediatorEnabledViewModel<T> : WorkspaceViewModel
	{
		public Mediator Mediator
		{
			get
			{
				return Mediator.Instance;
			}
		}

		public MediatorEnabledViewModel()
		{
			this.Mediator.Register(this);
		}
	}
}
