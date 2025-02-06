namespace Observation.Anomalies;

public class DelayedSound : Anomaly
{
	[Property] public BaseSoundComponent? SoundComponent { get; set; }

	[Property] public bool UseRandomTime { get; set; }

	[Property, ShowIf( nameof( UseRandomTime ), false )] public float Delay { get; set; }

	[Property, ShowIf( nameof( UseRandomTime ), true )] public float MinTime { get; set; }

	[Property, ShowIf( nameof( UseRandomTime ), true )] public float MaxTime { get; set; }

	[Property] public bool IsLooping { get; set; }

	private TimeUntil _soundTimer;

	public override void OnAnomalyActive()
	{
		if ( !SoundComponent.IsValid() )
			return;

		SoundComponent.Enabled = true;
		_soundTimer = !UseRandomTime ? Delay : GetRandomTime();

		base.OnAnomalyActive();
	}

	public override void OnAnomalyClear()
	{
		if ( SoundComponent.IsValid() )
		{
			SoundComponent.Enabled = false;
			Stop();
		}

		base.OnAnomalyClear();
	}

	protected override void OnUpdate()
	{
		if ( IsActive && _soundTimer )
		{
			Play();
		}

		base.OnUpdate();
	}

	private void Play()
	{
		SoundComponent?.StartSound();
		_soundTimer = IsLooping ? GetRandomTime() : int.MaxValue;
	}

	private void Stop()
	{
		SoundComponent?.StopSound();
	}

	private float GetRandomTime()
	{
		return Game.Random.Float( MinTime, MaxTime );
	}
}
