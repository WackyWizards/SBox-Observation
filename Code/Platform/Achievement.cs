namespace Observation.Platform;

public enum Achievement
{
	[Title( "win_house" )]
	WinHouse,
	[Title( "win_pool" )]
	WinPool,
	[Title( "win_house_hard" )]
	WinHouseHard,
	[Title( "win_pool_hard" )]
	WinPoolHard,
	[Title( "s_rank_house" )]
	SRankHouse,
	[Title( "s_rank_pool" )]
	SRankPool,
	[Title( "s_house_hard" )]
	SRankHouseHard,
	[Title( "s_pool_hard" )]
	SRankPoolHard,
	[Title( "win_all_maps" )]
	WinAllMaps,
	[Title( "s_all_maps" )]
	SRankAllMaps,
	[Title( "inconvenient_roc" )]
	InconvenientRock
}

public static class AchievementExtensions
{
	private static string GetTitle( this Achievement achievement )
	{
		var title = achievement.GetAttributeOfType<TitleAttribute>();
		return title.Value ?? achievement.ToString();
	}

	public static void Unlock( this Achievement achievement )
	{
		var ident = achievement.GetTitle();
		Sandbox.Services.Achievements.Unlock( ident );

		Log.Info( $"Unlocked achievement: {ident}" );
	}
}
