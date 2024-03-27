using Sandbox;
using Sandbox.Citizen;
using System;
using System.Text.Json.Nodes;
using System.Threading;

public sealed class Playercontroller : Component
{
	[Sync] public Angles angles { get; set; } = new Angles();
	public Angles angles_object = new Angles();
	[Property] public GameObject aim_show;
	[Property] public GameObject point;
	[Property] public GameObject chat;
	[Property] public GameObject modelself;
	[Property] public GameObject Eye;
	[Property] public GameObject showavatar;
	[Property] public CitizenAnimationHelper citizenAnimationHelper;
	[Property] public CharacterController characterController;
	[Property] public LocalCloth localcloth;
	[Property] public ModelCollider modelCollider;
	public Model model = Model.Cube;
	public Color color = Color.Green;
	public Vector3 scale = Vector3.One;
	public GameObject gun = null;
	public Boolean isMe;
	public Model GunModel = Cloud.Model( "https://asset.party/jakx/pistoldeusex" );
	public float Health = 100;
	string SteamName = "No Name";
	bool netInit;
	public bool CanDrag;
	CameraComponent Camera;
	public Vector3 lastobjectPos;
	public Vector3 offsetobject;
	public GameObject moveObject = null;
	public PhysicsBody moveBody = null;
	public GameObject lastObject = null;
	bool isChange = false;
	public float distObject;
	BBox hull = new BBox(
		new Vector3( -10 ),
		new Vector3( 10 )
	);
	int ind = 0;
	bool cnasd = true;
	public List<SceneParticles> particles = new List<SceneParticles>();
	string[] Tools = ["PhysGun", "Spawner", "Gun", "Scale", "GravGun", "Thruster", "Remove", "Color", "Balloon", "Light", "Display", "Info", "Save", "Rope", "Weld", "Lidar", "CreateTerry"];
	public Dictionary<GameObject, Color> DefaultColors = new();

	public static Playercontroller Local => GameManager.ActiveScene.Components.GetAll<Playercontroller>( FindMode.EnabledInSelfAndDescendants ).ToList().FirstOrDefault( x => x.Network.OwnerConnection.SteamId == (ulong)Game.SteamId );

	public Vector3 WishVelocity { get; private set; }
	[Sync] public bool IsNoclipping { get; set }
	public Vector3 NoclipVelocity;
	public float EyeHeight = 64;
	[Sync] public bool nenabled { get; set; } = true;
	public List<Color> cycleColors = new List<Color> { Color.Red, Color.Green, Color.Cyan, Color.Blue, Color.Yellow, Color.Magenta, Color.Orange, Color.Yellow };
	public Dictionary<GameObject, int> currentColorIndex = new Dictionary<GameObject, int>();
	public Vector3 lastObjectOffset;

	public Rotation EyeRotatation {
		get => (Network.IsOwner ? angles.ToRotation() : Eye.Transform.Rotation);
	}


	protected override void OnAwake()
	{
		base.OnAwake();
		// Transform.Scale = 0.5f;
		Camera = Scene.Camera.Components.Get<CameraComponent>();
		Camera.FieldOfView = 80f;
		gun = new GameObject();
		gun.Transform.Position = Vector3.Down * 100000f;
		gun.Components.Create<ModelRenderer>();
		gun.Components.GetInChildrenOrSelf<ModelRenderer>().Model = GunModel;
		// chat.Components.GetInChildrenOrSelf<TextRenderer>().Text

	}

	public void OnDeath()
	{
		if ( Health > 0 )
			return;
		Health = 100;
		Transform.Position = Vector3.Zero;
	}
	public SceneTraceResult Trace(Vector3 start, Vector3 end)
	{
		return Scene.Trace.Ray(start, end )
			.UsePhysicsWorld()
			.Radius(2f)
			.IgnoreGameObject(modelself)
			.WithoutTags([ "ragdoll" ])
			.Run();
	}

	public void BuildWishVelocity()
	{
		var rot = Eye.Transform.Rotation;

		WishVelocity = rot * Input.AnalogMove;
		WishVelocity = WishVelocity.WithZ( 0 );

		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( Input.Down( "Run" ) ) WishVelocity *= 320.0f;
		else WishVelocity *= 110.0f;
	}

	public Vector3 EyePosition()
	{
		return Transform.Position + Vector3.Up * EyeHeight;
	}

	private async void CloudLoadModel( string model )
	{
		var packageg = await Package.FetchAsync( model, false );
		await packageg.MountAsync();
		_ = Model.Load( packageg.GetMeta( "PrimaryAsset", "" ) );
	}

