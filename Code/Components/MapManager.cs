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
	}

	public void Restart()
	{
		if ( ActiveMap is null )
		{
			return;
		}
		
		var scene = ActiveMap.Scene;
		Scene.Load( scene );
	}
}
