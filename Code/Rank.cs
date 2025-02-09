using System;

namespace Observation;

[AttributeUsage( AttributeTargets.Field )]
public class ThresholdAttribute : Attribute
{
	public int MinimumValue { get; }

	// ReSharper disable once ConvertToPrimaryConstructor
	public ThresholdAttribute( int minimumValue )
	{
		MinimumValue = minimumValue;
	}
}

public enum Rank
{
	[Threshold( 100 )]
	S,
	[Threshold( 85 )]
	A,
	[Threshold( 70 )]
	B,
	[Threshold( 50 )]
	C,
	[Threshold( 30 )]
	D,
	[Threshold( 10 )]
	E,
	[Threshold( 0 )]
	F
}
