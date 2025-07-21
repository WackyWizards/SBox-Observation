using System;
using Observation.UI;
using Sandbox.UI;
using kEllie.Utils;

namespace Observation;

public sealed class Cursor : Singleton<Cursor>
{
	/// <summary>
	/// Duration required to hold the mouse button for interaction.
	/// </summary>
	[Property] public float HoldTime { get; set; } = 2f;

	private CameraComponent Camera => Scene.Camera;

	private TimeUntil _reportTimer;
	private bool _isHolding;

	protected override void OnStart()
	{
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
		Mouse.Visibility = MouseVisibility.Hidden;
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

		anomalyList.AvailableTypes = Enum.GetValues<Anomaly.AnomalyType>()
			.Where( AnomalyList.IsVisibleType )
			.ToList();

		anomalyList.Show();
		hud.GetFirstElement<RoomList>()?.Hide();
		anomalyList.OnReport += ReportCallback;
		return;

		// Define which anomaly types use room-based reporting
		static bool UsesRoomBasedReporting( Anomaly.AnomalyType type )
		{
			return type.GetAttributeOfType<RoomReportAttribute>() is not null;
		}

		void ReportCallback( Anomaly.AnomalyType type )
		{
			var selectedRoom = GetRoom( type );

			if ( UsesRoomBasedReporting( type ) )
			{
				RoomSelection();
				return;
			}

			CompleteReport( type, selectedRoom );
		}

		string GetRoom( Anomaly.AnomalyType type )
		{
			if ( !UsesRoomBasedReporting( type ) )
			{
				return CameraManager.Instance?.ActiveCamera?.Room ?? "N/A";
			}

			return "N/A";
		}

		void RoomSelection()
		{
			var roomList = Hud.GetElement<RoomList>();
			if ( !roomList.IsValid() )
				return;

			roomList.Show();
			roomList.OnReport += HandleRoomSelection;
		}

		void HandleRoomSelection( string room, Anomaly.AnomalyType anomalyType )
		{
			var roomList = Hud.GetElement<RoomList>();
			if ( roomList.IsValid() )
			{
				roomList.OnReport -= HandleRoomSelection;
			}

			CompleteReport( anomalyType, room );
		}

		void CompleteReport( Anomaly.AnomalyType type, string room )
		{
			ShowReportConfirmation( type, room );
			anomalyList.OnReport -= ReportCallback;
		}

		void ShowReportConfirmation( Anomaly.AnomalyType type, string room )
		{
			var reportConfirmation = hud.GetFirstElement<ReportConfirm>();
			if ( !reportConfirmation.IsValid() )
				return;

			reportConfirmation.Show( type, room, () =>
			{
				if ( UsesRoomBasedReporting( type ) )
				{
					AnomalyManager.Instance?.Report( type, room );
				}
				else
				{
					AnomalyManager.Instance?.Report( type, targetObject );
				}

				Log.Info( $"Reporting {type} in room: {room}" );
			} );
		}
	}

	public GameObject? RunRay()
	{
		if ( !Camera.IsValid() )
			return null;

		var trace = RunSphereRay();
		return trace?.GameObject;
	}

	private SceneTraceResult? RunSphereRay()
	{
		if ( !Camera.IsValid() )
			return null;

		var ray = Camera.ScreenPixelToRay( Mouse.Position );
		var worldPhysics = Game.ActiveScene.GetAllObjects( true ).FirstOrDefault( x => x.Name == "World Physics" );
		var trace = Scene.Trace.Sphere( 20, ray, 100000 )
			.IgnoreGameObject( worldPhysics )
			.WithTag( "anomaly" )
			.Run();

		return trace;
	}
}
