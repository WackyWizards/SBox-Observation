using kEllie.Utils;

namespace Observation;

public sealed class MapManager : Singleton<MapManager>
{
	[Property, InlineEditor, ReadOnly] public Map? ActiveMap { get; private set; }

	protected override void OnStart()
	{
		GameObject.Flags = GameObjectFlags.DontDestroyOnLoad;
		base.OnStart();
	}

	public void SetMap( Map map )
	{
		ActiveMap = map;
		Sandbox.Services.Stats.Increment( $"Plays_{map.Ident}", 1 );
		Sandbox.Services.Stats.Increment( $"Plays_{map.Ident}_with_difficulty_{map.Difficulty}", 1 );

		var mapData = MapData.Data;

		if ( mapData is null )
		{
			return;
		}
		
		var mapsPlayed = mapData.MapsPlayed;
		if ( mapsPlayed.Contains( map.Ident ) )
		{
			return;
		}

		mapsPlayed.Add( map.Ident );
		mapData.Save();
		Sandbox.Services.Stats.Increment( "Unique_maps_played", 1 );
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
	}
}
