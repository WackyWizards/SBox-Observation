using System;
using System.Threading.Tasks;

namespace Observation.UI;

public partial class MapInfo
{
	public Map? Map { get; set; }
	
	private int _selectedTab = 0;
	private int _selectedLeaderboardTab = 0;
	
	private Sandbox.Services.Leaderboards.Board2? _sRankBoard;
	private Sandbox.Services.Leaderboards.Board2? _completionsBoard;

	private void SelectTab( int index )
	{
		_selectedTab = index;
		if ( index == 1 )
		{
			_ = RefreshCurrentLeaderboard();
		}
	}

	private void SelectLeaderboardTab( int index )
	{
		_selectedLeaderboardTab = index;
		_ = RefreshCurrentLeaderboard();
	}

	private void SelectDifficulty( Difficulty difficulty )
	{
		if ( Map is null )
		{
			return;
		}

		Map.Difficulty = difficulty;
	}

	private async Task RefreshCurrentLeaderboard()
	{
		if ( Map is null ) return;

		switch ( _selectedLeaderboardTab )
		{
			case 0:
				_sRankBoard =
					Sandbox.Services.Leaderboards.GetFromStat( Game.Ident, $"Wins_on_map_{Map.Ident}_with_rank_S" );
				_sRankBoard.CenterOnMe();
				await _sRankBoard.Refresh();
				break;
			case 1:
				_completionsBoard = Sandbox.Services.Leaderboards.GetFromStat( Game.Ident, $"Wins_on_map_{Map.Ident}" );
				_completionsBoard.CenterOnMe();
				await _completionsBoard.Refresh();
				break;
		}
	}

	private void Play()
	{
		if ( Map is null )
		{
			Log.Error( "Unable to play a null map!" );
			return;
		}

		if ( Menu.IsValid() )
		{
			if ( Menu.MenuMusic.IsValid() )
			{
				Menu.MenuMusic.Stop();
			}
		}

		MapManager.Instance?.SetMap( Map );
		var scene = Map.Scene;
		var loadOptions = new SceneLoadOptions();
		loadOptions.SetScene( scene );
		Game.ChangeScene( loadOptions );
	}

	protected override void Return()
	{
		Delete();
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( _selectedTab == 1 )
		{
			_ = RefreshCurrentLeaderboard();
		}

		base.OnAfterTreeRender( firstTime );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Map, _selectedTab, _selectedLeaderboardTab, _completionsBoard, _sRankBoard );
	}
}
