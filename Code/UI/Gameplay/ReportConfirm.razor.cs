using System;
using Sandbox.UI;

namespace Observation.UI;

public partial class ReportConfirm
{
	private Label? Title { get; set; }
	private string RoomName { get; set; } = "N/A";
	private Anomaly.AnomalyType AnomalyType { get; set; }
	private Action? OnConfirm { get; set; }
	private Action? OnDeny { get; set; }
	
	private Label? _text;
	private string? _localizedText;

	public void Show( Anomaly.AnomalyType type, string room, Action? onConfirm = null, Action? onDeny = null )
	{
		AnomalyType = type;
		RoomName = room;
		_localizedText = GetFormattedText();

		if ( onConfirm is not null )
		{
			OnConfirm = onConfirm;
		}

		if ( onDeny is not null )
		{
			OnDeny = onDeny;
		}

		this.Show();
	}

	private void Confirm()
	{
		OnConfirm?.Invoke();
		this.Hide();
	}

	private void Deny()
	{
		OnDeny?.Invoke();
		this.Hide();
	}

	private string GetFormattedText()
	{
		const string key = "ui.report.confirm.text";
		var confirmationPhrase = Language.GetPhrase( key );

		// Get the localized anomaly type name
		var anomalyTypeTitle = AnomalyType.GetTitle();
		var typeDisplayName = "#".StartsWith( anomalyTypeTitle )
			? Language.GetPhrase( anomalyTypeTitle[1..] )
			: anomalyTypeTitle;

		// Get the localized room name
		var roomDisplayName = "#".StartsWith( RoomName )
			? Language.GetPhrase( RoomName[1..] )
			: RoomName;

		return string.Format( confirmationPhrase, typeDisplayName, roomDisplayName );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Title, Title?.Text, AnomalyType, _text, _text?.Text, _localizedText );
	}
}
