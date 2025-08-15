using System;

namespace Observation.UI;

public partial class MapSelection
{
	private List<Map>? Maps => Menu?.Maps;

#pragma warning disable CA1822
	private void SelectMap( Map map )
#pragma warning restore CA1822
	{
		var mapPanel = AddChild<MapInfo>();
		mapPanel.Map = map;
		mapPanel.Menu = Menu;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Maps, Maps?.Count, Maps?.HashCombine( x => x.HighestRankAchieved ) );
	}
}
