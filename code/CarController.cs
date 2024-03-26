using Sandbox;
using System.Numerics;

public sealed class CarController : Component, IUsable
{
	public Rigidbody body;
	[Property] public float speed = 600f;
	[Property] public float rotationspeed = 270f;
	[Property] public GameObject rf;
	[Property] public GameObject rb;
	[Property] public GameObject lf;
	[Property] public GameObject lb;
	[Sync] public GameObject Player { get; set }
	[Sync] public Playercontroller PlayerC { get; set }

	private float Usetime = 0;

	protected override void OnAwake()
	{
		base.OnAwake();
		body = Components.GetInChildrenOrSelf<Rigidbody>();
	}
	protected override void OnUpdate()
	{
		// Transform.Rotation = Rotation.Lerp( Transform.Rotation, Rotation.LookAt( Transform.Rotation.Forward.WithZ( 0 ) ), 0.01f );
		if ( Player == null || !PlayerC.Network.IsOwner )
		{
			Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = true;
			return;
		}
		CameraComponent Camera = Game.ActiveScene.Camera;
		if ( Input.Pressed( "use" ) && Time.Now - Usetime > 1f )
		{
			PlayerC.nenabled = true;
			PlayerC.modelCollider.Enabled = true;
			Player.Transform.Position += Vector3.Up * 200f + Vector3.Left * 100f;
			PlayerC.citizenAnimationHelper.IsSitting = false;
			PlayerC.citizenAnimationHelper.Sitting = Sandbox.Citizen.CitizenAnimationHelper.SittingStyle.None;
			Player = null;
			PlayerC = null;
			Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = false;
			Camera.Components.GetInChildrenOrSelf<LerpTo>().Enabled = false;
			/*
			if ( PlayerC.Components.Get<LocalCloth>() != null )
			{
				PlayerC.Components.Get<LocalCloth>().Enabled = false;
			}*/
			Camera.Components.GetInChildrenOrSelf<Rotator>().Enabled = false;
			return;
		}
		PlayerC.modelCollider.Enabled = false;
		Player.Transform.Position = Transform.Position + Transform.Rotation.Up * 20f + Transform.Rotation.Backward * 5f;
		PlayerC.citizenAnimationHelper.WithLook( Transform.Rotation.Forward );
		PlayerC.citizenAnimationHelper.IsSitting = true;
		PlayerC.citizenAnimationHelper.IsGrounded = true;
		PlayerC.citizenAnimationHelper.IsGrounded = true;
		PlayerC.citizenAnimationHelper.Sitting = Sandbox.Citizen.CitizenAnimationHelper.SittingStyle.Chair;
		Player.Transform.Scale = Transform.Scale;
		Player.Transform.Rotation = Transform.Rotation;
		PlayerC.modelself.Components.GetInChildrenOrSelf<ModelRenderer>().RenderType = ModelRenderer.ShadowRenderType.On;
		foreach ( var child in PlayerC.modelself.Children )
		{
			child.Components.GetInChildrenOrSelf<ModelRenderer>().RenderType = ModelRenderer.ShadowRenderType.On;
		}

		Transform transform = new Transform( Transform.Position, Transform.Rotation, Transform.Scale );
		Gizmo.Draw.Color = Color.White;
		Gizmo.Draw.LineThickness = 0.05f;
		Gizmo.Draw.LineBBox( Components.Get<ModelRenderer>().Bounds );
		SceneTraceResult IsGrounded = Scene.Trace.Ray( Transform.Position, Transform.Position - Vector3.Down * 100f ).IgnoreGameObject( GameObject ).Run();
		// Gizmo.Draw.Line( Transform.Position, IsGrounded.HitPosition);
		Gizmo.Draw.LineThickness = 1f;
		Vector3 Move = Input.AnalogMove;
		float MoveFB = Move.x; // -1 - Backward, 1 - Forward
		float MoveRL = Move.y; // 1 - Left, -1 - Right
		body.AngularVelocity = Vector3.Lerp( body.AngularVelocity, new Vector3( 0, 0, MoveRL * Time.Delta * rotationspeed * MoveFB ), 0.3f );
		Vector3 Velocity = new Vector3( MoveFB * speed, 0, 0 );
		// Log.Info( transform.PointToWorld( Velocity ) );
		float z = body.Velocity.z;
		body.Velocity = Vector3.Lerp( body.Velocity, ( Velocity * Transform.Rotation ).WithZ(z), 0.5f );

		rb.Transform.LocalRotation = new Angles( 0, 90, -transform.PointToLocal(body.Velocity).x ).ToRotation();
		rf.Transform.LocalRotation = new Angles( 0, 90, -transform.PointToLocal( body.Velocity ).x ).ToRotation();

		lb.Transform.LocalRotation = new Angles( 0, -90, transform.PointToLocal( body.Velocity ).x ).ToRotation();
		lf.Transform.LocalRotation = new Angles( 0, -90, transform.PointToLocal( body.Velocity ).x ).ToRotation();
	}
	public void OnUse( Playercontroller player )
	{
		Player = player.GameObject;
		PlayerC = player;
		PlayerC.nenabled = false;
		Usetime = Time.Now;
		CameraComponent Camera = Game.ActiveScene.Camera;
		LerpTo lerp = Camera.Components.GetOrCreate<LerpTo>();
		lerp.Enabled = true;
		lerp.frac = 0.25f;
		lerp.rotationfrac = 0.28f;
		lerp.rotationoffset = new Rotation( 0, -0.03141076f, 0, 0.9995065f );
		lerp.offset = new Vector3( -400.7f, 0, 156.1f );
		lerp.Object = GameObject;
		Rotator rotator = Camera.Components.GetOrCreate<Rotator>();
		rotator.Enabled = true;
		rotator.Offset = lerp.offset;
		rotator.lerpTo = lerp;
		if ( !player.isMe ) 
			return;
		LocalCloth localcloth = player.Components.GetOrCreate<LocalCloth>();
		localcloth.Enabled = true;
		localcloth.BodyTarget = player.modelself.Components.Get<SkinnedModelRenderer>();
	}
}
