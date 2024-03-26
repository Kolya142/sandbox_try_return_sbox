using Sandbox;

public sealed class Rotator : Component
{
	[Property] public Vector3 Offset;
	[Property] public LerpTo lerpTo;
	[Property] public float frac = 0.65f;

	protected override void OnUpdate()
	{
		Angles look = Input.AnalogLook;
		look.roll = -look.roll;
		Offset = Vector3.Lerp( Offset, look.ToRotation() * Offset, frac );
		/*
		SceneTrace scenetrace = Scene.Trace.Ray( lerpTo.Object.Transform.Position, Offset )
			.UseHitboxes()
			.IgnoreGameObject( lerpTo.Object );
		foreach ( var child in lerpTo.Object.Children )
		{
			scenetrace = scenetrace.IgnoreGameObject( child );
		}
		Vector3 offset = scenetrace.Run().EndPosition;*/
		lerpTo.offset = Vector3.Lerp(lerpTo.offset, Offset, frac);
	}
}
