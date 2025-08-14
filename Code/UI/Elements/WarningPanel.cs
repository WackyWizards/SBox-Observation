using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Observation.UI;

public class WarningPanel : Panel
{
	private List<Button> Buttons { get; set; } = [];

	private Action? OnClose { get; set; } = null;

	private readonly Panel _buttonContainer;

	/// <summary>
	/// Constructs a WarningPanel with a title, warning message, and optional buttons.
	/// </summary>
	public WarningPanel( string title, string warning, List<Button>? buttons = null )
	{
		var main = Add.Panel( "main" );
		main.Add.Label( title, "title" );
		main.Add.Label( warning, "warning" );

		_buttonContainer = main.Add.Panel( "buttons" );
		if ( buttons != null )
		{
			InitializeButtons( buttons );
		}
	}

	/// <summary>
	/// Sets the buttons displayed in the panel, replacing any existing ones.
	/// </summary>
	public void SetButtons( List<Button> buttons )
	{
		Buttons = buttons;
		_buttonContainer.DeleteChildren();
		AddButtonsToContainer( Buttons );
	}

	private void Close()
	{
		OnClose?.Invoke();
		Delete();
	}

	/// <summary>
	/// Creates and adds a WarningPanel to the HUD.
	/// </summary>
	public static WarningPanel Create( string titleText, string warningText, List<Button>? buttons = null )
	{
		var warning = new WarningPanel( titleText, warningText, buttons );
		Hud.Instance?.AddElement( warning );
		return warning;
	}

	/// <summary>
	/// Initializes the buttons in the panel.
	/// </summary>
	private void InitializeButtons( List<Button> buttons )
	{
		Buttons = buttons;

		if ( Buttons.Count == 0 )
		{
			// Add a default "Close" button if no buttons are provided.
			var closeButton = new Button( "Close", "", Close );
			_buttonContainer.AddChild( closeButton );
		}
		else
		{
			AddButtonsToContainer( Buttons );
		}
	}

	/// <summary>
	/// Adds a list of buttons to the button container.
	/// </summary>
	private void AddButtonsToContainer( List<Button> buttons )
	{
		foreach ( var button in buttons )
		{
			_buttonContainer.AddChild( button );
		}
	}
}
