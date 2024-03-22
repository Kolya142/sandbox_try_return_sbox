using Sandbox;

public sealed class GameController : Component
{
	[Property] public GameObject ServerTitle;

	protected override void OnAwake()
	{
		base.OnAwake();
		if ( Networking.IsHost )
		{
			if ( Game.IsEditor )
			{
				Log.Info( "Dev Server" );
				ServerTitle.Components.GetInChildrenOrSelf<TextRenderer>().Text = "IS DEV SERVER\nIs automatic message\nIf (Networking.IsHost && Game.IsEditor)";
			}
			else
			{
				ServerTitle.Components.GetInChildrenOrSelf<TextRenderer>().Text = "IS NOT DEV SERVER";
			}
		}
		if ( !FileSystem.Data.DirectoryExists( "saves" ) )
		{
			FileSystem.Data.CreateDirectory( "saves" );
		}
		if ( !FileSystem.Data.FileExists( "lastsave" ) )
		{
			FileSystem.Data.WriteJson<int>( "lastsave", 0 );
		}
	}
	protected override void OnUpdate()
	{

	}
}
