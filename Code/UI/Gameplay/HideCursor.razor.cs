using Sandbox.UI;

namespace Observation.UI;

public partial class HideCursor
{
	public async void HideMouse()
	{
		try
		{
			this.Show();
			await Task.DelayRealtime( 100 );
			this.Hide();
		}
		catch
		{
			// Ignored.
		}
	}
}
