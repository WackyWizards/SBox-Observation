using Observation.UI;
using Sandbox.UI;
using WackyLib.Patterns;

namespace Observation;

public sealed class PauseManager : Singleton<PauseManager>
{
	[Property, ReadOnly]
	private bool IsPaused => Scene.TimeScale == 0;
	
	protected override void OnUpdate()
	{
		if ( !Input.EscapePressed )
		{
			return;
		}

		Input.EscapePressed = false;
		PauseGame();
	}

	private void PauseGame()
	{
		if ( !CanPause() )
		{
			return;
		}
		
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
