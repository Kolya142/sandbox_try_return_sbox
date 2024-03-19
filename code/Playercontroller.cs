using Sandbox;
using Sandbox.Services;
using Sandbox.Utility;
using System;
using System.Diagnostics;

public sealed class Playercontroller : Component
{
	Angles angles = new Angles();
	Angles angles_object = new Angles();
	[Property] public GameObject aim_show;
	[Property] public GameObject point;
	[Property] public GameObject chat;
	Model model = Model.Cube;
	Color color = Color.Green;
	Vector3 scale = Vector3.One;
	Boolean isMe;
	string SteamName = "No Name";
	bool netInit;
	bool CanDrag;
	CameraComponent Camera;
	Vector3 lastobjectPos;
	Vector3 offsetobject;
	GameObject moveObject = null;
	float distObject;
	BBox hull = new BBox(
		new Vector3( -10 ),
		new Vector3( 10 )
	);
	int ind = 0;
	List<SceneParticles> particles = new List<SceneParticles>();
	string[] Tools = ["PhysGun", "Scale", "GravGun", "Remove", "Color"];
	Dictionary<GameObject, Color> DefaultColors = new();
	private List<Color> cycleColors = new List<Color> { Color.Red, Color.Green, Color.Cyan, Color.Blue, Color.Yellow, Color.Magenta, Color.Orange, Color.Yellow };
	private Dictionary<GameObject, int> currentColorIndex = new Dictionary<GameObject, int>();


	protected override void OnAwake()
	{
		base.OnAwake();
		Transform.Scale = 0.5f;
		Camera = Scene.Camera.Components.Get<CameraComponent>();
		Camera.FieldOfView = 80;
		// chat.Components.GetInChildrenOrSelf<TextRenderer>().Text


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
		// Log.Info( particles );
		foreach ( var particle in particles )
		{
			particle.Simulate( Time.Delta );
			if ( particle.Finished )
			{
				particles.Remove( particle );
				particle.Delete();
			}
		}
		if ( Input.Pressed( "Drop" ) )
		{
			ind++;
			ind = ind % Tools.Count();
		}
		if ( Network.Active && !netInit )
		{
			netInit = true;
			isMe = Network.IsOwner; // Assuming isMe has been declared elsewhere
			if ( Network.OwnerConnection != null )
			{
				SteamName = Network.OwnerConnection.Name; // Assuming SteamName has been declared elsewhere
			}
		}

		if ( chat != null && chat.Components != null )
		{
			var textRenderer = chat.Components.GetInChildrenOrSelf<TextRenderer>();
			if ( textRenderer != null )
			{
				textRenderer.Text = SteamName;
			}
		}
		// chat.Transform.Rotation = Camera.Transform.Rotation; TODO fix this
		if ( isMe && ( !Input.Down( "Use" ) || Tools[ind] != "PhysGun") )
		{
			angles += Input.AnalogLook * 0.5f;
			angles.pitch = angles.pitch.Clamp( -60f, 90f );
			Transform.Rotation = Rotation.Lerp( Transform.Rotation, angles.ToRotation(), Time.Delta * 16f );
		}
		SceneTraceResult aim = Trace( Transform.Position, Transform.Position + Transform.Rotation.Forward * 5000f );
		aim_show.Transform.Position = aim.EndPosition;
		if ( isMe ) 
			point.Transform.Position = Transform.Position + Transform.Rotation.Forward * 50f + Transform.Rotation.Right * 40f;
		else
			point.Transform.Position = Transform.Position;
		if ( Tools[ind] == "PhysGun" )
		{
			PhysGun( aim );
		}
		if ( Tools[ind] == "Scale" )
		{
			Scale( aim );
		}
		if ( Tools[ind] == "GravGun" )
		{
			GravGun( aim );
		}	
		if ( Tools[ind] == "Remove" )
		{
			Remove( aim );
		}
		if ( Tools[ind] == "Color" )
		{
			ColorTool( aim );
		}
		Vector3 move = Input.AnalogMove;
		if ( Input.Down( "Run" ) )
			move *= 5;
		else if ( Input.Down( "Duck" ) )
			move /= 5;

		// Log.Info( move );
		//Transform.Position += Transform.Rotation.ClosestAxis(move);
		// /*
		if ( isMe )
		{
			Vector3 MoveVector = Vector3.Zero;
			if ( move.x > 0 )
			{
				MoveVector += Transform.Rotation.Forward;
			}
			if ( move.x < 0 )
			{
				MoveVector += Transform.Rotation.Backward;
			}
			if ( move.y > 0 )
			{
				MoveVector += Transform.Rotation.Left;
			}
			if ( move.y < 0 )
			{
				MoveVector += Transform.Rotation.Right;
			}
			// MoveVector = MoveVector.Normal;
			Transform.Position += MoveVector * move.Length;
			// */
			Camera.Transform.Position = Transform.Position;
			Camera.Transform.Rotation = Transform.Rotation;

		}
	}

