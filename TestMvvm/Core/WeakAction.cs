using System;
using System.Reflection;

namespace TestMvvm.Core
{
	public class WeakAction
	{
		private readonly WeakReference _target;

		private readonly Type _ownerType;

		private readonly Type _actionType;

		private readonly string _methodName;

		public Type ActionType
		{
			get
			{
				return this._actionType;
			}
		}

		public bool HasBeenCollected
		{
			get
			{
				return this._ownerType == null && (this._target == null || !this._target.IsAlive);
			}
		}

		public WeakAction(object target, Type actionType, MethodBase mi)
		{
			if (target == null)
			{
				this._ownerType = mi.DeclaringType;
			}
			else
			{
				this._target = new WeakReference(target);
			}
			this._methodName = mi.Name;
			this._actionType = actionType;
		}

		public Delegate GetMethod()
		{
			if (this._ownerType != null)
			{
				return Delegate.CreateDelegate(this._actionType, this._ownerType, this._methodName);
			}
			if (this._target != null && this._target.IsAlive)
			{
				object target = this._target.Target;
				if (target != null)
				{
					return Delegate.CreateDelegate(this._actionType, target, this._methodName);
				}
			}
			return null;
		}
	}
}
