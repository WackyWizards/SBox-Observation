namespace Observation.Cameras;

public class Camera : Anomaly
{
	public override AnomalyType Type => AnomalyType.CameraMalfunction;
	
	public override ReportType ReportType => ReportType.Room;

	private static CameraManager? CameraManager => CameraManager.Instance;

	public override void OnAnomalyActive()
	{
		CameraManager?.PossibleCameras.Remove( this );
		base.OnAnomalyActive();
	}

	public override void OnAnomalyClear()
	{
		CameraManager?.PossibleCameras.Add( this );
		base.OnAnomalyClear();
	}

	public override bool IsAvailable()
	{
		return CameraManager?.ActiveCamera != this;
	}
}
