using System;
using Sandbox.UI;

namespace Observation.UI;

public partial class RoomList
{
	public Anomaly.AnomalyType AnomalyType { get; set; }
	public Action<string, Anomaly.AnomalyType>? OnReport { get; set; }

	private void Report( string room )
	{
		OnReport?.Invoke( room, AnomalyType );
		this.Hide();
	}
}
