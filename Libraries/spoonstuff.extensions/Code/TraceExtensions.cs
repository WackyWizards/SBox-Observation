using System.Collections.Generic;
using System.Linq;

namespace Sandbox;

public static class TraceExtensions
{
	public static SceneTraceResult RunRayTrace( this SceneTrace trace, Ray ray, float distance = 100f, params string[] tags )
	{
		return trace.Ray( ray, distance )
			.WithAnyTags( tags )
			.IgnoreGameObject( Game.ActiveScene.GetAllObjects( true ).FirstOrDefault( x => x.Name == "World Physics" ) )
			.Run();
	}

	public static IEnumerable<SceneTraceResult> RunAllRayTrace( this SceneTrace trace, Ray ray, float distance = 100f, params string[] tags )
	{
		return trace.Ray( ray, distance )
			.WithAnyTags( tags )
			.RunAll();
	}
}
