using System;
using Sandbox.UI;

namespace Observation.UI;

public partial class GameOver
{
	private bool _isWin;
	private GameManager.LoseReason _loseReason = GameManager.LoseReason.TooManyAnomalies;
	
	private Label? _title;
	private Label? _subTitle;

	private string GetTitle()
	{
		return _isWin ? "#ui.gameover.win.title" : "#ui.gameover.lose.title";
	}

	private static string GetSubTitle( bool victory, GameManager.LoseReason reason )
	{
		return victory ? "#ui.gameover.congratulations" : reason.GetTitle();
	}

	public void OnGameEnd( bool victory )
	{
		_isWin = victory;
	}

	public void OnGameLose( GameManager.LoseReason reason )
	{
		_isWin = false;
		_loseReason = reason;
	}
	
	private static string GetFormattedText( Anomaly anomaly )
	{
		const string key = "ui.gameover.missedanomaly";
		var confirmationPhrase = Language.GetPhrase( key );

		// Get the localized anomaly type name
		var anomalyTypeTitle = anomaly.Type.GetLocalizedName();
		var typeDisplayName = anomalyTypeTitle.StartsWith( '#' )
			? Language.GetPhrase( anomalyTypeTitle[1..] )
			: anomalyTypeTitle;

		// Get the localized room name
		var roomName = anomaly.Room;
		var roomDisplayName = roomName.StartsWith( '#' )
			? Language.GetPhrase( roomName[1..] )
			: roomName;

		return string.Format( confirmationPhrase, typeDisplayName, roomDisplayName );
	}

	private static void PlayAgain()
	{
		MapManager.Instance?.Restart();
	}

	private static void Return()
	{
		GameManager.Instance?.ToMenu();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( _isWin, _loseReason, _title?.Text, _subTitle?.Text );
	}
}
