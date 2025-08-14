namespace Observation;

public class PlayerData : IDataFile<PlayerData>
{
	public static PlayerData Data
	{
		get
		{
			_data = FileSystem.Data.ReadJson( FileName, new PlayerData() );
			return _data;
		}
	}
	private static PlayerData? _data;

	private const string FileName = "PlayerData.json";

	/// <summary>
	/// Is this the players first time playing the game?
	/// </summary>
	public bool FirstTime { get; set; } = true;

	public ObserverRank Rank { get; set; } = ObserverRank.Newbie;

	public void Save()
	{
		FileSystem.Data.WriteJson( FileName, this );
	}
}

public enum ObserverRank
{
	[Title( "New Hire" )]
	Newbie,
	[Title( "Veteran Employee" )]
	Veteran,
	[Title( "Manager" )]
	Manager
}

public static class ObserverRankExtensions
{
	public static string GetTitle( this ObserverRank rank )
	{
		var title = rank.GetAttributeOfType<TitleAttribute>();
		return title.Value ?? rank.ToString();
	}
}
