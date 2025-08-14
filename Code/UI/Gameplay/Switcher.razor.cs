using Observation.Cameras;

namespace Observation.UI;

public partial class Switcher
{
	private static CameraManager? CameraManager => Observation.CameraManager.Instance;
	private static Camera? ActiveCamera => CameraManager?.ActiveCamera;

	private static void NextCamera()
	{
		if ( CameraManager?.PossibleCameras is null || !CameraManager.PossibleCameras.Any() )
			return;

		if ( !ActiveCamera.IsValid() )
			return;

		CameraManager.SetNextAvailableActive();
	}

	private static void LastCamera()
	{
		if ( CameraManager?.PossibleCameras is null || !CameraManager.PossibleCameras.Any() )
			return;

		if ( !ActiveCamera.IsValid() )
			return;

		CameraManager.SetPreviousAvailableActive();
	}
}
