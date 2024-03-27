using Sandbox;

public sealed class GameController : Component
{
	[Property] public GameObject ServerTitle;
	[Property] public GameObject BallonPrefab;
	[Property] public GameObject CarPrefab;

	protected override void OnAwake()
	{
		base.OnAwake();
		if ( Networking.IsHost && ServerTitle != null )
		{
			if ( Game.IsEditor )
			{
				Log.Info( "Dev Server" );
				ServerTitle.Components.GetInChildrenOrSelf<TextRenderer>().Text = "IS DEV SERVER\nIs automatic message\nIf (Networking.IsHost && Game.IsEditor)";
				ServerTitle.Components.GetInChildrenOrSelf<TextRenderer>().Color = Color.Cyan;
			}
			else
			{
				ServerTitle.Components.GetInChildrenOrSelf<TextRenderer>().Text = "IS NOT DEV SERVER";
				ServerTitle.Components.GetInChildrenOrSelf<TextRenderer>().Color = Color.Yellow;
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
