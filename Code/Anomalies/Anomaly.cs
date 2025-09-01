using System;
using WackyLib.Attributes;

namespace Observation;

[Category( "Anomalies" )]
public class Anomaly : Component
{
	[Property]
	public string Name { get; set; } = string.Empty;

	[Property]
	public string Room { get; set; } = string.Empty;

	[Property]
	public virtual AnomalyType Type { get; set; }

	[Property]
	public virtual ReportType ReportType { get; set; }

	/// <summary>
	/// If this anomaly can be activated while it's in view of a camera.
	/// </summary>
	[Property]
	private bool ShowOnCamera { get; set; } = false;

	[Property]
	public Action<GameObject>? OnAfterActive { get; set; }
	
	[Property]
	public Action<GameObject>? OnAfterClear { get; set; }

	protected bool IsActive => AnomalyManager.Instance.IsValid() && AnomalyManager.Instance.ActiveAnomalies.Contains( this );

	/// <summary>
	/// Called when this anomaly becomes active.
	/// </summary>
	public virtual void OnAnomalyActive()
	{
		if ( GameObject.IsValid() )
		{
			OnAfterActive?.Invoke( GameObject );
		}
	}

	/// <summary>
	/// When this anomaly is reported and to be cleared.
	/// </summary>
	public virtual void OnAnomalyClear()
	{
		if ( GameObject.IsValid() )
		{
			OnAfterClear?.Invoke( GameObject );
		}
	}

	/// <summary>
	/// If this anomaly is available for activation.
	/// </summary>
	public virtual bool IsAvailable()
	{
		if ( ShowOnCamera )
		{
			return true;
		}

		if ( CameraManager.Instance?.ActiveCamera is not { } activeCamera )
		{
			return false;
		}

		// Check if the active camera's name is different from the room name.
		return !string.Equals( activeCamera.Name, Room, StringComparison.OrdinalIgnoreCase );
	}

	public enum AnomalyType
	{
		[LocalizedName( "#report.extraobject" )]
		Extra,
		[LocalizedName( "#report.movement" )]
		Movement,
		[LocalizedName( "#report.manipulation" )]
		Manipulation,
		[LocalizedName( "#report.disappearance" )]
		Disappearance,
		[LocalizedName( "#report.audio" )]
		Audio,
		[LocalizedName( "#report.cameramalfunction" )]
		[RoomReport]
		CameraMalfunction,
		[LocalizedName( "#report.intruder" )]
		Intruder,
		[LocalizedName( "#report.other" )]
		Other,
		[LocalizedName( "#report.rock" )]
		[HideReport]
		Rock
	}
}

public static class AnomalyTypeExtensions
{
	public static string GetLocalizedName( this Anomaly.AnomalyType type )
	{
		var localizedName = type.GetAttributeOfType<LocalizedNameAttribute>();
		return localizedName.Value;
	}
}
