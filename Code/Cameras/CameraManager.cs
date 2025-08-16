using Observation.Cameras;
using Observation.UI;
using Sandbox.Audio;
using Sandbox.UI;
using kEllie.Utils;

namespace Observation;

public sealed class CameraManager : Singleton<CameraManager>
{
	[Property, ReadOnly]
	public Camera? ActiveCamera { get; private set; }

	[Property]
	// ReSharper disable once CollectionNeverUpdated.Global
	public List<Camera> PossibleCameras { get; set; } = [];

	[Property, Category( "Inputs" ), InputAction]
	private string NextCameraInput { get; set; } = "nextcamera";

	[Property, Category( "Inputs" ), InputAction]
	private string LastCameraInput { get; set; } = "lastcamera";

	protected override void OnStart()
	{
		if ( PossibleCameras.Count > 0 )
		{
			var firstEnabledCamera = PossibleCameras.FirstOrDefault( _ => GameObject.Enabled );
			if ( firstEnabledCamera.IsValid() )
			{
				SetActiveCamera( firstEnabledCamera );
			}
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( Input.Pressed( NextCameraInput ) )
		{
			SetNextAvailableActive();
		}

		if ( Input.Pressed( LastCameraInput ) )
		{
			SetPreviousAvailableActive();
		}
	}

	private void SetActiveCamera( Camera camera )
	{
		if ( ActiveCamera.IsValid() )
		{
			DisableCamera( ActiveCamera );
		}

		ActiveCamera = camera;
		camera.GameObject.Enabled = true;

		Hud.GetElement<AnomalyList>()?.Hide();
		Hud.GetElement<RoomList>()?.Hide();

		var sound = Sound.Play( "ui.button.deny" );
		if ( sound.IsValid() )
		{
			sound.TargetMixer = Mixer.FindMixerByName( "UI" );
		}
	}

	private static void DisableCamera( Camera camera )
	{
		camera.GameObject.Enabled = false;
	}

	public void SetNextAvailableActive()
	{
		if ( !ActiveCamera.IsValid() || PossibleCameras.Count == 0 )
		{
			return;
		}

		var currentIndex = PossibleCameras.IndexOf( ActiveCamera );
		var nextIndex = currentIndex;

		foreach ( var _ in PossibleCameras )
		{
			nextIndex = (nextIndex + 1) % PossibleCameras.Count;
			if ( !CanActivateCamera( PossibleCameras[nextIndex] ) )
			{
				continue;
			}

			SetActiveCamera( PossibleCameras[nextIndex] );
			return;
		}
	}

	public void SetPreviousAvailableActive()
	{
		if ( !ActiveCamera.IsValid() || PossibleCameras.Count == 0 )
		{
			return;
		}

		var currentIndex = PossibleCameras.IndexOf( ActiveCamera );
		var prevIndex = currentIndex;

		foreach ( var _ in PossibleCameras )
		{
			prevIndex = (prevIndex - 1 + PossibleCameras.Count) % PossibleCameras.Count;
			if ( !CanActivateCamera( PossibleCameras[prevIndex] ) )
			{
				continue;
			}

			SetActiveCamera( PossibleCameras[prevIndex] );
			return;
		}
	}

	private static bool CanActivateCamera( Camera camera )
	{
		if ( AnomalyManager.Instance is { } anomalyManager )
		{
			return !anomalyManager.ActiveAnomalies.Contains( camera );
		}

		return camera.IsAvailable();
	}
}
