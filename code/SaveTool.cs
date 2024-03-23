using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Sandbox
{
	public class SaveTool
	{
		static public void Save( SceneTraceResult aim, Playercontroller Player )
		{
			if ( Input.Pressed( "attack1" ) && Networking.IsHost && Player.isMe )
			{
				// Serialize the current scene to a JSON object.
				foreach ( var obj in Player.Scene.SceneWorld.SceneObjects )
				{
					if ( obj.Tags.Has( "player" ) )
					{
						obj.Delete();
					}
				}
				JsonObject resource = Player.Scene.Serialize();
				// Log.Info( resource );

				// Use 'using' statement for automatic resource management.
				int lastfile = FileSystem.Data.ReadJson<int>( "lastsave" );
				FileSystem.Data.WriteJson<JsonObject>( $"scene{lastfile}.json", resource );
				FileSystem.Data.WriteJson<int>( "lastsave", lastfile + 1 );
				Game.Disconnect();
				Game.Close();
			}
		}
	}
}
