using System;
using Sandbox.UI;

namespace Observation.UI;

public partial class Tutorial
{
	private int CurrentTabIndex { get; set; } = 0;
	private bool IsFirstTab => CurrentTabIndex == 0;
	private bool IsLastTab => CurrentTabIndex == _tabs?.ChildrenCount - 1;

	private Panel? _tabs;
	
	private void NextTab()
	{
		if ( IsLastTab || _tabs is null )
		{
			return;
		}
		
		CurrentTabIndex++;
		StateHasChanged();
	}

	private void LastTab()
	{
		if ( IsFirstTab )
		{
			return;
		}
		
		CurrentTabIndex--;
		StateHasChanged();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( CurrentTabIndex );
	}
}
