using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class DublicatorTool
	{
		static public void Dublicate( SceneTraceResult aim, Playercontroller Player )
		{
			GameObject picker = aim.GameObject;
			if ( picker != null && Player.isMe && aim.Body.BodyType == PhysicsBodyType.Dynamic )
			{
				if ( Input.Pressed( "attack2" ) )
				{
					List<GameObject> gameObjects = RemoveTool.TraceConnections( picker );
					GameObject main = new GameObject();
					main.Transform.Position = aim.EndPosition;
					foreach ( var item in gameObjects )
					{
						item.Parent = main;
						// item.Transform.Position -= main.Transform.Position;
					}
					Player.test = main.Serialize();
				}
			}

			if ( Input.Pressed( "attack1" ) )
			{
				Log.Info( Player.test );
				GameObject objectq = new GameObject();
				objectq.Name = "BackUpped";
				objectq.Deserialize( Player.test );
				objectq.Transform.Position = aim.EndPosition;
				objectq.NetworkSpawn( Player.Network.OwnerConnection );
			}
		}
	}
}
