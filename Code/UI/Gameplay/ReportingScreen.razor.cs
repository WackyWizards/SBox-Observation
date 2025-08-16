using Sandbox.UI;

namespace Observation.UI;

public partial class ReportingScreen
{
	public Label? Report { get; private set; }

	public void SetReport( bool validReport )
	{
		if ( Report.IsValid() )
		{
			Report.Text = validReport ? "#ui.report.clear" : "#ui.report.none";
		}
	}
}
