using System;
using NTemplate;
using WebOnDiet.Framework;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
/// <summary>
/// Entry point for static access into WebOnDiet services
/// </summary>
public static class wod
// ReSharper restore InconsistentNaming
// ReSharper restore CheckNamespace

{
	private static bool initialized;
	static wod()
	{
		Initialize();
	}
	internal static void Initialize()
	{
		if (initialized) return;
		initialized = true;

		WebOnDietHttpHandlerFactory.Initialize();
	}
	public static Renderer Render { get; internal set; }

	public static NTemplateEngine TemplateEngine { get; set; }
}