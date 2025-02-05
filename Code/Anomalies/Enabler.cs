namespace Observation.Anomalies;

public class Enabler : Anomaly
{
	[Property] public override AnomalyType Type { get; set; } = AnomalyType.Extra;
	[Property] public Component? Component { get; set; }

	public override void OnAnomalyActive()
	{
		if ( Component.IsValid() )
			Component.Enabled = !Component.Enabled;
		
		base.OnAnomalyActive();
	}

	public override void OnAnomalyClear()
	{
		if ( Component.IsValid() )
			Component.Enabled = !Component.Enabled;
		
		base.OnAnomalyClear();
	}
}
