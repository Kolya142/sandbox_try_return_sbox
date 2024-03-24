using Sandbox;
using System.Numerics;

public sealed class Thruster : Component
{
	SceneParticles effects;

	protected override void OnAwake()
	{
		base.OnAwake();
		effects = new SceneParticles( Scene.SceneWorld, "particles/physgun_end_nohit.vpcf" );
	}

	protected override void OnUpdate()
	{
		if ( !Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled )
			return;
		effects.SetControlPoint( 0, Transform.Position + Transform.Rotation.Up*20f );
		// effects.SetControlPoint( 0, Transform.Rotation );
		effects.Simulate( Time.Delta );
		Vector3 velocity = Components.GetInChildrenOrSelf<Rigidbody>().Velocity;
		velocity = Vector3.Lerp( velocity, Transform.Rotation.Down * 600f, 0.4f );
		Components.GetInChildrenOrSelf<Rigidbody>().Velocity = velocity;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		effects.Delete();
	}
}
