using Sandbox.UI;

namespace Observation.UI;

public partial class ReportingScreen
{
	public Label? Report;

	public void SetReport( bool validReport )
	{
		if ( Report.IsValid() )
			Report.Text = validReport ? "Anomaly Cleared!" : "No Anomalies Found!";
	}
}
