using System;
using System.Threading.Tasks;
using Observation.Platform;
using Sandbox.Diagnostics;
using Sandbox.UI;
using Observation.UI;
using kEllie.Utils;
using CollectionExtensions=System.Collections.Generic.CollectionExtensions;

namespace Observation;

public sealed class AnomalyManager : Singleton<AnomalyManager>
{
	[Property, Category( "Anomalies" )]
	public List<Anomaly> ActiveAnomalies { get; set; } = [];

	[Property, Category( "Anomalies" ), InlineEditor, WideMode]
	public List<AnomalyEntry> PossibleAnomalies { get; set; } = [];

	[Property, Category( "Anomalies" )]
	public RangedFloat FirstAnomalyTime { get; set; } = (20f, 35f);

	[Property, Category( "Anomalies" )]
	public RangedFloat AnomalyTime { get; set; } = (25f, 120f);

	[Property, Category( "Anomalies" )]
	public int MaxAmountOfAnomalies { get; set; } = 5;

	[Property, Category( "Anomalies" )]
	public int MaxAnomaliesTilWarning { get; set; } = 3;

	[Property, Category( "Anomalies" )]
	public bool CanActivateAnomalies { get; private set; } = true;

	[Property, Category( "Reports" )]
	public int SuccessfulReports { get; private set; }

	[Property, Category( "Reports" )]
	public int FailReports { get; private set; }

	[Property, Category( "Reports" )]
	public int FailReportsTilWarning { get; set; } = 3;

	[Property, Category( "Reports" )]
	public int FailReportsTilGameOver { get; set; } = 5;

	[Property, Category( "Reports" )]
	public int TotalReports
	{
		get
		{
			return SuccessfulReports + FailReports;
		}
	}

	[Property, Category( "Reports" )] public Rank Rank
	{
		get
		{
			return GameManager.GetRank( SuccessfulReports, ActiveAnomalies.Count, TotalReports ).rank;
		}
	}

	[Property, Category( "Reports" )] public int SuccessRate
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

	private const string ActiveAnomaliesAlert = "Attention employee, multiple active anomalies detected in your area. Locate and send reports ASAP.";
	private const string FailReportsAlert = "Attention employee, excessive false reports detected. Please only report active anomalies.";

	private readonly Logger Log = new( "Anomaly Manager" );

	protected override void OnStart()
	{
		_nextAnomaly = FirstAnomalyTime.GetValue();
		_sinceStart = 0;

		base.OnStart();
	}

	protected override void OnFixedUpdate()
	{
		if ( !_nextAnomaly || !CanActivateAnomalies )
			return;

		try
		{
			TryActivateRandomAnomaly();
		}
		catch ( Exception ex )
		{
			Log.Error( $"Error activating anomaly: {ex.Message}" );
		}

		_nextAnomaly = AnomalyTime.GetValue();
		
		base.OnFixedUpdate();
	}

	[Button]
	private void TryActivateRandomAnomaly()
	{
		var anomaly = GetRandomPossibleAnomaly();
		if ( anomaly is null || !anomaly.IsValid() )
			return;

		SetAnomalyActive( anomaly );
	}

	private void SetAnomalyActive( Anomaly anomaly )
	{
		if ( !CanSetAnomalyActive( anomaly ) )
			return;

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

		if ( Game.IsEditor )
			Log.Info( $"Activated anomaly: {anomaly}" );

		CheckAnomalyWarningThresholds();
	}

	private void CheckAnomalyWarningThresholds()
	{
		if ( ActiveAnomalies.Count == MaxAnomaliesTilWarning )
		{
			var alert = Hud.GetElement<Alert>();
			alert?.WriteText( ActiveAnomaliesAlert );
		}

		if ( ActiveAnomalies.Count < MaxAmountOfAnomalies )
		{
			return;
		}

		CanActivateAnomalies = false;
		GameManager.EndGameInLoss( GameManager.LoseReason.TooManyAnomalies );
	}

	public bool CanSetAnomalyActive( Anomaly anomaly )
	{

		return anomaly?.IsValid() == true &&
			anomaly.IsAvailable() &&
			!ActiveAnomalies.Contains( anomaly );
	}

	public Anomaly? GetRandomPossibleAnomaly()
	{
		var availableAnomalies = PossibleAnomalies
			.Where( x =>
				x.Anomaly.IsValid() &&
				x.Anomaly.IsAvailable() &&
				IsPassedMinTime( x ) )
			.ToList();

		if ( availableAnomalies.Count == 0 )
			return null;

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
			return true;

		return _sinceStart >= entry.MinTime.Value;
	}

	public async Task Report( Anomaly.AnomalyType type, string room )
	{
		var reportingScreen = await ShowReportingScreen();
		if ( reportingScreen is null )
			return;

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
			return;

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
			return null;

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
			alert?.WriteText( FailReportsAlert );
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
			return null;

		var anomaly = gameObject.Components.Get<Anomaly>();
		return anomaly is { IsValid: true } && ActiveAnomalies.Contains( anomaly ) ? anomaly : null;
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
		var weight = CollectionExtensions.GetValueOrDefault( _removedAnomalies, anomaly, 1.0f );
		PossibleAnomalies.Add( new AnomalyEntry
		{
			Anomaly = anomaly, Weight = weight
		} );

		OnAnomalyCleared?.Invoke( anomaly );
		Log.Info( $"Cleared anomaly: {anomaly}" );
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
