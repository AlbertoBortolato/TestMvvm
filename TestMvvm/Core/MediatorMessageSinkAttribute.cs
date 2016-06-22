using System;

namespace TestMvvm.Core
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class MediatorMessageSinkAttribute : Attribute
	{
		public object MessageKey
		{
			get;
			private set;
		}

		public MediatorMessageSinkAttribute()
		{
			this.MessageKey = Mediator.DefaultKey;
		}

		public MediatorMessageSinkAttribute(string messageKey)
		{
			this.MessageKey = messageKey;
		}
	}
}