	[Broadcast]
	public void ModelLoad(string model, bool isassetparty)
	{
		Log.Info( model );
		if (isassetparty)
		{
			CloudLoadModel( model );
		}
		else
		{
			_ = Model.Load( model );
		}
	}

	protected override void OnUpdate()
	{
		if (!nenabled)
			return;
		if (isMe) 
			BuildWishVelocity();
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
		ModelRenderer.ShadowRenderType rendertype = (Network.IsOwner ? ModelRenderer.ShadowRenderType.ShadowsOnly : ModelRenderer.ShadowRenderType.On);
		modelself.Components.GetInChildrenOrSelf<ModelRenderer>().RenderType = rendertype;
		foreach ( var child in modelself.Children )
		{
			child.Components.GetInChildrenOrSelf<ModelRenderer>().RenderType = rendertype;
		}
		if ( Network.IsOwner )
		{
			showavatar.Enabled = false;
		}
		for ( int i = 0; i < (int)Tools.Count(); i++ )
		{
			if ( Input.Pressed( $"Slot{i + 1}" ) && isMe )
			{
				ind = i;
			}
		}
		if ( Input.Pressed( "Drop" ) && isMe )
		{
			ind++;
			ind = ind % Tools.Count();
		}
		if ( Input.Pressed( "Voice" ) && isMe )
		{
			IsNoclipping = !IsNoclipping;
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

		if ( chat != null && chat.Components != null && !isMe )
		{
			var textRenderer = chat.Components.GetInChildrenOrSelf<TextRenderer>();
			if ( textRenderer != null )
			{
				textRenderer.Text = SteamName + "\nHealth: " + Health.ToString();
			}
			// Network.OwnerConnection.SteamId
		}
		if ( isMe )
		{
			var textRenderer = chat.Components.GetInChildrenOrSelf<TextRenderer>();
			if ( textRenderer != null )
			{
				textRenderer.Text = "";
			}
		}
		/*
		if ( cnasd )
		{
			try
			{
				chat.Transform.Rotation = Scene.Camera.Components.Get<CameraComponent>().Transform.Rotation;
			}
			catch
			{
				cnasd = false;
				Log.Info( "WHAT, IS ERROR?" );
			}
		}
		*/
		if ( isMe && ( !Input.Down( "Use" ) || Tools[ind] != "PhysGun" || !Input.Down( "attack1" )) )
		{
			angles += Input.AnalogLook * 0.5f;
			Angles angle = angles;
			angle.pitch = angle.pitch.Clamp( -90f, 90f );
			angles = angle;
		}
		Eye.Transform.Rotation = Rotation.Lerp( Eye.Transform.Rotation, angles.ToRotation(), Time.Delta * 16f );

		SceneTraceResult aim = Trace( EyePosition(), EyePosition() + EyeRotatation.Forward * 5000f );


		if ( isMe && Input.Pressed( "Use" ) && !( Input.Down( "attack1" ) && Tools[ind] == "PhysGun" ) && aim.Hit && aim.Distance < 300f && aim.GameObject != null )
		{
			Log.Info( "Use" );
			foreach ( var damageable in aim.GameObject.Components.GetAll<IUsable>() )
			{
				aim.GameObject.Components.GetInChildrenOrSelf<IUsable>().OnUse( this );
			}
		}

		if ( !isMe )
		{	
			aim_show.Transform.Position = aim.HitPosition * 5000f;
			point.Transform.Position = EyePosition() + Transform.Rotation.Forward * 7f;
		}
		if ( Tools[ind] == "PhysGun" )
		{
			PhysGunTool.PhysGun( aim, this );
		}
		if ( Tools[ind] == "Spawner" )
		{
			SpawnTool.Spawn( aim, this );
		}
		if ( Tools[ind] == "Scale" )
		{
			ScaleTool.Scale( aim, this );
		}
		if ( Tools[ind] == "GravGun" )
		{
			GravGunTool.GravGun( aim, this );
		}	
		if ( Tools[ind] == "Remove" )
		{
			RemoveTool.Remove( aim, this );
		}
		if ( Tools[ind] == "Color" )
		{
			ColorTool.Color( aim, this );
		}
		if ( Tools[ind] == "Thruster" )
		{
			ThrusterTool.Thruster( aim, this );
		}
		if ( Tools[ind] == "CreateTerry")
		{
			CreateTerryTool.CreateTerry( aim, this );
		}
		if ( Tools[ind] == "Gun" )
		{
			GunTool.Gun( aim, this );
		}
		else
		{
			if (gun != null)
			{
				gun.Transform.Position = Vector3.Down * 10000f;
			}
		}
		if ( Tools[ind] == "Balloon" )
		{
			BallonTool.Balloon( aim, this );
		}
		if ( Tools[ind] == "Light" )
		{
			LightTool.Light( aim, this );
		}
		if ( Tools[ind] == "Info" )
		{
			InfoTool.Info( aim, this );
		}
		/*
		if ( Tools[ind] == "Thruster" )
		{
			Thruster( aim );
		}
		*/
		if ( Tools[ind] == "Save" )
		{
			SaveTool.Save( aim, this );
		}
		if ( Tools[ind] == "Rope" )
		{
			RopeTool.Rope( aim, this );
		}
		if ( Tools[ind] == "Weld" )
		{
			WeldTool.Weld( aim, this );
		}
		if ( Tools[ind] == "Lidar" )
		{
			LidarTool.Lidar( aim, this );
		}
		if ( isMe )
		{
			if ( Tools[ind] == "Display" )
			{
				if ( Input.Down( "attack2" ) )
				{
					Game.ActiveScene.Camera.FieldOfView += 1.5f * Input.AnalogLook.pitch;
					Angles angl = Game.ActiveScene.Camera.Transform.Rotation.Angles();
					angl.roll += 1.5f * Input.AnalogLook.yaw;
					Game.ActiveScene.Camera.Transform.Rotation = angl.ToRotation();
				}
			}
			else
			{
				if ( isMe && aim.Hit && ((Tools[ind] == "PhysGun" && !Input.Down( "attack1" )) || Tools[ind] != "PhysGun") )
				{
					Gizmo.Draw.Color = Color.Cyan;
					Gizmo.Draw.SolidSphere( aim.HitPosition, 5f, 50 );
				}
				Game.ActiveScene.Camera.FieldOfView = 80f;
				Angles angl = Game.ActiveScene.Camera.Transform.Rotation.Angles();
				angl.roll = 0;
				Game.ActiveScene.Camera.Transform.Rotation = angl.ToRotation();
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
		Vector3 forward = Eye.Transform.Rotation.Forward;
		forward = forward.WithZ( 0 );
		// forward = forward.WithX( 0 );
		Rotation rotation = Rotation.LookAt( forward );

		float rotateDifference = Transform.Rotation.Distance( Rotation.LookAt( forward ) );
		Vector3 Velocity = characterController.Velocity * 2f;
		if ( rotateDifference > 20f || WishVelocity.Length > 0.5 )
		{
			Transform.Rotation = Rotation.Lerp( Transform.Rotation, Rotation.LookAt( forward ), Time.Delta * 2f );
		}
		citizenAnimationHelper.WithLook( Eye.Transform.Rotation.Forward );
		citizenAnimationHelper.WithVelocity( Velocity );
		citizenAnimationHelper.WithWishVelocity( WishVelocity * 2f );
		citizenAnimationHelper.IsGrounded = characterController.TraceDirection( Vector3.Down ).Hit;
		citizenAnimationHelper.FootShuffle = rotateDifference;
		citizenAnimationHelper.IsNoclipping = IsNoclipping;
		if ( isMe )
		{
			if ( IsNoclipping )
			{
				Vector3 MoveVector = Vector3.Zero;
				if ( move.x > 0 )
				{
					MoveVector += Eye.Transform.Rotation.Forward;
				}
				if ( move.x < 0 )
				{
					MoveVector += Eye.Transform.Rotation.Backward;
				}
				if ( move.y > 0 )
				{
					MoveVector += Eye.Transform.Rotation.Left;
				}
				if ( move.y < 0 )
				{
					MoveVector += Eye.Transform.Rotation.Right;
				}
				NoclipVelocity += MoveVector * (Input.Down( "Run" ) ? 2.5f : 0.5f);
			}
			else
			{
				if ( Input.Pressed( "Jump" ) && characterController.TraceDirection( Vector3.Down ).Hit )
				{
					citizenAnimationHelper.TriggerJump();
					characterController.Punch( Vector3.Up * 700 );
				}
				characterController.Velocity += Vector3.Down * 2000f * Time.Delta;
				characterController.Accelerate( WishVelocity );
				characterController.ApplyFriction( 4.0f );
				characterController.Move();
			}
			// */
			Camera.Transform.Position = EyePosition();
			float roll = Camera.Transform.Rotation.Angles().roll;
			Angles angles1 = angles;
			angles1.roll = roll;
			Camera.Transform.Rotation = angles1.ToRotation();
			if ( IsNoclipping )
			{
				NoclipVelocity *= 0.95f;
				Transform.Position += NoclipVelocity;
			}
			if (Input.Released("Voice"))
			{
				characterController.Velocity += NoclipVelocity;
			}
		}
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	[Broadcast]
	public void Damage(float damage)
	{
		Health -= damage;
	}
}
