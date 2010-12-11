using System;

namespace WebOnDiet.Framework.Routes
{
	public abstract class AbstractRouteAttribute : Attribute, IRouteAttribute
	{
		public int Precedence { get; set; }
		public string Route { get; protected set; }
	}
}