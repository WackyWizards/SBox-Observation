using System;
using System.Threading.Tasks;
using Sandbox.Diagnostics;
using Sandbox.UI;
using Observation.UI;
using Observation.Platform;
using kEllie.Utils;

namespace Observation;

public sealed class AnomalyManager : Singleton<AnomalyManager>
{
	[Property, Category( "Anomalies" )]
	public List<Anomaly> ActiveAnomalies { get; set; } = [];

	[Property, Category( "Anomalies" ), InlineEditor, WideMode]
	private List<AnomalyEntry> PossibleAnomalies { get; set; } = [];

	[Property, Category( "Anomalies" )]
	private RangedFloat FirstAnomalyTime { get; set; } = (20f, 35f);

	[Property, Category( "Anomalies" )]
	private RangedFloat AnomalyTime { get; set; } = (25f, 120f);

	[Property, Category( "Anomalies" )]
	private int MaxAmountOfAnomalies { get; set; } = 5;

	[Property, Category( "Anomalies" )]
	private int MaxAnomaliesTilWarning { get; set; } = 3;

	[Property, Category( "Anomalies" )]
	private bool CanActivateAnomalies { get; set; } = true;

	[Property, Category( "Reports" )]
	private int SuccessfulReports { get; set; }

	[Property, Category( "Reports" )]
	private int FailReports { get; set; }

	[Property, Category( "Reports" )]
	private int FailReportsTilWarning { get; set; } = 3;

	[Property, Category( "Reports" )]
	private int FailReportsTilGameOver { get; set; } = 5;

	[Property, Category( "Reports" )]
	private int TotalReports
	{
		get
		{
			return SuccessfulReports + FailReports;
		}
	}

	[Property, Category( "Reports" )]
	public Rank Rank
	{
		get
		{
			return GameManager.GetRank( SuccessfulReports, ActiveAnomalies.Count, TotalReports ).rank;
		}
	}

	[Property, Category( "Reports" )]
	public int SuccessRate
	{
		get
		{
			return GameManager.GetRank( SuccessfulReports, ActiveAnomalies.Count, TotalReports ).percentage;
		}
	}

	public event Action<Anomaly>? OnAnomalyActivated;
	public event Action<Anomaly>? OnAnomalyCleared;
	public event Action<bool>? OnReportSubmitted;

	private readonly Dictionary<Anomaly, float> _removedAnomalies = new();
	private TimeUntil _nextAnomaly;
	private TimeSince _sinceStart;

	private const string ActiveAnomaliesAlert = "#warning.activeanomalies";
	private const string FailReportsAlert = "#warning.failreports";

	private static readonly Logger Log = new( "Anomaly Manager" );
	
	protected override void OnStart()
	{
		_nextAnomaly = FirstAnomalyTime.GetValue();
		_sinceStart = 0;
	}

	protected override void OnFixedUpdate()
	{
		if ( !_nextAnomaly || !CanActivateAnomalies )
		{
			return;
		}

		try
		{
			TryActivateRandomAnomaly();
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error activating anomaly: {ex.Message}" );
		}

		_nextAnomaly = AnomalyTime.GetValue();
	}

	[Button]
	private void TryActivateRandomAnomaly()
	{
		var anomaly = GetRandomPossibleAnomaly();
		if ( !anomaly.IsValid() )
		{
			return;
		}

		SetAnomalyActive( anomaly );
	}

	private void SetAnomalyActive( Anomaly anomaly )
	{
		if ( !CanSetAnomalyActive( anomaly ) )
		{
			return;
		}

		ActiveAnomalies.Add( anomaly );

		// Find the corresponding anomaly reference to remove from PossibleAnomalies
		var anomalyReference = PossibleAnomalies.FirstOrDefault( x => x.Anomaly == anomaly );
		if ( anomalyReference.Anomaly != null )
		{
			_removedAnomalies[anomaly] = anomalyReference.Weight;
			PossibleAnomalies.Remove( anomalyReference );
		}

		anomaly.OnAnomalyActive();
		OnAnomalyActivated?.Invoke( anomaly );

		Log.EditorLog( $"Activated anomaly: {anomaly}" );
		CheckAnomalyWarningThresholds();
	}

	private void CheckAnomalyWarningThresholds()
	{
		if ( ActiveAnomalies.Count == MaxAnomaliesTilWarning )
		{
			var alert = Hud.GetElement<Alert>();
			
			// Get the localized phrase by removing the # and searching the language.
			var keyWithoutHash = ActiveAnomaliesAlert[1..];
			var phrase = Language.GetPhrase( keyWithoutHash );
			alert?.WriteText( phrase );
		}

		if ( ActiveAnomalies.Count < MaxAmountOfAnomalies )
		{
			return;
		}

		CanActivateAnomalies = false;
		GameManager.EndGameInLoss( GameManager.LoseReason.TooManyAnomalies );
	}

	private bool CanSetAnomalyActive( Anomaly anomaly )
	{
		return anomaly.IsValid() &&
			anomaly.IsAvailable() &&
			!ActiveAnomalies.Contains( anomaly );
	}

	private Anomaly? GetRandomPossibleAnomaly()
	{
		var availableAnomalies = PossibleAnomalies
			.Where( x =>
				x.Anomaly.IsValid() &&
				x.Anomaly.IsAvailable() &&
				IsPassedMinTime( x ) )
			.ToList();

		if ( availableAnomalies.Count == 0 )
		{
			return null;
		}

		var totalWeight = availableAnomalies.Sum( x => x.Weight );

		var randomValue = Game.Random.Float( 0, totalWeight );
		var cumulativeWeight = 0f;

		foreach ( var reference in availableAnomalies )
		{
			cumulativeWeight += reference.Weight;
			if ( randomValue <= cumulativeWeight )
			{
				return reference.Anomaly;
			}
		}

		return null;
	}

