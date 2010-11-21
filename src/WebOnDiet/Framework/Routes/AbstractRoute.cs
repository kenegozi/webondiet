using System;
using System.Collections;
using System.Reflection;

namespace WebOnDiet.Framework.Routes
{
	public abstract class AbstractRoute : IRoute
	{
		public abstract RouteMatch Match(string path);

		public TargetMethod Target { get; internal set; }
	}

	public class TargetMethod
	{
		readonly MethodInfo _methodInfo;

		public TargetMethod(MethodInfo methodInfo)
		{
			_methodInfo = methodInfo;
			Parameters = _methodInfo.GetParameters();
		}

		public Type DeclaringType { get { return _methodInfo.DeclaringType; } }

		public ParameterInfo[] Parameters { get; private set; }

		public object Invoke(object instance, params object[] parameters)
		{
			try
			{
				return _methodInfo.Invoke(instance, parameters);
			}
			catch (TargetInvocationException tie)
			{
				throw;
			}
		}
	}
}