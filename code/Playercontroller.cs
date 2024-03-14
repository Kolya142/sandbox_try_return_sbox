using Sandbox;
using System;
using System.Diagnostics;

public sealed class Playercontroller : Component
{
	Angles angles = new Angles();
	[Property] public GameObject aim_show;
	[Property] public GameObject point;
	GameObject moveObject = null;
	float distObject;
	BBox hull = new BBox(
		new Vector3( -1 ),
		new Vector3( 1 )
	);
	protected override void OnAwake()
	{
		base.OnAwake();
	}
	private SceneTraceResult Trace(Vector3 start, Vector3 end)
	{
		return Scene.Trace.Ray(end, start).Size(hull).Run();
	}
	protected override void OnUpdate()
	{
		angles += Input.AnalogLook * 0.5f;
		angles.pitch = angles.pitch.Clamp( -60f, 80f );
		Transform.Rotation = Rotation.Lerp( Transform.Rotation, angles.ToRotation(), Time.Delta * 16f );
		SceneTraceResult aim = Trace( Transform.Position, Transform.Position + Transform.Rotation.Forward * 5000f );
		aim_show.Transform.Position = aim.EndPosition;
		point.Transform.Position = Transform.Position + Transform.Rotation.Forward * 50f + Transform.Rotation.Right * 40f;
		float dist = MathF.Sqrt(
							MathF.Pow( aim.EndPosition.x - Transform.Position.x, 2 ) +
							MathF.Pow( aim.EndPosition.y - Transform.Position.y, 2 ) +
							MathF.Pow( aim.EndPosition.z - Transform.Position.z, 2 )
							);
		GameObject picker = aim.GameObject;
		if ( Input.Pressed( "attack2" ) )
		{
			GameObject newobject = new GameObject( true, "spawned" );
			newobject.Transform.Position = aim.HitPosition;
			newobject.Components.Create<ModelRenderer>();
			newobject.Components.GetInChildrenOrSelf<ModelRenderer>().Model = Model.Cube;
			newobject.Components.Create<BoxCollider>();
			// newobject.Components.Create<Rigidbody>();
		}
		if ( picker != null )
		{
			if ( Input.Down( "attack1" ) )
			{
				if ( moveObject == null )
				{
					if ( !picker.Components.GetInChildrenOrSelf<Collider>().Static )
					{
						moveObject = picker;
						distObject = dist;
						Rigidbody moveBody = moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
						if ( moveBody != null )
							moveBody.MotionEnabled = false;
					}
				}
				else
				{
					moveObject.Transform.Position = Transform.Position + Transform.Rotation.Forward * distObject;
				}
			}
			else
			{
				if (moveObject != null)
				{
					moveObject.Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = true;
				}
				moveObject = null;
			}
		}
		Vector3 move = Input.AnalogMove;
		if ( Input.Down( "Run" ) )
			move *= 2;
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
	}
}
