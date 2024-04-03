using Sandbox;

public sealed class RagdollRope : Component
{
	[Property] public PhysicsBody physics;
	[Property] public GameObject Object;
	GameObject joint;
	bool init = false;
	protected override void OnUpdate()
	{
		if ( physics == null || Object == null || !Object.IsValid || !physics.IsValid() )
			return;
		if ( !init )
		{
			joint = new GameObject();
			var joints = joint.Components.Create<SpringJoint>();
			joints.Body = Object;
			joints.Frequency = 5;
			joints.Damping = 0.7f;
			joints.EnableCollision = false;
			joints.MinLength = 10f;

			joints = Object.Components.Create<SpringJoint>();
			joints.Body = joint;
			joints.Frequency = 5;
			joints.Damping = 0.7f;
			joints.EnableCollision = false;
			joint.Components.Create<Rigidbody>();
			init = true;
		}
		if ( joint != null && joint.Transform != null )
		{
			physics.SmoothMove( joint.Transform.Position, 0.1f, Time.Delta );
		}
	}
}
