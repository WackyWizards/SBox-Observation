namespace Observation;

public class Settings
{
	public static Settings Data
	{
		get
		{
			return FileSystem.Data.ReadJson( FileName, new Settings() );
		}
	}

	private const string FileName = "settings.json";

	public float MasterVolume { get; set; } = 0.5f;
	public float GameVolume { get; set; } = 0.5f;
	public float MusicVolume { get; set; } = 0.5f;
	public float UIVolume { get; set; } = 0.5f;

	public void Load()
	{
		FileSystem.Data.ReadJson( FileName, new Settings() );
	}

	public void Save()
	{
		FileSystem.Data.WriteJson( FileName, this );
	}
}

public enum ObserverRank
{
	[Title("New Hire")]
	Newbie,
	[Title("Veteran Employee")]
	Veteran,
	[Title("Manager")]
	Manager
}

public static class ObserverRankExtensions
{
	public static string GetName( this ObserverRank rank )
	{
		var title = rank.GetAttributeOfType<TitleAttribute>();
		return title.Value ?? rank.ToString();
	}
}
