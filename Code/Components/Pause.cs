using Observation.UI;
using Sandbox.UI;
namespace Observation;

public class Pause : Component
{
	public static Pause? Instance { get; private set; }

	[Property, ReadOnly] public bool IsPaused => Scene.TimeScale == 0;
	
	protected override void OnStart()
	{
		Instance = this;
		base.OnStart();
	}

	protected override void OnUpdate()
	{
		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			PauseGame();
		}
		
		base.OnUpdate();
	}

	public void PauseGame()
	{
		if ( !CanPause() )
			return;
		
		Scene.TimeScale = 0;
		Hud.GetElement<PauseMenu>()?.Show();
	}

	public void UnpauseGame()
	{
		Scene.TimeScale = 1;
		Hud.GetElement<PauseMenu>()?.Hide();
		Cursor.HideMouse();
	}

	public void TogglePause()
	{
		if ( IsPaused )
		{
			UnpauseGame();
		}
		else
		{
			PauseGame();
		}
	}

	private bool CanPause()
	{
		return Scene.TimeScale > 0;
	}
}
