using Sandbox;

public sealed class LocalCloth : Component
{
	[Property] public SkinnedModelRenderer BodyTarget;
	private bool isInit = false;

	protected override void OnUpdate()
	{
		if ( isInit )
			return;
		if ( BodyTarget == null )
			return;
		isInit = true;
		var clothing = ClothingContainer.CreateFromLocalUser();
		clothing.Apply( BodyTarget );
	}
}
