using System;

namespace kEllie.Utils;

/// <summary>
/// Useful to mark properties or fields with a 
/// </summary>
[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
public class LocalizedNameAttribute( string key ) : Attribute
{
	public string Value { get; private set; } = key;
}
