namespace Observation.Anomalies;

public class Enabler : Anomaly
{
	[Property] public override AnomalyType Type { get; set; } = AnomalyType.Extra;

	[Property] public List<GameObject> GameObjects { get; set; } = [];
	[Property] public List<Component> ComponentList { get; set; } = [];

	public override void OnAnomalyActive()
	{
		Toggle();
		base.OnAnomalyActive();
	}

	public override void OnAnomalyClear()
	{
		Toggle();
		base.OnAnomalyClear();
	}

	public void Toggle()
	{
		foreach ( var gameObject in GameObjects.Where( x => x.IsValid() ) )
		{
			gameObject.Enabled = !gameObject.Enabled;
		}

		foreach ( var component in ComponentList.Where( x => x.IsValid() ) )
		{
			component.Enabled = !component.Enabled;
		}
	}

	public void Enable()
	{
		foreach ( var gameObject in GameObjects.Where( x => x.IsValid() ) )
		{
			gameObject.Enabled = true;
		}

		foreach ( var component in ComponentList.Where( x => x.IsValid() ) )
		{
			component.Enabled = true;
		}
	}

	public void Disable()
	{
		foreach ( var gameObject in GameObjects.Where( x => x.IsValid() ) )
		{
			gameObject.Enabled = false;
		}

		foreach ( var component in ComponentList.Where( x => x.IsValid() ) )
		{
			component.Enabled = false;
		}
	}
}
