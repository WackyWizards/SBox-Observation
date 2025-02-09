using System;

namespace Observation;

[Category( "Anomalies" )]
public class Anomaly : Component
{
	[Property] public virtual string Name { get; set; } = string.Empty;

	[Property] public virtual string Room { get; set; } = string.Empty;

	[Property] public virtual AnomalyType Type { get; set; }

	[Property] public virtual ReportType ReportType { get; set; }

	[Property] public Action<GameObject>? OnAfterActive { get; set; }
	[Property] public Action<GameObject>? OnAfterClear { get; set; }

	protected bool IsActive => AnomalyManager.Instance.IsValid() && AnomalyManager.Instance.ActiveAnomalies.Contains(this);

	/// <summary>
	/// Called when this anomaly becomes active.
	/// </summary>
	public virtual void OnAnomalyActive()
	{
		if ( GameObject.IsValid() )
			OnAfterActive?.Invoke( GameObject );
	}

	/// <summary>
	/// When this anomaly is reported and to be cleared.
	/// </summary>
	public virtual void OnAnomalyClear()
	{
		if ( GameObject.IsValid() )
			OnAfterClear?.Invoke( GameObject );
	}

	/// <summary>
	/// If this anomaly is available for activation.
	/// </summary>
	public virtual bool IsAvailable()
	{
		if ( CameraManager.Instance?.ActiveCamera is not {} activeCamera )
			return false;

		// Check if the active camera's name is different from the room name.
		return !string.Equals( activeCamera.Name, Room, StringComparison.OrdinalIgnoreCase );
	}

	public enum AnomalyType
	{
		[Title( "Extra Object" )]
		Extra,
		[Title( "Object Movement" )]
		Movement,
		[Title( "Object Manipulation" )]
		Manipulation,
		[Title( "Object Disappearance" )]
		Disappearance,
		[Title( "Audio Anomaly" )]
		Audio,
		[Title( "Camera Malfunction" )]
		CameraMalfunction,
		[Title( "Intruder" )]
		Intruder,
	}
}

public static class AnomalyTypeExtensions
{
	public static string GetTitle( this Anomaly.AnomalyType type )
	{
		var title = type.GetAttributeOfType<TitleAttribute>();
		return title.Value ?? type.ToString();
	}
}
