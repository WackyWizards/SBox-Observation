using System;

namespace Observation.UI;

public partial class Timer
{
	private static GameTimer? TimerInstance => GameTimer.Instance;
	private static string ElapsedTime => TimerInstance?.GetDisplayTime() ?? "N/A";

	protected override int BuildHash()
	{
		return HashCode.Combine( TimerInstance?.SinceStart.Relative );
	}
}
