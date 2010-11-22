Web on Diet
===========

A fat free, no fuss, simple and straightforward web framework.
Currently targeting .NET framework, Mono support coming soon. At some point, a Java port will also be created.


Example
-------

The 'hello world' of this is to have this simple code in an empty web application:

	public class Actions
	{
		[Get("/")]
		public string Testing()
		{
			return "Hello";
		}
	}
	
And the following web.config snippet is all you need (when running in IIS):

	<handlers>
		<add name="WebOnDiet" verb="*" path="*" type="WebOnDiet.Framework.WebOnDietHttpHandlerFactory, WebOnDiet"/>
	</handlers>

and that's it !