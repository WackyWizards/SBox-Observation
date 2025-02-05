namespace Observation.Helpers;

/// <summary>
/// Disables all the provided game objects on start.
/// </summary>
public class GameObjectDisabler : Component
{
	[Property] public List<GameObject> GameObjects { get; set; } = [];

	protected override void OnStart()
	{
		foreach ( var gameObject in GameObjects.Where( gameObject => gameObject.IsValid() ) )
		{
			gameObject.Enabled = false;
		}
		base.OnStart();
	}
}
