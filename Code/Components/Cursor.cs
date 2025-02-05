using Observation.UI;
using Sandbox.UI;

namespace Observation;

public class Cursor : Component
{
	public static Cursor? Instance { get; private set; }

	/// <summary>
	/// Duration required to hold the mouse button for interaction.
	/// </summary>
	[Property] public float HoldTime { get; set; } = 2f;

	private CameraComponent Camera => Scene.Camera;

	private TimeUntil _reportTimer;
	private bool _isHolding;

	protected override void OnStart()
	{
		Instance = this;
		_reportTimer = HoldTime;
		HideMouse();
		
		base.OnStart();
	}

	protected override void OnUpdate()
	{
		HandleMouseInteraction();
	}

	private void HandleMouseInteraction()
	{
		if ( Input.Down( "attack1" ) )
		{
			ProcessMouseHold();
		}
		else
		{
			_isHolding = false;
		}
	}

	private void ProcessMouseHold()
	{
		if ( !_isHolding )
		{
			_reportTimer = HoldTime;
			_isHolding = true;
		}

		if ( _reportTimer )
		{
			TriggerMouseHeldAction();
		}
	}

	private void TriggerMouseHeldAction()
	{
		var targetObject = RunRay();
		ShowAnomalyList( targetObject! );

		_isHolding = false;
	}

	public static void HideMouse()
	{
		Mouse.Visible = false;
		Mouse.CursorType = "none";
	}

	private static void ShowAnomalyList( GameObject targetObject )
	{
			
		var hud = Hud.Instance;
		if ( !hud.IsValid() )
			return;

		var anomalyList = hud.GetFirstElement<AnomalyList>();
		if ( !anomalyList.IsValid() )
			return;

		anomalyList.Show();
		anomalyList.OnReport += ReportCallback;
		return;

		void ReportCallback( Anomaly.AnomalyType type )
		{
			AnomalyManager.Instance?.Report( type, targetObject );
			anomalyList.OnReport -= ReportCallback;
		}
	}

	public GameObject? RunRay()
	{
		if ( !Camera.IsValid() )
			return null;

		var ray = Camera.ScreenPixelToRay( Mouse.Position );
		var trace = Scene.Trace.RunRayTrace( ray, 100000 );
		return trace.GameObject;
	}
}
