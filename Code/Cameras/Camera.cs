namespace Observation.Cameras;

public class Camera : Anomaly
{
	public override AnomalyType Type => AnomalyType.CameraMalfunction;
	
	private static CameraManager? CameraManager => CameraManager.Instance;

	public override void OnAnomalyActive()
	{
		base.OnAnomalyActive();
		CameraManager?.PossibleCameras.Remove( this );
	}

	public override void OnAnomalyClear()
	{
		base.OnAnomalyClear();
		CameraManager?.PossibleCameras.Add( this );
	}
}
