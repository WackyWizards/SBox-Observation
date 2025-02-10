using System;
using Sandbox.UI;

namespace Observation.UI;

public class MenuSubPanel : Panel
{
	public MainMenu? Menu { get; set; }
	
	public Action? OnReturn { get; set; }
	
	protected virtual void Return()
	{
		this.Hide();
		OnReturn?.Invoke();
	}
}
