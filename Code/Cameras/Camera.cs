namespace Observation.Cameras;

public class Camera : Anomaly
{
	public override AnomalyType Type => AnomalyType.CameraMalfunction;

	public override ReportType ReportType => ReportType.Room;

	private static CameraManager? CameraManager => CameraManager.Instance;

	public override bool IsAvailable()
	{
		return CameraManager?.ActiveCamera != this;
	}
}
