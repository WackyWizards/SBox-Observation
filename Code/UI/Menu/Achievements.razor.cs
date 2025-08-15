namespace Observation.UI;

public partial class Achievements
{
	private readonly List<AchievementModel> _achievements = [];

	public Achievements()
	{
		var achievements = Sandbox.Services.Achievements.All;
		_achievements.AddRange( achievements.Select( achievement => new AchievementModel
		{
			NameLocalizationKey = $"#achievement.{achievement.Name}.name",
			DescriptionLocalizationKey = $"#achievement.{achievement.Name}.description",
			Achievement = achievement
		} ) );
	}

	private readonly struct AchievementModel
	{
		public required string NameLocalizationKey { get; init; }
		public required string DescriptionLocalizationKey { get; init; }
		public required Achievement Achievement { get; init; }
	}
}
