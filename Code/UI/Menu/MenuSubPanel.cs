using Sandbox.UI;

namespace Observation.UI;

public class MenuSubPanel : Panel
{
	public MainMenu? Menu { get; set; }
	
	protected virtual void Return()
	{
		this.Hide();
	}
}
