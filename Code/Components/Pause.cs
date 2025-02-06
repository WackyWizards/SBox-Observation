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

	public async void UnpauseGame()
	{
		Scene.TimeScale = 1;
		Hud.GetElement<PauseMenu>()?.Hide();
		
		// This is a hack to hide the cursor after unpausing again.
		var hideCursor = Hud.GetElement<HideCursor>();
		if ( !hideCursor.IsValid() )
		{
			return;
		}
		
		hideCursor?.Show();
		await Task.DelayRealtime( 100 );
		hideCursor?.Hide();
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
