using System;
using Sandbox.Audio;
using Sandbox.UI;

namespace Observation.UI;

public partial class MainMenu
{
	[Property, InlineEditor, WideMode] public List<Map> Maps { get; set; } = [];
	[Property] public SoundEvent? Music { get; set; }
	
	public SoundHandle? MenuMusic { get; set; }
	
	private MapSelection? _mapSelection;
	private Settings? _settings;
	private Tutorial? _tutorial;
	private Leaderboards? _leaderboards;
	private Credits? _credits;
	private Achievements? _achievements;
	private VideoPanel? _splashScreen;
	private Panel? _webContainer;
	private WebPanel? _web;
	
	private const string DiscordInvite = "https://discord.gg/kKU6a4AYNk";
	private const string NewsUrl = "https://sbox.game/spoonstuff/observation/news";

	protected override void OnTreeFirstBuilt()
	{
		// Capture references for the asynchronous lambda
		// Prevent the issue of these being null by the time the lambda executes.
		var gameObject = GameObject;
		var mixer = Mixer.FindMixerByName( "Music" );

		if ( _splashScreen.IsValid() )
		{
			_splashScreen.Play( "/videos/KUO_SPLASH.mp4" );
			_splashScreen.OnVideoEnd += () =>
			{
				_splashScreen.Hide();

				if ( !Music.IsValid() )
					return;

				if ( MenuMusic.IsValid() )
					return;

				MenuMusic = gameObject?.PlaySound( Music );
				if ( MenuMusic is not null && mixer is not null )
				{
					MenuMusic.TargetMixer = mixer;
				}
			};
		}

		base.OnTreeFirstBuilt();
	}

	protected override void OnStart()
	{
		var settings = Observation.Settings.Data;
		if ( settings is not null )
		{
			Mixer.FindMixerByName( "Master" ).Volume = settings.MasterVolume;
			Mixer.FindMixerByName( "Music" ).Volume = settings.MusicVolume;
			Mixer.FindMixerByName( "Game" ).Volume = settings.GameVolume;
			Mixer.FindMixerByName( "UI" ).Volume = settings.UIVolume;
		}

		base.OnStart();
	}

	private void Play()
	{
		var data = PlayerData.Data;

		if ( data is null )
		{
			FileSystem.Data.WriteJson( PlayerData.FileName, new PlayerData() );
			data = PlayerData.Data;
		}

		if ( data?.FirstTime ?? true )
		{
			// ReSharper disable AccessToModifiedClosure
			WarningPanel? warning = null;
			warning = new WarningPanel( "#warning.tutorial.title", "#warning.tutorial.text", [
				new Button( "#ui.button.yes", "check_circle", () =>
				{
					warning?.Delete();
					Tutorial( () => _mapSelection?.Show() );
				} ),

				new Button( "#ui.button.no", "cancel", () =>
				{
					_mapSelection?.Show();
					warning?.Delete();
				} )
			] );
			Panel.AddChild( warning );
		}
		else
		{
			_mapSelection?.Show();
		}
	}

	private void Settings()
	{
		_settings?.Show();
	}

	private void Tutorial( Action? onReturn = null )
	{
		if ( !_tutorial.IsValid() )
			return;

		_tutorial.Show();
		_tutorial.OnReturn = onReturn;
	}

	private void Achievements()
	{
		_achievements?.Show();
	}

	private void Leaderboards()
	{
		_leaderboards?.Show();
	}

	private void Credits()
	{
		_credits?.Show();
	}

	private void Discord()
	{
		_webContainer.Show();
		if ( _web.IsValid() )
			_web.Url = DiscordInvite;
	}

	private void News()
	{
		_webContainer.Show();
		if ( _web.IsValid() )
			_web.Url = NewsUrl;
	}

	private void CloseWebPanel()
	{
		_webContainer.Hide();
	}
}
