using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TestMvvm.Core
{
	public class Mediator
	{
		public static readonly string DefaultKey;

		private static readonly Mediator instance;

		private static readonly object syncLock;

		private readonly Dictionary<object, List<WeakAction>> _registeredHandlers = new Dictionary<object, List<WeakAction>>();

		private Type _objectActionType;

		private Type _stringActionType;

		private Type ObjectActionType
		{
			get
			{
				if (this._objectActionType == null)
				{
					object obj = new object();
					this._objectActionType = typeof(Action<>).MakeGenericType(new Type[]
					{
						obj.GetType()
					});
				}
				return this._objectActionType;
			}
		}

		private Type StringActionType
		{
			get
			{
				if (this._stringActionType == null)
				{
					string text = "";
					this._stringActionType = typeof(Action<>).MakeGenericType(new Type[]
					{
						text.GetType()
					});
				}
				return this._stringActionType;
			}
		}

		public static Mediator Instance
		{
			get
			{
				return Mediator.instance;
			}
		}

		static Mediator()
		{
			Mediator.DefaultKey = "_any";
			Mediator.instance = new Mediator();
			Mediator.syncLock = new object();
		}

		private Mediator()
		{
		}

		private void RegisterHandler(object key, Type actionType, Delegate handler)
		{
			WeakAction item = new WeakAction(handler.Target, actionType, handler.Method);
			lock (this._registeredHandlers)
			{
				List<WeakAction> list;
				if (this._registeredHandlers.TryGetValue(key, out list))
				{
					if (list.Count > 0)
					{
						WeakAction weakAction = list[0];
						if (weakAction.ActionType != actionType && !weakAction.ActionType.IsAssignableFrom(actionType) && !actionType.IsAssignableFrom(weakAction.ActionType))
						{
							if (!(weakAction.ActionType == this.ObjectActionType) && !(actionType == this.ObjectActionType))
							{
								throw new ArgumentException("Invalid key passed to RegisterHandler - existing handler has incompatible parameter type");
							}
							"are compatable".ToString();
						}
					}
					list.Add(item);
				}
				else
				{
					list = new List<WeakAction>
					{
						item
					};
					this._registeredHandlers.Add(key, list);
				}
			}
		}

		private void UnregisterHandler(object key, Type actionType, Delegate handler)
		{
			lock (this._registeredHandlers)
			{
				List<WeakAction> list;
				if (this._registeredHandlers.TryGetValue(key, out list))
				{
					list.RemoveAll((WeakAction wa) => handler == wa.GetMethod() && actionType == wa.ActionType);
					if (list.Count == 0)
					{
						this._registeredHandlers.Remove(key);
					}
				}
			}
		}

		private bool NotifyColleaguesInternal(object key, object message)
		{
			List<WeakAction> list = new List<WeakAction>();
			List<WeakAction> list2;
			lock (this._registeredHandlers)
			{
				if (!this._registeredHandlers.TryGetValue(key, out list2))
				{
					return false;
				}
				foreach (WeakAction current in list2)
				{
					list.Add(current);
				}
			}
			Type type = typeof(Action<>).MakeGenericType(new Type[]
			{
				message.GetType()
			});
			foreach (WeakAction current2 in list)
			{
				Delegate method = current2.GetMethod();
				Type actionType = current2.ActionType;
				if (method != null)
				{
					if (type != actionType && !type.IsAssignableFrom(actionType) && !actionType.IsAssignableFrom(type))
					{
						if (type == this.ObjectActionType || actionType == this.ObjectActionType)
						{
							method.DynamicInvoke(new object[]
							{
								message
							});
						}
						else
						{
							method.DynamicInvoke(new object[]
							{
								message.ToString()
							});
						}
					}
					else
					{
						method.DynamicInvoke(new object[]
						{
							message
						});
					}
				}
			}
			lock (this._registeredHandlers)
			{
				list2.RemoveAll((WeakAction wa) => wa.HasBeenCollected);
			}
			return true;
		}

		public void Register(object view)
		{
			MethodInfo[] methods = view.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(MediatorMessageSinkAttribute), true);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					object obj = customAttributes[j];
					MediatorMessageSinkAttribute mediatorMessageSinkAttribute = (MediatorMessageSinkAttribute)obj;
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length != 1)
					{
						throw new InvalidCastException("Cannot cast " + methodInfo.Name + " to Action<T> delegate type.");
					}
					Type type = typeof(Action<>).MakeGenericType(new Type[]
					{
						parameters[0].ParameterType
					});
					object key = mediatorMessageSinkAttribute.MessageKey ?? type;
					if (methodInfo.IsStatic)
					{
						this.RegisterHandler(key, type, Delegate.CreateDelegate(type, methodInfo));
					}
					else
					{
						this.RegisterHandler(key, type, Delegate.CreateDelegate(type, view, methodInfo.Name));
					}
				}
			}
		}

		public void Unregister(object view)
		{
			MethodInfo[] methods = view.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				object[] customAttributes = methodInfo.GetCustomAttributes(typeof(MediatorMessageSinkAttribute), true);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					object obj = customAttributes[j];
					MediatorMessageSinkAttribute mediatorMessageSinkAttribute = (MediatorMessageSinkAttribute)obj;
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length != 1)
					{
						throw new InvalidCastException("Cannot cast " + methodInfo.Name + " to Action<T> delegate type.");
					}
					Type type = typeof(Action<>).MakeGenericType(new Type[]
					{
						parameters[0].ParameterType
					});
					object key = mediatorMessageSinkAttribute.MessageKey ?? type;
					if (methodInfo.IsStatic)
					{
						this.UnregisterHandler(key, type, Delegate.CreateDelegate(type, methodInfo));
					}
					else
					{
						this.UnregisterHandler(key, type, Delegate.CreateDelegate(type, view, methodInfo.Name));
					}
				}
			}
		}

		public void RegisterHandler<T>(string key, Action<T> handler)
		{
			this.RegisterHandler(key, handler.GetType(), handler);
		}

		public void RegisterHandler<T>(Action<T> handler)
		{
			this.RegisterHandler(typeof(Action<T>), handler.GetType(), handler);
		}

		public void UnregisterHandler<T>(string key, Action<T> handler)
		{
			this.UnregisterHandler(key, handler.GetType(), handler);
		}

		public void UnregisterHandler<T>(Action<T> handler)
		{
			this.UnregisterHandler(typeof(Action<T>), handler.GetType(), handler);
		}

		public bool NotifyColleagues<T>(string key, T message)
		{
			return this.NotifyColleaguesInternal(key, message);
		}

		public bool NotifyColleagues<T>(T message)
		{
			Type actionType = typeof(Action<>).MakeGenericType(new Type[]
			{
				typeof(T)
			});
			IEnumerable<object> enumerable = from key in this._registeredHandlers.Keys
			where key is Type && ((Type)key).IsAssignableFrom(actionType)
			select key;
			bool flag = false;
			foreach (object current in enumerable)
			{
				flag |= this.NotifyColleaguesInternal(current, message);
			}
			if (!flag)
			{
				flag |= this.NotifyColleagues<T>(Mediator.DefaultKey, message);
			}
			return flag;
		}

		public void NotifyColleaguesAsync<T>(string key, T message, Action<bool> callback)
		{
			Func<string, T, bool> smaFunc = new Func<string, T, bool>(this.NotifyColleagues<T>);
			smaFunc.BeginInvoke(key, message, delegate(IAsyncResult ia)
			{
				try
				{
					bool flag = smaFunc.EndInvoke(ia);
					callback.DynamicInvoke(new object[]
					{
						flag
					});
				}
				catch
				{
				}
			}, null);
		}

		public void NotifyColleaguesAsync<T>(string key, T message)
		{
			Func<string, T, bool> smaFunc = new Func<string, T, bool>(this.NotifyColleagues<T>);
			smaFunc.BeginInvoke(key, message, delegate(IAsyncResult ia)
			{
				try
				{
					smaFunc.EndInvoke(ia);
				}
				catch
				{
				}
			}, null);
		}

		public void NotifyColleaguesAsync<T>(T message, Action<bool> callback)
		{
			Func<T, bool> smaFunc = new Func<T, bool>(this.NotifyColleagues<T>);
			smaFunc.BeginInvoke(message, delegate(IAsyncResult ia)
			{
				try
				{
					bool flag = smaFunc.EndInvoke(ia);
					callback.DynamicInvoke(new object[]
					{
						flag
					});
				}
				catch
				{
				}
			}, null);
		}

		public void NotifyColleaguesAsync<T>(T message)
		{
			Func<T, bool> smaFunc = new Func<T, bool>(this.NotifyColleagues<T>);
			smaFunc.BeginInvoke(message, delegate(IAsyncResult ia)
			{
				try
				{
					smaFunc.EndInvoke(ia);
				}
				catch
				{
				}
			}, null);
		}
	}
}
