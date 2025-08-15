using System;
using Sandbox.UI;

namespace Observation.UI;

public class MenuSubPanel : Panel
{
	[Parameter]
	public MainMenu? Menu { get; set; }
	
	[Parameter]
	public Action? OnReturn { get; set; }
	
	protected virtual void Return()
	{
		this.Hide();
		OnReturn?.Invoke();
	}
}
