using System;

namespace TestMvvm.Core
{
	public class MediatorEnabledViewModelBase<T> : ViewModelBase
	{
		public Mediator Mediator
		{
			get
			{
				return Mediator.Instance;
			}
		}

		public MediatorEnabledViewModelBase()
		{
			this.Mediator.Register(this);
		}

		public bool Notify<T>(string key, T message)
		{
			return this.Mediator.NotifyColleagues<T>(key, message);
		}

		public bool Notify<T>(T message)
		{
			return this.Mediator.NotifyColleagues<T>(message);
		}

		public void NotifyAsync<T>(string key, T message, Action<bool> callback)
		{
			this.Mediator.NotifyColleaguesAsync<T>(key, message, callback);
		}

		public void NotifyAsync<T>(string key, T message)
		{
			this.Mediator.NotifyColleaguesAsync<T>(key, message);
		}

		public void NotifyAsync<T>(T message, Action<bool> callback)
		{
			this.Mediator.NotifyColleaguesAsync<T>(message, callback);
		}

		public void NotifyAsync<T>(T message)
		{
			this.Mediator.NotifyColleaguesAsync<T>(message);
		}
	}
}
