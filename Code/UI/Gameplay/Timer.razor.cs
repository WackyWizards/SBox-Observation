using System;

namespace Observation.UI;

public partial class Timer
{
	private static GameTimer? TimerInstance => GameTimer.Instance;
	private static string ElapsedTime => TimerInstance?.GetDisplayTime() ?? "N/A";
	private static bool ActiveCameraHasName => !string.IsNullOrEmpty( CameraManager.Instance?.ActiveCamera?.Name );

	protected override int BuildHash()
	{
		return HashCode.Combine( TimerInstance?.SinceStart.Relative, ActiveCameraHasName );
	}
}
