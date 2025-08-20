namespace Observation.Anomalies;

[Title( "Enabler Anomaly" )]
public class Enabler : Anomaly
{
	[Property]
	public override AnomalyType Type { get; set; } = AnomalyType.Extra;

	// ReSharper disable CollectionNeverUpdated.Local
	[Property]
	private List<GameObject> GameObjectList { get; set; } = [];
	
	[Property]
	private List<Component> ComponentList { get; set; } = [];

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

	private void Toggle()
	{
		foreach ( var gameObject in GameObjectList.Where( x => x.IsValid() ) )
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
		foreach ( var gameObject in GameObjectList.Where( x => x.IsValid() ) )
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
		foreach ( var gameObject in GameObjectList.Where( x => x.IsValid() ) )
		{
			gameObject.Enabled = false;
		}

		foreach ( var component in ComponentList.Where( x => x.IsValid() ) )
		{
			component.Enabled = false;
		}
	}
}