	private void ColorTool( SceneTraceResult aim )
	{
		GameObject picker = aim.GameObject;
		if ( picker != null && isMe && !picker.Components.GetInChildrenOrSelf<Collider>().Static )
		{
			if ( Input.Pressed( "attack1" ) )
			{
				if ( !DefaultColors.ContainsKey( picker ) )
				{
					DefaultColors.Add( picker, picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint );
				}
				if ( currentColorIndex.ContainsKey( picker ) )
				{
					currentColorIndex[picker]++;
					currentColorIndex[picker] = currentColorIndex[picker] % cycleColors.Count;
				}
				else
				{
					currentColorIndex[picker] = 0;
				}
				picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint = cycleColors[currentColorIndex[picker]];
			}
			else if ( Input.Pressed( "attack2" ) && DefaultColors.ContainsKey( picker ) )
			{
				currentColorIndex[picker] = 0;
				picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint = DefaultColors[picker];
			}
		}
	}

	private void Remove( SceneTraceResult aim )
	{
		GameObject picker = aim.GameObject;
		if ( picker != null && isMe && !picker.Components.GetInChildrenOrSelf<Collider>().Static )
		{
			if ( Input.Pressed( "attack1" ) )
			{
				picker.Destroy();
			}
		} 
	}

	private void GravGun( SceneTraceResult aim )
	{
		GameObject picker = aim.GameObject;
		if ( picker != null && isMe && !picker.Components.GetInChildrenOrSelf<Collider>().Static && aim.Distance < 500f ) {
			if ( Input.Pressed( "attack2" ) )
			{
				if ( moveObject == null )
				{
					moveObject = picker;
					if ( moveObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
					{
						moveObject.Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = false;
					}
				}
				else
				{
					if ( moveObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
					{
						moveObject.Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = true;
					}
					moveObject = null;
				}
			} 
			else if ( Input.Pressed( "attack1" ) )
			{
				if ( moveObject == null )
					moveObject = picker;
				if ( moveObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
				{
					moveObject.Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = true;
					moveObject.Components.GetInChildrenOrSelf<Rigidbody>().Velocity = Transform.Rotation.Forward * 1000f;
				}
				moveObject = null;
			}
		}
		if ( moveObject != null )
		{
			moveObject.Transform.Position = Transform.Position + Transform.Rotation.Forward * 150f;
			moveObject.Transform.Rotation = Rotation.Identity;
			if ( moveObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
			{
				moveObject.Components.GetInChildrenOrSelf<Rigidbody>().Velocity = Vector3.Zero;
			}
		}
	}

	private void Scale( SceneTraceResult aim )
	{
		GameObject picker = aim.GameObject;
		if ( picker != null && isMe )
		{
			picker.Transform.Scale *= new Vector3( Input.MouseWheel.y * 0.1f + 1f );
		}
	}

	private void PhysGun(SceneTraceResult aim)
	{
		GameObject picker = aim.GameObject;
		if ( Input.Pressed( "attack2" ) && !Input.Down( "attack1" ) && isMe )
		{
			var particle = new SceneParticles( Scene.SceneWorld, "particles/createeffect.vpcf" );
			particle.SetControlPoint( 0, aim.HitPosition );
			particle.SetControlPoint( 0, Rotation.Identity );
			// particles.Add( particle );
			Sound.PlayFile( SoundFile.Load( "Sounds/balloon_pop_cute.sound" ) );
			Log.Info( "got \"entitiescount\" stat" );
			Sandbox.Services.Stats.Increment( "entitiescount", 1 );
			GameObject newobject = new GameObject( true, "spawned" );
			newobject.Transform.Position = aim.HitPosition;
			newobject.Transform.Scale = scale;
			newobject.Components.Create<ModelRenderer>();
			newobject.Components.GetInChildrenOrSelf<ModelRenderer>().Model = model;
			if ( model == Model.Cube )
			{
				newobject.Components.Create<BoxCollider>();
			}
			else if ( model == Model.Sphere )
			{
				newobject.Components.Create<SphereCollider>();
				newobject.Components.GetInChildrenOrSelf<SphereCollider>().Radius *= 2;
			}
			else if ( model.Name == "models/dev/plane.vmdl" )
			{
				newobject.Components.Create<BoxCollider>();
				newobject.Components.GetInChildrenOrSelf<BoxCollider>().Scale = new Vector3( 100, 100, 3 );
			}
			else
			{
				newobject.Components.Create<ModelCollider>();
				newobject.Components.GetInChildrenOrSelf<ModelCollider>().Model = model;
			}
			newobject.Components.GetInChildrenOrSelf<ModelRenderer>().Tint = color;
			Log.Info( model.Name );
			newobject.Components.Create<Rigidbody>();
		}
		if ( Input.Pressed( "Reload" ) && isMe )
		{
			model = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Model;
			color = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint;
			scale = picker.Transform.Scale;
		}
		// Log.Info(picker);
		if ( picker != null && isMe )
		{

			float dist = MathF.Sqrt(
								MathF.Pow( aim.HitPosition.x - Transform.Position.x, 2 ) +
								MathF.Pow( aim.HitPosition.y - Transform.Position.y, 2 ) +
								MathF.Pow( aim.HitPosition.z - Transform.Position.z, 2 )
								);
			if ( Input.Down( "attack1" ) && CanDrag )
			{
				if ( moveObject == null )
				{
					if ( !picker.Components.GetInChildrenOrSelf<Collider>().Static )
					{
						moveObject = picker;
						distObject = dist;
						lastobjectPos = picker.Transform.Position;
						offsetobject = aim.HitPosition - picker.Transform.Position;
						Rigidbody moveBody = moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
						if ( moveBody != null )
							moveBody.MotionEnabled = false;
					}
				}
				else
				{
					distObject += Input.MouseWheel.y * 6f * (Input.Down( "Run" ) ? 2f : 1f);
					Rigidbody moveBody = moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
					moveObject.Transform.Position = Transform.Position + Transform.Rotation.Forward * distObject - offsetobject;
					if ( moveBody != null )
					{
						Vector3 velocity = moveBody.Velocity;
						Vector3.SmoothDamp( lastobjectPos, moveObject.Transform.Position, ref velocity, 0.075f, Time.Delta );
						moveBody.Velocity = velocity;
						if ( Input.Pressed( "attack2" ) )
						{
							moveBody.Velocity = Vector3.Zero;
							moveBody.AngularVelocity = Vector3.Zero;
							moveBody.AngularDamping = 0;
							CanDrag = false;
							var particle = new SceneParticles( Scene.SceneWorld, "particles/createeffect.vpcf" );
							particle.SetControlPoint( 0, aim.HitPosition );
							particle.SetControlPoint( 0, Rotation.Identity );
							particles.Add( particle );
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
			if ( !Input.Down( "attack1" ) && isMe )
			{
				if ( moveObject != null && CanDrag )
				{
					Rigidbody moveBody = moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
					if ( moveBody != null )
					{
						moveBody.MotionEnabled = true;
						moveBody.PhysicsBody.Enabled = true;
						//moveBody.Enabled = true;
					}
					if ( moveBody.Velocity.Length > 1000f )
					{
						Log.Info( "got \"velua\" stat" );
						Sandbox.Services.Stats.Increment( "velya", 1 );
					}
				}
				CanDrag = true;
				moveObject = null;
			}
		}
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
