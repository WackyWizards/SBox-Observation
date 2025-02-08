namespace Observation.Platform;

public enum Achievement
{
	[Title( "win_house" )]
	WinHouse,
	[Title( "win_pool" )]
	WinPool
}

public static class AchievementExtensions
{
	public static string GetName( this Achievement achievement )
	{
		var title = achievement.GetAttributeOfType<TitleAttribute>();
		return title.Value ?? achievement.ToString();
	}

	public static void Unlock( this Achievement achievement )
	{
		var name = achievement.GetName();
		Sandbox.Services.Achievements.Unlock( name );

		Log.Info( $"Unlocked achievement: {name}" );
	}
}
