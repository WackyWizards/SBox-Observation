using System;

namespace Observation.UI;

public partial class MapSelection
{
	private List<Map>? Maps => Menu?.Maps;

	private void SelectMap( Map map )
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
