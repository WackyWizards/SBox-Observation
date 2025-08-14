using System;
using Sandbox.UI;

namespace Observation.UI;

public partial class Tutorial
{
	private Panel? _tabs;
	private int CurrentTabIndex { get; set; } = 0;
	private bool IsFirstTab => CurrentTabIndex == 0;
	private bool IsLastTab => CurrentTabIndex == _tabs?.ChildrenCount - 1;

	private void NextTab()
	{
		if ( IsLastTab || _tabs == null ) return;
		CurrentTabIndex++;
		StateHasChanged();
	}

	private void LastTab()
	{
		if ( IsFirstTab ) return;
		CurrentTabIndex--;
		StateHasChanged();
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime ) return;
		base.OnAfterTreeRender( firstTime );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( CurrentTabIndex );
	}
}