	private bool IsPassedMinTime( AnomalyEntry entry )
	{
		if ( !entry.MinTime.HasValue )
		{
			return true;
		}

		return _sinceStart >= entry.MinTime.Value;
	}

	public async Task Report( Anomaly.AnomalyType type, string room )
	{
		var reportingScreen = await ShowReportingScreen();
		if ( !reportingScreen.IsValid() )
		{
			return;
		}

		try
		{
			var anomaly = GetValidActiveAnomaly( room );
			var isValidReport = anomaly?.Type == type;

			await ProcessReport( isValidReport, reportingScreen, anomaly, type );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error processing room report: {ex.Message}" );
			reportingScreen.Hide();
		}
	}

	public async Task Report( Anomaly.AnomalyType type, GameObject gameObject )
	{
		var reportingScreen = await ShowReportingScreen();
		if ( !reportingScreen.IsValid() )
		{
			return;
		}

		try
		{
			var anomaly = GetValidActiveAnomaly( gameObject );
			var isValidReport = anomaly?.Type == type;

			await ProcessReport( isValidReport, reportingScreen, anomaly, type );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error processing object report: {ex.Message}" );
			reportingScreen.Hide();
		}
	}

	private async Task<ReportingScreen?> ShowReportingScreen()
	{
		var reportingScreen = Hud.GetElement<ReportingScreen>();
		if ( !reportingScreen.IsValid() )
		{
			return null;
		}

		if ( reportingScreen.Report.IsValid() )
		{
			reportingScreen.Report.Text = "Verifying Report...";
		}
		reportingScreen.Show();

		await Task.DelayRealtimeSeconds( 2 );
		return reportingScreen;
	}

	private async Task ProcessReport( bool isValidReport, ReportingScreen reportingScreen, Anomaly? anomaly, Anomaly.AnomalyType type )
	{
		reportingScreen.SetReport( isValidReport );
		OnReportSubmitted?.Invoke( isValidReport );

		if ( !isValidReport )
		{
			await HandleFailedReport( reportingScreen, type );
			return;
		}

		await Task.DelayRealtimeSeconds( 1 );
		reportingScreen.Hide();

		if ( isValidReport && anomaly.IsValid() )
		{
			SuccessfulReports++;
			Sandbox.Services.Stats.Increment( "report_success", 1 );
			Sandbox.Services.Stats.Increment( $"report_success_{type}", 1 );

			if ( type == Anomaly.AnomalyType.Rock )
			{
				Platform.Achievement.InconvenientRock.Unlock();
			}
			
			ClearAnomaly( anomaly );
		}
	}

	private async Task HandleFailedReport( ReportingScreen reportingScreen, Anomaly.AnomalyType? type = null )
	{
		FailReports++;

		Sandbox.Services.Stats.Increment( "report_fail", 1 );
		if ( type.HasValue )
		{
			Sandbox.Services.Stats.Increment( $"report_fail_{type.Value}", 1 );
		}

		Log.Info( $"Failed Reports: {FailReports} | Type: {type}" );

		if ( FailReports == FailReportsTilWarning )
		{
			var alert = Hud.GetElement<Alert>();
			
			// Get the localized phrase by removing the # and searching the language.
			var keyWithoutHash = FailReportsAlert[1..];
			var phrase = Language.GetPhrase( keyWithoutHash );
			alert?.WriteText( phrase );
		}

		if ( FailReports >= FailReportsTilGameOver )
		{
			reportingScreen.Hide();
			GameManager.EndGameInLoss( GameManager.LoseReason.FailReports );
			return;
		}

		await Task.DelayRealtimeSeconds( 1 );
		reportingScreen.Hide();
	}

	private Anomaly? GetValidActiveAnomaly( GameObject gameObject )
	{
		if ( !gameObject.IsValid() )
		{
			return null;
		}

		var anomaly = gameObject.Components.Get<Anomaly>();
		return anomaly.IsValid() && ActiveAnomalies.Contains( anomaly ) ? anomaly : null;
	}

	private Anomaly? GetValidActiveAnomaly( string room )
	{
		return ActiveAnomalies.FirstOrDefault( x => x.Room == room );
	}

	private void ClearAnomaly( Anomaly anomaly )
	{
		if ( !ActiveAnomalies.Contains( anomaly ) )
		{
			Log.Warning( $"Unable to clear anomaly: {anomaly}, not active!" );
			return;
		}

		ActiveAnomalies.Remove( anomaly );
		anomaly.OnAnomalyClear();

		// Restore original weight if available, otherwise use a default value
		var weight = _removedAnomalies.GetValueOrDefault( anomaly, 1.0f );
		PossibleAnomalies.Add( new AnomalyEntry
		{
			Anomaly = anomaly, Weight = weight
		} );

		OnAnomalyCleared?.Invoke( anomaly );
		Log.Info( $"Cleared anomaly: {anomaly}" );
	}
	
	public void SetFailReportLimits( Difficulty difficulty )
	{
		switch ( difficulty )
		{
			case Difficulty.Easy or Difficulty.Normal:
				FailReportsTilWarning = int.MaxValue;
				FailReportsTilGameOver = int.MaxValue;
				break;
			case Difficulty.Hard:
				FailReportsTilWarning = 3;
				FailReportsTilGameOver = 5;
				break;
			default:
				throw new ArgumentOutOfRangeException( nameof(difficulty) );
		}
	}
}

public record struct AnomalyEntry
{
	public Anomaly Anomaly { get; set; }
	public float Weight { get; set; }

	/// <summary>
	/// The minimum amount of passed time before this anomaly can be activated.
	/// </summary>
	public float? MinTime { get; set; }
}
