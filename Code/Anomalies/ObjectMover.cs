namespace Observation.Anomalies;

public class ObjectMover : Anomaly
{
	[Property]
	public override AnomalyType Type { get; set; } = AnomalyType.Movement;
	
	/// <summary>
	/// The actual position we want to move this anomaly to when it's active.
	/// </summary>
	[Property]
	public Transform DesiredWorldTransform { get; set; }

	/// <summary>
	/// Keep track of the old position to move us back to.
	/// </summary>
	[Property]
	public Transform DefaultWorldTransform { get; set; }

	public override void OnAnomalyActive()
	{
		WorldTransform = DesiredWorldTransform;
		base.OnAnomalyActive();
	}

	public override void OnAnomalyClear()
	{
		WorldTransform = DefaultWorldTransform;
		base.OnAnomalyClear();
	}
}
