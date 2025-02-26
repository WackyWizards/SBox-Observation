namespace Observation.Cameras;

public class Camera : Anomaly
{
	public override AnomalyType Type => AnomalyType.CameraMalfunction;

	public override ReportType ReportType => ReportType.Room;

	public override bool IsAvailable()
	{
		return CameraManager.Instance?.ActiveCamera != this;
	}
}
