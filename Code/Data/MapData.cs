namespace Observation;

public class MapData : IDataFile<MapData>
{
	public static MapData Data
	{
		get
		{
			return FileSystem.Data.ReadJson( FileName, new MapData() );
		}
	}

	public static bool CanSave => FileSystem.Data.FileExists( FileName );

	public const string FileName = "MapData.json";

	public Dictionary<string, Rank> SRanks { get; set; } = [];

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
