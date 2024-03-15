using Sandbox;

public sealed class GameController : Component
{
	[Property] public GameObject ServerTitle;

	protected override void OnAwake()
	{
		base.OnAwake();
		if (Networking.IsHost)
		{
			if ( Game.IsEditor )
			{
				Log.Info( "Dev Server" );
				ServerTitle.Components.GetInChildrenOrSelf<TextRenderer>().Text = "IS DEV SERVER, i know.\nIs automatic message";
			}
		}
	}
	protected override void OnUpdate()
	{

	}
}
