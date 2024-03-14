using Sandbox;
using System;
using System.Diagnostics;

public sealed class Playercontroller : Component
{
	Angles angles = new Angles();
	Angles angles_object = new Angles();
	[Property] public GameObject aim_show;
	[Property] public GameObject point;
	Model model = Model.Cube;
	bool CanDrag;
	CameraComponent Camera;
	Vector3 lastobjectPos;
	GameObject moveObject = null;
	float distObject;
	BBox hull = new BBox(
		new Vector3( -10 ),
		new Vector3( 10 )
	);
	protected override void OnAwake()
	{
		base.OnAwake();
		Transform.Scale = 0.5f;
		Camera = Scene.Camera.Components.Get<CameraComponent>();
	}
	private SceneTraceResult Trace(Vector3 start, Vector3 end)
	{
		return Scene.Trace.Ray(start, end )
			.UsePhysicsWorld()
			.Radius(2f)
			.Run();
	}
	protected override void OnUpdate()
	{
		Camera.FieldOfView = 80;
		if ( !Input.Down( "Use" ) )
		{
			angles += Input.AnalogLook * 0.5f;
			//angles.pitch = angles.pitch.Clamp( -60f, 80f );
			Transform.Rotation = Rotation.Lerp( Transform.Rotation, angles.ToRotation(), Time.Delta * 16f );
		}
		SceneTraceResult aim = Trace( Transform.Position, Transform.Position + Transform.Rotation.Forward * 5000f );
		aim_show.Transform.Position = aim.EndPosition;
		point.Transform.Position = Transform.Position + Transform.Rotation.Forward * 50f + Transform.Rotation.Right * 40f;
		float dist = MathF.Sqrt(
							MathF.Pow( aim.EndPosition.x - Transform.Position.x, 2 ) +
							MathF.Pow( aim.EndPosition.y - Transform.Position.y, 2 ) +
							MathF.Pow( aim.EndPosition.z - Transform.Position.z, 2 )
							);
		GameObject picker = aim.GameObject;
		if ( Input.Pressed( "attack2" ) && !Input.Down( "attack1" ) )
		{
			GameObject newobject = new GameObject( true, "spawned" );
			newobject.Transform.Position = aim.HitPosition;
			newobject.Components.Create<ModelRenderer>();
			newobject.Components.GetInChildrenOrSelf<ModelRenderer>().Model = model;
			newobject.Components.Create<BoxCollider>();
			//newobject.Components.GetInChildrenOrSelf<ModelCollider>().Model = model;
			newobject.Components.Create<Rigidbody>();
		}
		if ( Input.Pressed( "Reload" ) )
		{
			model = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Model;
		}
		// Log.Info(picker);
		if ( picker != null )
		{
			if ( Input.Down( "attack1" ) && CanDrag )
			{
				if ( moveObject == null )
				{
					if ( !picker.Components.GetInChildrenOrSelf<Collider>().Static )
					{
						moveObject = picker;
						distObject = dist;
						lastobjectPos = picker.Transform.Position;
						Rigidbody moveBody = moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
						if ( moveBody != null )
							moveBody.MotionEnabled = false;
					}
				}
				else
				{
					Rigidbody moveBody = moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
					moveObject.Transform.Position = Transform.Position + Transform.Rotation.Forward * distObject;
					if ( moveBody != null )
					{
						Vector3 velocity = moveBody.Velocity;
						Vector3.SmoothDamp( lastobjectPos, moveObject.Transform.Position, ref velocity, 0.075f, Time.Delta );
						moveBody.Velocity = velocity;
						if ( Input.Pressed( "attack2" ) )
						{
							CanDrag = false;
						}
					}
					if ( Input.Pressed( "Use" ) )
						angles_object = moveObject.Transform.Rotation.Angles();
					if ( Input.Down( "Use" ) )
					{
						// Log.Info( Input.AnalogLook.Forward );
						angles_object += Input.AnalogLook * 0.5f;
						Rotation rotation = moveObject.Transform.Rotation;
						moveObject.Transform.Rotation = Rotation.Lerp( rotation, angles_object.ToRotation(), Time.Delta * 16f );
					}	
					lastobjectPos = picker.Transform.Position;
				}
			}
			if ( !Input.Down( "attack1" ) )
			{
				if ( moveObject != null && CanDrag )
				{
					Rigidbody moveBody = moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
					if ( moveBody != null )
					{
						moveBody.MotionEnabled = true;
					}
				}
				CanDrag = true;
				moveObject = null;
			}
		}
		Vector3 move = Input.AnalogMove;
		if ( Input.Down( "Run" ) )
			move *= 5;
		else if ( Input.Down( "Duck" ) )
			move /= 5;

		// Log.Info( move );
		//Transform.Position += Transform.Rotation.ClosestAxis(move);
		// /*
		if ( move.x > 0 )
		{
			Transform.Position += Transform.Rotation.Forward * move.Length;
		}
		if ( move.x < 0 )
		{
			Transform.Position += Transform.Rotation.Backward * move.Length;
		}
		if ( move.y > 0 )
		{
			Transform.Position += Transform.Rotation.Left * move.Length;
		}
		if ( move.y < 0 )
		{
			Transform.Position += Transform.Rotation.Right * move.Length;
		}
		// */
		Camera.Transform.Position = Transform.Position;
		Camera.Transform.Rotation = Transform.Rotation;
	}
}
