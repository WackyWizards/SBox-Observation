using System;
using Sandbox.Internal;

namespace Observation;

[AttributeUsage( AttributeTargets.Field )]
public class IdentAttribute( string ident ) : Attribute
{
	public string Value { get; private set; } = ident;
}
