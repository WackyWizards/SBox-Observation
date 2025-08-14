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
		return _isWin ? "Shift Complete" : "Notice Of Dismissal";
	}

	private static string GetSubTitle( bool victory, GameManager.LoseReason reason )
	{
		return victory ? "Congratulations!" : reason.GetDescription();
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
