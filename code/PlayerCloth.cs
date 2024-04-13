using Sandbox;
using Sandbox.Services;
using System.Threading.Channels;

public sealed class PlayerCloth : Component, Component.INetworkListener
{
	[Property]
	public SkinnedModelRenderer BodyRenderer { get; set; }

	public void OnNetworkSpawn( Connection owner )
	{
		var clothing = new ClothingContainer();
		clothing.Deserialize( owner.GetUserData( "avatar" ) );
		Stats.Increment( "what", 1 );
		clothing.Apply( BodyRenderer );
	}
}
