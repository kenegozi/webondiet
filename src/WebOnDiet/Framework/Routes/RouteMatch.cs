using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace WebOnDiet.Framework.Routes
{
	public class RouteMatch
	{
		public IRoute Route;
		public int Score;
		public Dictionary<string, string[]> RouteParameters = new Dictionary<string, string[]>();
		public IHttpContext Context;
		public object[] ExtractParameters()
		{

			var parameterBags = new Dictionary<string, string[]>[3];
			parameterBags[0] = RouteParameters;
			parameterBags[1] = FromNameValueCollection(Context.Request.Form);
			parameterBags[2] = FromNameValueCollection(Context.Request.QueryString);
			var parameterValues = new object[Route.Target.Parameters.Length];
			for (var i = 0; i < Route.Target.Parameters.Length; ++i)
			{
				var parameterInfo = Route.Target.Parameters[i];
				parameterValues[i] = ParameterBinder.For(parameterInfo.ParameterType).Bind(parameterInfo.Name,
																						   parameterInfo.ParameterType,
																						   parameterBags);
			}
			return parameterValues;
		}

		static Dictionary<string, string[]> FromNameValueCollection(NameValueCollection collection)
		{
			var result = new Dictionary<string, string[]>(collection.Count, StringComparer.InvariantCultureIgnoreCase);
			for (var i = 0; i < collection.Count; ++i)
			{
				var key = collection.GetKey(i);
				if (key == null) continue;
				result.Add(key, collection.GetValues(i) ?? new string[0]);
			}
			return result;
		}
	}

	public class ParameterBinder
	{
		public static IParameterBinder For(Type type)
		{
			if (type.IsArray)
				return For(type.GetElementType());
			if (type.IsGenericTypeDefinition && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
				return For(type.GetGenericArguments()[0]);
			if (type == typeof(int))
				return Int32ParameterBinder.Instance;
			if (type == typeof(string))
				return StringParameterBinder.Instance;

			return NullParameterBinder.Instance;
		}
	}

	public interface IParameterBinder
	{
		object Bind(string name, Type type, params IDictionary<string, string[]>[] parameterBags);
	}
	public abstract class AbstractParameterBinder<T> : IParameterBinder
	{
		public object Bind(string name, Type type, params IDictionary<string, string[]>[] parameterBags)
		{
			var values = (from bag in parameterBags
						  where bag.ContainsKey(name)
						  select bag[name]).FirstOrDefault();

			if (values == null)
				return default(T);
			if (values.Length == 0)
				return default(T);
			if (type.IsArray)
				return Convert(values);
			if (type.IsGenericTypeDefinition && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
				return Convert(values);
			return Convert(values[0]);
		}

		protected abstract T Convert(string values);
		protected T[] Convert(string[] values)
		{
			return values.Select<string, T>(Convert).ToArray();
		}
	}

	public class NullParameterBinder : IParameterBinder
	{
		public static NullParameterBinder Instance = new NullParameterBinder();
		public object Bind(string name, Type type, params IDictionary<string, string[]>[] parameterBags)
		{
			return null;
		}
	}

	public class Int32ParameterBinder : AbstractParameterBinder<int>
	{
		public static Int32ParameterBinder Instance = new Int32ParameterBinder();
		protected override int Convert(string value)
		{
			return int.Parse(value);
		}
	}
	public class StringParameterBinder : AbstractParameterBinder<string>
	{
		public static StringParameterBinder Instance = new StringParameterBinder();
		protected override string Convert(string value)
		{
			return value;
		}
	}
}