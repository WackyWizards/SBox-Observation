using Sandbox.UI;

namespace Observation.UI;

public partial class Tip
{
	private List<string> PossibleTips { get; set; } =
	[
		"#tip.1",
		"#tip.2",
		"#tip.3",
		"#tip.4",
		"#tip.5",
		"#tip.6",
		"#tip.7"
	];

	private string ActiveTip { get; set; } = string.Empty;
	
	private TimeUntil _nextTip = 30;
	private TimeUntil _nextChar = 0.1f;
	private Label? _tipLabel;
	private string _displayText = string.Empty;
	private int _index = 0;

	public override void Tick()
	{
		if ( !_tipLabel.IsValid() )
		{
			return;
		}

		_tipLabel.Text = _displayText;

		if ( _nextTip && _index == _displayText.Length )
		{
			// Select random tip and get its localized phrase.
			var selectedTip = Game.Random.FromList( PossibleTips! ) ?? string.Empty;
			var tipKey = selectedTip.TrimStart( '#' );
			
			ActiveTip = Language.GetPhrase( tipKey );

			_displayText = string.Empty;
			_index = 0;
			_nextTip = 30;
		}

		if ( _index >= ActiveTip.Length || !_nextChar )
		{
			return;
		}

		_displayText += ActiveTip[_index++];
		_nextChar = 0.1f;

		base.Tick();
	}
}
