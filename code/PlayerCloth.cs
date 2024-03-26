using Sandbox;
using System.Threading.Channels;

public sealed class PlayerCloth : Component, Component.INetworkListener
{
	private readonly object channel;
	[Property] public SkinnedModelRenderer BodyTarget;
	public string cloth;
	private bool isInit = false;

	protected override void OnUpdate()
	{
		if ( isInit )
			return;
		if ( BodyTarget == null )
			return;
		isInit = true;
		var clothing = new ClothingContainer();
		clothing.Deserialize( cloth );
		clothing.Apply( BodyTarget );
	}
}
