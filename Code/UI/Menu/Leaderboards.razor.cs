using System;
using System.Threading.Tasks;

namespace Observation.UI;

public partial class Leaderboards
{
	private int _selectedTab;
	private int _selectedLeaderboardTab;

	private Sandbox.Services.Leaderboards.Board2? _sRankBoard;
	private Sandbox.Services.Leaderboards.Board2? _winsBoard;

	private void SelectTab( int index )
	{
		_selectedTab = index;
	}

	private async Task SelectLeaderboardTab( int index )
	{
		_selectedLeaderboardTab = index;
		await RefreshLeaderboard();
	}

	private async Task RefreshLeaderboard()
	{
		switch ( _selectedLeaderboardTab )
		{
			case 0:
				_sRankBoard = Sandbox.Services.Leaderboards.GetFromStat( Game.Ident, "Wins_with_rank_S" );
				_sRankBoard.MaxEntries = 50;
				_sRankBoard.CenterOnMe();
				await _sRankBoard.Refresh();
				break;
			case 1:
				_winsBoard = Sandbox.Services.Leaderboards.GetFromStat( Game.Ident, "Wins" );
				_winsBoard.MaxEntries = 50;
				_winsBoard.CenterOnMe();
				await _winsBoard.Refresh();
				break;
		}
	}

	private static string GetRank( int position )
	{
		return position switch
		{
			1 => "gold",
			2 => "silver",
			3 => "bronze",
			_ => string.Empty
		};
	}

	private static bool IsMe( SteamId id )
	{
		return id == Connection.Local.SteamId;
	}

	protected override async void OnAfterTreeRender( bool firstTime )
	{
		try
		{
			await RefreshLeaderboard();
			base.OnAfterTreeRender( firstTime );
		}
		catch
		{
			// Ignored.
		}
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( _winsBoard, _sRankBoard, _selectedLeaderboardTab );
	}
}
