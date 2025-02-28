using Observation.UI;
using Sandbox.UI;
using kEllie.Utils;

namespace Observation;

public sealed class PauseManager : Singleton<PauseManager>
{
	[Property, ReadOnly] public bool IsPaused => Scene.TimeScale == 0;
	
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
		Hud.GetElement<HideCursor>()?.HideMouse();
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
