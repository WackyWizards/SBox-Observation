namespace Observation.Platform;

public enum Achievement
{
	[Title( "win_house" )]
	WinHouse,
	[Title( "win_pool" )]
	WinPool,
	[Title( "s_rank_house" )]
	SRankHouse,
	[Title( "s_rank_pool" )]
	SRankPool
}

public static class AchievementExtensions
{
	public static string GetTitle( this Achievement achievement )
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
