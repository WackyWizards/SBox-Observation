using System;

namespace Observation;

[Category( "Anomalies" )]
public class Anomaly : Component
{
	[Property] public virtual string Name { get; set; } = string.Empty;
	
	[Property] public virtual string Room { get; set; } = string.Empty;
	
	[Property] public virtual AnomalyType Type { get; set; }

	[Property] public Action<GameObject>? OnAfterActive { get; set; }
	[Property] public Action<GameObject>? OnAfterClear { get; set; }

	/// <summary>
	/// Called when this anomaly becomes active.
	/// </summary>
	public virtual void OnAnomalyActive()
	{
		OnAfterActive?.Invoke( GameObject );
	}

	/// <summary>
	/// When this anomaly is reported and to be cleared.
	/// </summary>
	public virtual void OnAnomalyClear()
	{
		OnAfterClear?.Invoke( GameObject );
	}

	/// <summary>
	/// If this anomaly is able to be reported.
	/// </summary>
	public virtual bool IsAvailable()
	{
		return true;
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
	public static string GetName( this Anomaly.AnomalyType type )
	{
		var title = type.GetAttributeOfType<TitleAttribute>();
		return title.Value ?? type.ToString();
	}
}
