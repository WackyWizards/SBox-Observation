using System;
using Sandbox.Audio;
using Sandbox.UI;

namespace Observation.UI;

public partial class VideoPanel
{
	// This should be named src for compatibility and html standards.
	// ReSharper disable once InconsistentNaming
	[Parameter]
	public string src
	{
		get
		{
			return _source;
		}

		set
		{
			if ( _source == value )
			{
				return;
			}

			if ( _videoPlayer is null )
			{
				Log.Warning( "Attempted to set source on a null VideoPlayer." );
				return;
			}

			_lastSource = _source;
			_source = value;
			_videoPlayer.Play( FileSystem.Mounted, src.Trim() );
		}
	}
	private string _source = string.Empty;
	private string _lastSource = string.Empty;

	[Parameter]
	public bool Repeat
	{
		set
		{
			if ( _videoPlayer is null )
			{
				Log.Warning( "Attempted to set repeat on a null VideoPlayer." );
				return;
			}

			_videoPlayer.Repeat = value;

			if ( value )
			{
				_videoPlayer.OnFinished = null;
			}
			else
			{
				_videoPlayer.OnFinished = () => OnVideoEnd?.Invoke();
			}
		}
	}

	[Parameter]
	public bool Muted
	{
		set
		{
			if ( _videoPlayer is null )
			{
				Log.Warning( "Attempted to set mute on a null VideoPlayer." );
				return;
			}

			_videoPlayer.Muted = value;
		}
	}

	[Parameter]
	public Mixer Mixer { get; set; } = Mixer.Master;

	public Action? OnVideoEnd { get; set; }

	private VideoPlayer? _videoPlayer;

	protected override void OnParametersSet()
	{
		if ( _videoPlayer is null )
		{
			return;
		}

		if ( src.Trim() != _lastSource?.Trim() )
		{
			_lastSource = src.Trim();
			_videoPlayer.Play( FileSystem.Mounted, src.Trim() );
		}

		base.OnParametersSet();
	}

	public VideoPanel()
	{
		_videoPlayer = new VideoPlayer();

		if ( Mixer is not null )
		{
			_videoPlayer.Audio.Volume = Mixer.Volume;
		}
		
		_videoPlayer.OnFinished = () => OnVideoEnd?.Invoke();
	}

	public override void OnDeleted()
	{
		_videoPlayer?.Dispose();
		_videoPlayer = null;
	}
	
	public void Play()
	{
		if ( _videoPlayer is null )
		{
			Log.Warning( "Unable to play video player, it is null." );
			return;
		}

		if ( string.IsNullOrWhiteSpace( src ) )
		{
			Log.Warning( "Unable to play video player, source is empty." );
			return;
		}

		_videoPlayer.Play( FileSystem.Mounted, src.Trim() );
	}

	public void Play( string source )
	{
		if ( _videoPlayer is null )
		{
			Log.Warning( "Unable to play video player, it is null." );
			return;
		}

		if ( string.IsNullOrWhiteSpace( source ) )
		{
			Log.Warning( "Unable to play video player, source is empty." );
			return;
		}

		_videoPlayer.Play( FileSystem.Mounted, source.Trim() );
	}

	public void Mute()
	{
		if ( _videoPlayer is null )
		{
			Log.Warning( "Unable to mute video player, it is null." );
			return;
		}

		_videoPlayer.Muted = true;
	}

	public void Unmute()
	{
		if ( _videoPlayer is null )
		{
			Log.Warning( "Unable to unmute video player, it is null." );
			return;
		}

		_videoPlayer.Muted = false;
	}

	public override bool HasContent => true;

	public override void DrawContent( ref RenderState state )
	{
		base.DrawContent( ref state );

		_videoPlayer?.Present();
		var texture = _videoPlayer?.Texture;
		// Todo - change the UV so we can do object-fit: cover

		Graphics.Attributes.Set( "Texture", texture );
		Graphics.DrawQuad( Box.Rect, Material.UI.Basic, Color.White.WithAlpha( Opacity ) );
	}
}
