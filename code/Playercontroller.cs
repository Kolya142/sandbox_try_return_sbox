using System;
using System.Text.Json.Nodes;

public sealed class Playercontroller : Component
{
	Angles angles = new Angles();
	public Angles angles_object = new Angles();
	[Property] public GameObject aim_show;
	[Property] public GameObject point;
	[Property] public GameObject chat;
	public Model model = Model.Cube;
	public Color color = Color.Green;
	public Vector3 scale = Vector3.One;
	public Boolean isMe;
	string SteamName = "No Name";
	bool netInit;
	public bool CanDrag;
	CameraComponent Camera;
	public Vector3 lastobjectPos;
	public Vector3 offsetobject;
	public GameObject moveObject = null;
	public GameObject lastObject = null;
	bool isChange = false;
	public float distObject;
	BBox hull = new BBox(
		new Vector3( -10 ),
		new Vector3( 10 )
	);
	int ind = 0;
	public List<SceneParticles> particles = new List<SceneParticles>();
	string[] Tools = ["PhysGun", "Scale", "GravGun", "Remove", "Color", "Display", "Save", "Rope", "Weld"];
	public Dictionary<GameObject, Color> DefaultColors = new();

	public static Playercontroller Local => GameManager.ActiveScene.Components.GetAll<Playercontroller>( FindMode.EnabledInSelfAndDescendants ).ToList().FirstOrDefault( x => x.Network.OwnerConnection.SteamId == (ulong)Game.SteamId );
	public List<Color> cycleColors = new List<Color> { Color.Red, Color.Green, Color.Cyan, Color.Blue, Color.Yellow, Color.Magenta, Color.Orange, Color.Yellow };
	public Dictionary<GameObject, int> currentColorIndex = new Dictionary<GameObject, int>();


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
			.WithoutTags([ "ragdoll" ])
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
		for ( int i = 0; i < (int)Tools.Count(); i++ )
		{
			if ( Input.Pressed( $"Slot{i + 1}" ) )
			{
				ind = i;
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
			// Network.OwnerConnection.SteamId
		}
		// chat.Transform.Rotation = Camera.Transform.Rotation; TODO fix this
		if ( isMe && ( !Input.Down( "Use" ) || Tools[ind] != "PhysGun") )
		{
			angles += Input.AnalogLook * 0.5f;
			angles.pitch = angles.pitch.Clamp( -60f, 90f );
			Transform.Rotation = Rotation.Lerp( Transform.Rotation, angles.ToRotation(), Time.Delta * 16f );
		}

		SceneTraceResult aim = Trace( Transform.Position, Transform.Position + Transform.Rotation.Forward * 5000f );
		if ( !isMe )
		{	
			aim_show.Transform.Position = aim.EndPosition;
			point.Transform.Position = Transform.Position;
		}
		else if ( aim.Hit )
		{
			Gizmo.Draw.Color = Color.Cyan;
			Gizmo.Draw.SolidSphere( aim.HitPosition, 5f, 50 );
		}
		if ( Tools[ind] == "PhysGun" )
		{
			PhysGunTool.PhysGun( aim, this );
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

	/*
	private void Thruster( SceneTraceResult aim )
	{
		GameObject picker = aim.GameObject;
		if ( picker != null && isMe && !picker.Components.GetInChildrenOrSelf<Collider>().Static )
		{
			if ( Input.Pressed( "attack1" ) )
			{
				var ThrusterObject = new GameObject();
			}
		}
	}
	*/
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}
}
