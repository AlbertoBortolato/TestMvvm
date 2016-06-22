using System;
using System.Reflection;

namespace TestMvvm.Core
{
	public class Singleton<T> where T : class
	{
		private static object SyncRoot = new object();

		private static T instance;

		public static T Instance
		{
			get
			{
				if (Singleton<T>.instance == null)
				{
					lock (Singleton<T>.SyncRoot)
					{
						if (Singleton<T>.instance == null)
						{
							ConstructorInfo constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
							if (constructor == null)
							{
								throw new InvalidOperationException("class must contain a private constructor");
							}
							Singleton<T>.instance = (T)((object)constructor.Invoke(null));
						}
					}
				}
				return Singleton<T>.instance;
			}
			set
			{
				lock (Singleton<T>.SyncRoot)
				{
					Singleton<T>.instance = value;
				}
			}
		}
	}
}
