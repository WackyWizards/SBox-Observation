using System;

namespace kEllie.Utils;

[AttributeUsage( AttributeTargets.Field )]
public class IdentAttribute( string ident ) : Attribute
{
	public string Value { get; private set; } = ident;
}
