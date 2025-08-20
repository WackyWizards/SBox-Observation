using kEllie.Utils;

namespace Observation;

public enum Difficulty
{
	Easy,
	[LocalizedName( "#ui.mapinfo.difficulty.normal" )]
	Normal,
	[LocalizedName( "#ui.mapinfo.difficulty.hard" )]
	Hard
}
