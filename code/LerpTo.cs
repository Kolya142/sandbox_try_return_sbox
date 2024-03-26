using Sandbox;

public sealed class LerpTo : Component
{
	[Property] public GameObject Object;
	[Property] public float frac = 0.5f;
	[Property] public float rotationfrac = 0.9f;
	[Property] public Vector3 offset = Vector3.Up * 30 + Vector3.Backward * 20;
	[Property] public Rotation rotationoffset = Rotation.Identity;
	protected override void OnFixedUpdate()
	{
		Vector3 Position = Object.Transform.Position + offset * Object.Transform.Rotation;
		Vector3 Direction = (Object.Transform.Position - Transform.Position).Normal;
		Transform.Rotation = Rotation.Lerp( Transform.Rotation, Rotation.LookAt( Direction ) * rotationoffset, frac );
		Transform.Position =  Vector3.Lerp( Transform.Position, Position, frac );
	}
}
