using Sandbox;
using System.Numerics;

public sealed class Thruster : Component, IUsable
{
	SceneParticles effects;
	[Sync] bool Work { get; set; } = true;

	protected override void OnAwake()
	{
		base.OnAwake();
		effects = new SceneParticles( Scene.SceneWorld, "particles/physgun_end_nohit.vpcf" );
	}

	protected override void OnUpdate()
	{
		if ( !Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled || !Work )
			return;
		effects.SetControlPoint( 0, Transform.Position + Transform.Rotation.Up*20f );
		// effects.SetControlPoint( 0, Transform.Rotation );
		effects.Simulate( Time.Delta );
		Vector3 velocity = Components.GetInChildrenOrSelf<Rigidbody>().Velocity;
		velocity = Vector3.Lerp( velocity, Transform.Rotation.Down * 600f, 0.4f );
		Components.GetInChildrenOrSelf<Rigidbody>().Velocity = velocity;
	}

	public void OnUse( Playercontroller player )
	{
		Work = !Work;
		if ( !Work )
		{
			effects.Delete();
		}
		else
		{
			effects = new SceneParticles( Scene.SceneWorld, "particles/physgun_end_nohit.vpcf" );
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		effects.Delete();
	}
}
