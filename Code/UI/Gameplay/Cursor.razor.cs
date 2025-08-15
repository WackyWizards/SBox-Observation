using System;
using Sandbox.UI;

namespace Observation.UI;

public partial class Cursor
{
	private TimeSince SinceStartedHold { get; set; }
	private bool IsHolding { get; set; }
	
	private const int Margin = -10;

	public Cursor()
	{
		BindClass( "active", () => Input.Down( "attack1" ) );
	}

	public override void Tick()
	{
		base.Tick();

		var mousePos = Mouse.Position;
		var relativeX = mousePos.x / Screen.Width;
		var relativeY = mousePos.y / Screen.Height;

		Style.Left = Length.Percent( relativeX * 100 );
		Style.Top = Length.Percent( relativeY * 100 );
		Style.MarginLeft = Length.Pixels( Margin );
		Style.MarginTop = Length.Pixels( Margin );

		if ( Input.Down( "attack1" ) )
		{
			if ( !IsHolding )
			{
				IsHolding = true;
				SinceStartedHold = 0;
			}
		}
		else
		{
			IsHolding = false;
		}

		StateHasChanged();
	}

	private static float GetHoldTime()
	{
		var cursor = Observation.Cursor.Instance;
		return cursor.IsValid() ? cursor.HoldTime : 1f;
	}

	private string GetHoldProgress()
	{
		if ( !IsHolding )
		{
			return "transform: scale(0);";
		}

		var progress = Math.Min( SinceStartedHold / GetHoldTime(), 1.0f );
		return $"transform: scale({progress});";
	}
}
