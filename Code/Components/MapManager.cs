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
	}

	public void Restart()
	{
		if ( ActiveMap is not {} map )
		{
			return;
		}
		
		var scene = ActiveMap.Scene;
		Scene.Load( scene );
	}
}
