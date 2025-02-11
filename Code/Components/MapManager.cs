using System;
namespace Observation;

public class MapManager : Component
{
	public static MapManager? Instance { get; private set; }

	[Property, InlineEditor, ReadOnly] public Map? ActiveMap { get; private set; }

	protected override void OnStart()
	{
		Instance = this;
		GameObject.Flags = GameObjectFlags.DontDestroyOnLoad;
		base.OnStart();
	}

	public void SetMap( Map map )
	{
		ActiveMap = map;
		Sandbox.Services.Stats.Increment( $"Plays_{map.Ident}", 1 );
		Sandbox.Services.Stats.Increment( $"Plays_{map.Ident}_with_difficulty_{map.Difficulty}", 1 );
	}

	public void Restart()
	{
		if ( ActiveMap is null )
		{
			return;
		}

		var scene = ActiveMap.Scene;
		if ( scene.IsValid() )
			Scene.Load( scene );

		Sandbox.Services.Stats.Increment( "Restarts", 1 );
		Sandbox.Services.Stats.Increment( $"Restarts_on_map_{ActiveMap.Ident}", 1 );
		Sandbox.Services.Stats.Increment( $"Restarts_on_map_{ActiveMap.Ident}_with_difficulty_{ActiveMap.Difficulty}", 1 );
	}
}
