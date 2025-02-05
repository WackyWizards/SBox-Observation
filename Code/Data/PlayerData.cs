namespace Observation;

public class PlayerData
{
	public static PlayerData Data
	{
		get
		{
			return FileSystem.Data.ReadJson( FileName, new PlayerData() );
		}
	}
	
	private const string FileName = "data.json";

	public ObserverRank Rank { get; set; } = ObserverRank.Newbie;
	
	public void Load()
	{
		FileSystem.Data.ReadJson( FileName, new PlayerData() );
	}

	public void Save()
	{
		FileSystem.Data.WriteJson( FileName, this );
	}
}
