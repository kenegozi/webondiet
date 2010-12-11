namespace WebOnDiet.Framework.Routes
{
	public interface IRouteAttribute
	{
		int Precedence { get; }
		string Route { get; }
	}
}