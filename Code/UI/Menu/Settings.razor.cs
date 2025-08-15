using Sandbox.Audio;

namespace Observation.UI;

public partial class Settings
{
	private static Mixer Master => Mixer.Master;
	private static Mixer[] Mixers => Mixer.Master.GetChildren();

	private static void Save()
	{
		var data = Observation.Settings.Data;
		data.MasterVolume = Master.Volume;

		// Loop through each sub-mixer and save its volume
		foreach ( var mixer in Mixers )
		{
			// Settings volume property names follow the convention "<MixerName>Volume"
			var propertyName = mixer.Name + "Volume";
			var volume = mixer.Volume;
			var success = TypeLibrary.SetProperty( data, propertyName, volume );

			if ( !success )
			{
				Log.Error( "Unable to set mixer volume property in settings, is it properly named?" );
			}
		}

		data.Save();
	}
}
