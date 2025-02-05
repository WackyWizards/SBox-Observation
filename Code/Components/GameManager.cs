using Observation.Platform;
using Observation.UI;
using Sandbox.UI;
using Achievement=Observation.Platform.Achievement;
namespace Observation;

public class GameManager : Component
{
	public static GameManager? Instance { get; private set; }
	
	[Property] public SceneFile? MenuScene { get; set; }
	
	public static Map? ActiveMap { get; set; }
	
	protected override void OnStart()
	{
		Instance = this;
		base.OnStart();
	}

	public void EndGameInLoss( LoseReason reason )
	{
		var menu = Hud.GetElement<GameOver>();
		menu?.OnGameLose( reason );
		menu?.Show();

		Scene.TimeScale = 0;
	}

	public void EndGameInWin( Map map )
	{
		if ( map.WinAchievement is {} achievement )
		{
			achievement.Unlock();
		}

		var menu = Hud.GetElement<GameOver>();
		menu?.OnGameEnd( true );
		menu?.Show();
		
		Scene.TimeScale = 0;
	}

	public void ToMenu()
	{
		Scene.DestroyPersistentObjects();
		Scene.Load( MenuScene );
	}

	public enum LoseReason
	{
		[Description( "Too many failed reports" )]
		FailReports,
		[Description( "Too many active anomalies" )]
		TooManyAnomalies
	}
}

public class Map
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public Difficulty Difficulty { get; set; } = Difficulty.Normal;
	public SceneFile? Scene { get; set; }
	public Achievement? WinAchievement { get; set; }
}

public enum Difficulty
{
	Easy,
	Normal,
	Hard
}

public static class LoseReasonExtensions
{
	public static string GetDescription( this GameManager.LoseReason reason )
	{
		var description = reason.GetAttributeOfType<DescriptionAttribute>();
		return description.Value ?? reason.ToString();
	}
}
