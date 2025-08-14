using System;
using System.Text.Json.Serialization;

namespace Observation;

public class Map
{
	public string Ident { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public Difficulty Difficulty { get; set; } = Difficulty.Normal;

	public SceneFile? Scene { get; set; }

	public Observation.Platform.Achievement? WinAchievement { get; set; }

	public Observation.Platform.Achievement? HardWinAchievement { get; set; }

	[Title( "S Rank Achievement" )] public Observation.Platform.Achievement? SRankAchievement { get; set; }
	[Title( "S Rank Achievement Hard" )] public Observation.Platform.Achievement? SRankHardAchievement { get; set; }

	[JsonIgnore, Hide]
	public Rank? HighestRankAchieved
	{
		get
		{
			var rank = GetHighestRankAchieved();
			if ( rank.HasValue )
			{
				_highestRank = rank;
			}

			return _highestRank;
		}
	}

	[Hide] private Rank? _highestRank;

	[JsonIgnore, Hide]
	public double HighestPercentageAchieved
	{
		get
		{
			var percentage = GetHighestPercentageAchieved();
			if ( percentage.HasValue )
			{
				_highestPercentage = percentage.Value;
			}

			return _highestPercentage;
		}
	}

	[Hide] private double _highestPercentage;

	[Hide] private const string GameIdent = "spoonstuff.observation";

	public Map()
	{
		var initialRank = GetHighestRankAchieved();
		_highestRank = initialRank ?? Rank.F;

		var initialPercentage = GetHighestPercentageAchieved();
		_highestPercentage = initialPercentage ?? 0.0;
	}

	private Rank? GetHighestRankAchieved()
	{
		var stats = Sandbox.Services.Stats.GetLocalPlayerStats( GameIdent );

		foreach ( var rank in Enum.GetValues<Rank>().OrderBy( r => r ) )
		{
			var statName = $"Wins_on_map_{Ident}_with_rank_{rank}";
			if ( stats.TryGet( statName, out var stat ) && stat.Value > 0 )
			{
				return rank;
			}
		}

		return null;
	}

	private double? GetHighestPercentageAchieved()
	{
		var stats = Sandbox.Services.Stats.GetLocalPlayerStats( GameIdent );

		var statName = $"Success_rate_on_map_{Ident}";
		if ( stats.TryGet( statName, out var stat ) )
		{
			return stat.Max;
		}

		return null;
	}
}
