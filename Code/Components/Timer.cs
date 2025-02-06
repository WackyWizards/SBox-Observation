namespace Observation;

public class Timer : Component
{
	public static Timer? Instance { get; private set; }

	[Property, Range( 1, 999, step: 1 )] public float WinTime { get; set; }

	public TimeSince SinceStart { get; set; }
	public TimeUntil UntilVictory { get; set; }

	protected override void OnStart()
	{
		Instance = this;
		SinceStart = 0;
		UntilVictory = WinTime;

		base.OnStart();
	}

	protected override void OnUpdate()
	{
		if ( UntilVictory )
		{
			if ( MapManager.Instance is { ActiveMap: not null } mapManager )
				GameManager.Instance?.EndGameInWin( mapManager.ActiveMap );

			UntilVictory = 999999;
		}

		base.OnUpdate();
	}

	public string GetElapsedTime()
	{
		var totalSeconds = (int)(SinceStart / 1.3);
		var minutes = totalSeconds / 60;
		var seconds = totalSeconds % 60;

		return $"{minutes:D2}:{seconds:D2}";
	}
}
