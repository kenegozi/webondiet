using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebOnDiet.Framework.Container
{
	public class Kernel
	{
		interface IResolver
		{
			object Resolve();
		}
		private class Resolver:IResolver
		{
			protected readonly Activator Activator;
			public Resolver(Kernel kernel, Type implementationType)
			{
				Activator = new Activator(kernel, implementationType);
			}
			public virtual object Resolve()
			{
				return Activator.CreateInstance();
			}
		}

		public class InstanceResolver : IResolver
		{
			readonly object _instance;
			public InstanceResolver (Kernel kernel, object instance)
			{
				_instance = instance;
			}
			public object Resolve()
			{
				return _instance;
			}
		}

		private class SingletonResolver:Resolver
		{
			readonly object _creationLock = new object();
			private object _instance;
			public SingletonResolver(Kernel kernel, Type implementationType):base(kernel, implementationType)
			{
				InternalResolve = FirstTime;
			}

			object FirstTime()
			{
				lock (_creationLock)
				{
					_instance = base.Activator.CreateInstance();
					InternalResolve = NextTimes;
				}
				return _instance;
			}

			object NextTimes()
			{
				return _instance;
			}

			private Func<object> InternalResolve { get; set; }

			public override object Resolve()
			{
				return InternalResolve();
			}
		}

		private class Activator
		{
			readonly Type _implementationType;
			readonly Kernel _kernel;
			readonly ConstructorInfo _constructor;
			readonly ParameterInfo[] _constructorParameters;

			public Activator(Kernel kernel, Type implementationType)
			{
				_kernel = kernel;
				_implementationType = implementationType;
				_constructor = implementationType.GetConstructors()[0];
				_constructorParameters = _constructor.GetParameters();
			}

			public object CreateInstance()
			{
				var parameters = from p in _constructorParameters
								 select _kernel.Resolve(p.ParameterType);

				return _constructor.Invoke(parameters.ToArray());
	
			}
		}

		static readonly IDictionary<string, IResolver> Resolvers = new Dictionary<string, IResolver>();

		public void Register<TContract, TImplementation>()
		{
			Register(typeof(TContract), typeof(TImplementation));
		}
		public void Register(Type contractType, Type implementationType)
		{
			Resolvers[contractType.FullName] = new Resolver(this, implementationType);
		}

		public void RegisterSingleton<TContract, TImplementation>()
		{
			RegisterSingleton(typeof(TContract), typeof(TImplementation));
		}
		public void RegisterSingleton(Type contractType, Type implementationType)
		{
			Resolvers[contractType.FullName] = new SingletonResolver(this, implementationType);
		}

		public void RegisterInstance<TContract>(object instance)
		{
			RegisterInstance(typeof(TContract), instance);
		}
		public void RegisterInstance(Type contractType, object instance)
		{
			Resolvers[contractType.FullName] = new InstanceResolver(this, instance);
		}

		public T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}
		public T Resolve<T>(string key)
		{
			return (T)Resolve(key);
		}
		public object Resolve(Type contract)
		{
			return Resolve(contract.FullName);
		}
		public object Resolve(string key)
		{
			var resolver = Resolvers[key];
			return resolver.Resolve();
		}
	}
}