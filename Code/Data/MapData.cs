namespace Observation;

public class MapData : IDataFile<MapData>
{
	public static MapData? Data
	{
		get
		{
			_data = FileSystem.Data.ReadJson( FileName, new MapData() );
			return _data;
		}
	}
	private static MapData? _data;

	public static bool CanSave => FileSystem.Data.FileExists( FileName );

	public const string FileName = "MapData.json";

	public const int MapAmount = 2;

	public List<string> MapsPlayed { get; set; } = [];

	public List<string> MapsWon { get; set; } = [];

	public List<string> SRanks { get; set; } = [];

	public void Save()
	{
		if ( !CanSave )
		{
			Log.Error( "Unable to save data! No data file exists!" );
			return;
		}

		FileSystem.Data.WriteJson( FileName, this );
	}
}
