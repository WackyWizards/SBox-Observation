using System;
using System.Threading.Tasks;
using Sandbox.Diagnostics;
using Sandbox.UI;
using Observation.UI;

namespace Observation;

public class AnomalyManager : Component
{
	public static AnomalyManager? Instance { get; private set; }

	[Property, Category( "Anomalies" )]
	public List<Anomaly> ActiveAnomalies { get; set; } = [];

	[Property, Category( "Anomalies" )]
	public List<Anomaly> PossibleAnomalies { get; set; } = [];

	[Property, Category( "Anomalies" )]
	public float AnomalyTime { get; set; } = 100f;

	[Property, Category( "Anomalies" )]
	public int MaxAmountOfAnomalies { get; set; } = 5;

	[Property, Category( "Anomalies" )]
	public int MaxAnomaliesTilWarning { get; set; } = 3;

	[Property, Category( "Anomalies" ), ReadOnly]
	public bool CanActivateAnomalies { get; private set; } = true;

	[Property, Category( "Reports" ), ReadOnly]
	public int SuccessfulReports { get; private set; }

	[Property, Category( "Reports" ), ReadOnly]
	public int FailReports { get; private set; }

	[Property, Category( "Reports" )]
	public int FailReportsTilWarning { get; set; } = 3;

	[Property, Category( "Reports" )]
	public int FailReportsTilGameOver { get; set; } = 5;

	private TimeUntil _nextAnomaly;
	private readonly Logger _logger = new( "Anomaly Manager" );

	private const string ActiveAnomaliesWarning = "Attention employee, multiple active anomalies detected in your area. Locate and report them immediately.";
	private const string FailReportsWarning = "Attention employee, excessive false reports detected. Report only active anomalies or face termination.";

	public event Action<Anomaly>? OnAnomalyActivated;
	public event Action<Anomaly>? OnAnomalyCleared;
	public event Action<bool>? OnReportSubmitted;

	protected override void OnStart()
	{
		Instance = this;
		_nextAnomaly = AnomalyTime;
		base.OnStart();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( !_nextAnomaly || !CanActivateAnomalies )
			return;

		try
		{
			TryActivateRandomAnomaly();
		}
		catch ( Exception ex )
		{
			_logger.Error( $"Error activating anomaly: {ex.Message}" );
		}

		_nextAnomaly = AnomalyTime;
	}

	private void TryActivateRandomAnomaly()
	{
		var anomaly = GetRandomPossibleAnomaly();
		if ( anomaly is null || !anomaly.IsValid() )
			return;

		SetAnomalyActive( anomaly );
	}

	public void SetAnomalyActive( Anomaly anomaly )
	{
		if ( !CanSetAnomalyActive( anomaly ) )
			return;

		ActiveAnomalies.Add( anomaly );
		PossibleAnomalies.Remove( anomaly );
		anomaly.OnAnomalyActive();
		OnAnomalyActivated?.Invoke( anomaly );

		if ( Game.IsEditor)
			_logger.Info( $"Activated anomaly: {anomaly}" );

		CheckAnomalyWarningThresholds();
	}

	private void CheckAnomalyWarningThresholds()
	{
		if ( ActiveAnomalies.Count == MaxAnomaliesTilWarning )
		{
			var info = Hud.GetElement<Info>();
			info?.WriteText( ActiveAnomaliesWarning );
		}

		if ( ActiveAnomalies.Count >= MaxAmountOfAnomalies )
		{
			CanActivateAnomalies = false;
			GameManager.Instance?.EndGameInLoss( GameManager.LoseReason.TooManyAnomalies );
		}
	}

	public bool CanSetAnomalyActive( Anomaly anomaly )
	{
		return anomaly is { IsValid: true } &&
			anomaly.IsAvailable() &&
			!ActiveAnomalies.Contains( anomaly );
	}

	public Anomaly? GetRandomPossibleAnomaly()
	{
		var availableAnomalies = PossibleAnomalies
			.Where( x => x is { IsValid: true } && x.IsAvailable() )
			.ToList();

		return availableAnomalies.Count > 0 ? Game.Random.FromList( availableAnomalies! ) : null;
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

			await ProcessReport( isValidReport, reportingScreen, anomaly );
		}
		catch ( Exception ex )
		{
			_logger.Error( $"Error processing room report: {ex.Message}" );
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

			await ProcessReport( isValidReport, reportingScreen, anomaly );
		}
		catch ( Exception ex )
		{
			_logger.Error( $"Error processing object report: {ex.Message}" );
			reportingScreen.Hide();
		}
	}

	private async Task<ReportingScreen?> ShowReportingScreen()
	{
		var reportingScreen = Hud.GetElement<ReportingScreen>();
		if ( !reportingScreen.IsValid() )
			return null;

		reportingScreen.Report.Text = "Verifying Report...";
		reportingScreen.Show();

		await Task.DelayRealtimeSeconds( 2 );
		return reportingScreen;
	}

	private async Task ProcessReport( bool isValidReport, ReportingScreen reportingScreen, Anomaly? anomaly )
	{
		reportingScreen.SetReport( isValidReport );
		OnReportSubmitted?.Invoke( isValidReport );

		if ( !isValidReport )
		{
			await HandleFailedReport( reportingScreen );
			return;
		}

		await Task.DelayRealtimeSeconds( 1 );
		reportingScreen.Hide();

		if ( anomaly.IsValid() )
		{
			SuccessfulReports++;
			ClearAnomaly( anomaly );
		}
	}

	private async Task HandleFailedReport( ReportingScreen reportingScreen )
	{
		FailReports++;
		
		if ( Game.IsEditor)
			_logger.Info( $"Failed Reports: {FailReports}" );

		if ( FailReports == FailReportsTilWarning )
		{
			var info = Hud.GetElement<Info>();
			info?.WriteText( FailReportsWarning );
		}

		if ( FailReports >= FailReportsTilGameOver )
		{
			reportingScreen.Hide();
			GameManager.Instance?.EndGameInLoss( GameManager.LoseReason.FailReports );
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
			_logger.Warning( $"Unable to clear anomaly: {anomaly}, not active!" );
			return;
		}

		ActiveAnomalies.Remove( anomaly );
		anomaly.OnAnomalyClear();
		PossibleAnomalies.Add( anomaly );
		OnAnomalyCleared?.Invoke( anomaly );

		_logger.Info( $"Cleared anomaly: {anomaly}" );
	}
}
