using Sandbox.Audio;

namespace Observation.UI;

public partial class Settings
{
	private static Mixer Master => Mixer.Master;
	private static Mixer[] Mixers => Mixer.Master.GetChildren();

	private static void Save()
	{
		var data = Observation.Settings.Data;
		if ( data is null )
		{
			return;
		}

		data.MasterVolume = Master.Volume;

		foreach ( var mixer in Mixers )
		{
			var propertyName = mixer.Name + "Volume";
			var volume = mixer.Volume;
			var success = TypeLibrary.SetProperty( data, propertyName, volume );

			if ( !success )
			{
				Log.Error( "Unable to set volume property in settings, is it properly named?" );
			}
		}

		data.Save();
	}
}
