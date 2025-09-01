﻿using System;
using System.Linq;

namespace Sandbox;

public static class SceneExtensions
{
	/// <summary>
	/// Searches the scene for any enabled gameobjects with the DontDestroyOnLoad flag and destroys them.
	/// </summary>
	/// <param name="scene">The scene to search in.</param>
	/// <param name="ignored">Component types to ignore deletion.</param>
	public static void DestroyPersistentObjects( this Scene scene, Type[] ignored = null )
	{
		var objects = scene.GetAllObjects( true )
			.Where( x => x.Flags == GameObjectFlags.DontDestroyOnLoad );

		if ( ignored is null || ignored.Length == 0 )
		{
			foreach ( var go in objects )
			{
				go.Destroy();
			}
			return;
		}

		var ignoredSet = ignored.ToHashSet();
		foreach ( var go in objects )
		{
			var hasIgnoredType = ignoredSet.Any( type => go.Components.Get( type ) is not null );
			if ( !hasIgnoredType )
			{
				go.Destroy();
			}
		}
	}

	/// <summary>
	/// Search the scene for a specific component instance.
	/// </summary>
	/// <typeparam name="T">Component to search for.</typeparam>
	/// <param name="scene">The scene to search in.</param>
	/// <returns>Found component instance.</returns>
	/// <exception cref="InvalidOperationException">Thrown if no instance or multiple instances are found.</exception>
	public static T GetInstance<T>( this Scene scene ) where T : Component
	{
		var components = scene.GetAllComponents<T>().ToList();
		return components.Count switch
		{
			0 => throw new InvalidOperationException( $"No instance of type {typeof(T).Name} found in scene." ),
			1 => components[0],
			_ => throw new InvalidOperationException( $"Multiple instances of type {typeof(T).Name} found in scene. Expected singleton." )
		};
	}
}
