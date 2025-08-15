using System;
using kEllie.Utils;

namespace Observation;

public sealed class GameTimer : Singleton<GameTimer>
{
	[Property]
	private float WinTime { get; set; } = 1080;

	public TimeSince SinceStart { get; private set; }
	private TimeUntil UntilVictory { get; set; }

	protected override void OnStart()
	{
		SinceStart = 0;
		UntilVictory = WinTime;
	}

	protected override void OnUpdate()
	{
		if ( !UntilVictory )
		{
			return;
		}

		GameManager.EndGameInWin();
		UntilVictory = int.MaxValue;
	}

	public string GetDisplayTime()
	{
		var timeScale = 21600f / WinTime;
		var totalInGameSeconds = (int)(SinceStart * timeScale);

		// Clamp the total seconds to max 21,600 (06:00)
		totalInGameSeconds = Math.Min( totalInGameSeconds, 21600 );

		var hours = totalInGameSeconds / 3600;
		var minutes = (totalInGameSeconds % 3600) / 60;

		return $"{hours:D2}:{minutes:D2}";
	}
}
