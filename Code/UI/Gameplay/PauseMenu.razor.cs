using Sandbox.UI;

namespace Observation.UI;

public partial class PauseMenu
{
	private Settings? _settings;

	private static void Return()
	{
		PauseManager.Instance?.UnpauseGame();
	}

	private static void Restart()
	{
		MapManager.Instance?.Restart();
	}

	private void Settings()
	{
		if ( _settings.IsValid() )
		{
			_settings.Show();
		}
	}

	private static void Menu()
	{
		GameManager.Instance?.ToMenu();
	}
}
