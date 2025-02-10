using System;
using Sandbox.UI;
using Observation.Platform;
using Observation.UI;
using Achievement=Observation.Platform.Achievement;

namespace Observation;

public class GameManager : Component
{
	public static GameManager? Instance { get; private set; }

	[Property] public SceneFile? MenuScene { get; set; }

	private static readonly Dictionary<Rank, int> Thresholds = [];

	protected override void OnStart()
	{
		Instance = this;

		Thresholds.Clear();
		var ranks = Enum.GetValues<Rank>();
		foreach ( var rank in ranks )
		{
			var threshold = rank.GetAttributeOfType<ThresholdAttribute>();
			Thresholds.Add( rank, threshold.MinimumValue );
		}

		var playerData = PlayerData.Data;
		playerData.FirstTime = false;
		playerData.Save();

		base.OnStart();
	}

	public void EndGameInLoss( LoseReason reason )
	{
		Log.Info( "Game Lost!" );
		
		Sandbox.Services.Stats.Increment( "Losses", 1 );
		Sandbox.Services.Stats.Increment( $"Losses_due_to_{reason}", 1 );

		if ( MapManager.Instance?.ActiveMap is {} activeMap )
		{
			Sandbox.Services.Stats.Increment( $"Losses_on_map_{activeMap.Ident}", 1 );

			if ( AnomalyManager.Instance is {} anomalyManager )
			{
				Sandbox.Services.Stats.Increment( $"Losses_on_map_{activeMap.Ident}_with_rank_{anomalyManager.Rank}", 1 );
				Sandbox.Services.Stats.SetValue( "Success_rate", anomalyManager.SuccessRate );
				Sandbox.Services.Stats.SetValue( $"Success_rate_on_map_{activeMap.Ident}", anomalyManager.SuccessRate );
			}
		}

		var menu = Hud.GetElement<GameOver>();
		menu?.OnGameLose( reason );
		menu?.Show();

		Scene.TimeScale = 0;
	}

	public void EndGameInWin()
	{
		Log.Info( "Game Win!" );
		
		Sandbox.Services.Stats.Increment( "Wins", 1 );
		if ( MapManager.Instance?.ActiveMap is {} activeMap )
		{
			Sandbox.Services.Stats.Increment( $"Wins_on_map_{activeMap.Ident}", 1 );
			activeMap.WinAchievement?.Unlock();

			if ( AnomalyManager.Instance is {} anomalyManager )
			{
				Sandbox.Services.Stats.Increment( $"Wins_on_map_{activeMap.Ident}_with_rank_{anomalyManager.Rank}", 1 );
				Sandbox.Services.Stats.Increment( $"Wins_with_rank_{anomalyManager.Rank}", 1 );
				Sandbox.Services.Stats.SetValue( "Success_rate", anomalyManager.SuccessRate );
				Sandbox.Services.Stats.SetValue( $"Success_rate_on_map_{activeMap.Ident}", anomalyManager.SuccessRate );

				if ( anomalyManager.Rank == Rank.S )
				{
					Sandbox.Services.Stats.Increment( $"Rank_S", 1 );
					activeMap.SRankAchievement?.Unlock();
				}
			}
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

	public static Rank GetRank( int successRate )
	{
		if ( successRate is < 0 or > 100 )
			throw new ArgumentOutOfRangeException( nameof( successRate ), "Success rate must be between 0 and 100" );

		return Thresholds
			.OrderByDescending( x => x.Value )
			.First( x => successRate >= x.Value )
			.Key;
	}

	public static (Rank rank, int percentage) GetRank( int successes, int total )
	{
		if ( total == 0 ) return (Rank.F, 0);

		var percentage = (int)Math.Floor( (double)successes / total * 100 );
		percentage = Math.Min( 100, Math.Max( 0, percentage ) );

		return (GetRank( percentage ), percentage);
	}

	public enum LoseReason
	{
		[Description( "Too many failed reports" )]
		FailReports,
		[Description( "Too many active anomalies" )]
		TooManyAnomalies
	}
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
