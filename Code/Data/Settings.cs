namespace Observation;

public class Settings : IDataFile<Settings>
{
	public static Settings? Data
	{
		get
		{
			_data = FileSystem.Data.ReadJson( FileName, new Settings() );
			return _data;
		}
	}
	private static Settings? _data;

	public const string FileName = "Settings.json";

	public float MasterVolume { get; set; } = 0.5f;
	public float GameVolume { get; set; } = 0.5f;
	public float MusicVolume { get; set; } = 0.5f;
	public float UIVolume { get; set; } = 0.5f;
	
	public void Save()
	{		
		FileSystem.Data.WriteJson( FileName, this );
	}
}
