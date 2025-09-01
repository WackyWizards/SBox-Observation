using WackyLib.Attributes;

namespace Observation.Platform;

public enum Achievement
{
	[Ident( "win_house" )]
	WinHouse,
	[Ident( "win_pool" )]
	WinPool,
	[Ident( "win_house_hard" )]
	WinHouseHard,
	[Ident( "win_pool_hard" )]
	WinPoolHard,
	[Ident( "s_rank_house" )]
	SRankHouse,
	[Ident( "s_rank_pool" )]
	SRankPool,
	[Ident( "s_house_hard" )]
	SRankHouseHard,
	[Ident( "s_pool_hard" )]
	SRankPoolHard,
	[Ident( "win_all_maps" )]
	WinAllMaps,
	[Ident( "s_all_maps" )]
	SRankAllMaps,
	[Ident( "inconvenient_roc" )]
	InconvenientRock
}

public static class AchievementExtensions
{
	private static string GetIdent( this Achievement achievement )
	{
		var ident = achievement.GetAttributeOfType<IdentAttribute>();
		return ident.Value;
	}

	public static void Unlock( this Achievement achievement )
	{
		var ident = achievement.GetIdent();
		Sandbox.Services.Achievements.Unlock( ident );

		Log.Info( $"Unlocked achievement: {ident}" );
	}
}
