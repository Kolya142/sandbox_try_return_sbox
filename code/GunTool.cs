using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class GunTool
	{
		static public void Gun( SceneTraceResult aim, Playercontroller Player )
		{
			Player.gun.Transform.Position = Player.Transform.Position;

			Player.gun.Transform.Rotation = Player.Transform.Rotation;

			Player.gun.Transform.Position += Player.Transform.Rotation.Forward * 40f;
			Player.gun.Transform.Position += Player.Transform.Rotation.Right * 20f;
			Player.gun.Transform.Position += Player.Transform.Rotation.Down * 20f;
			if ( Input.Pressed( "attack1" ) )
			{
				if ( aim.Hit )
				{
					GameObject hitObject = aim.GameObject;
					if ( hitObject != null )
					{
						if ( hitObject.Components.GetInChildrenOrSelf<Playercontroller>() != null )
						{
							hitObject.Components.GetInChildrenOrSelf<Playercontroller>().Health -= 4.5f;
						}

						if ( hitObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
						{
							float mass = hitObject.Components.GetInChildrenOrSelf<Rigidbody>().PhysicsBody.Mass / 50;
							Vector3 impulse = Player.Transform.Rotation.Forward * 9000000f / hitObject.Transform.Scale.Length / mass;
							hitObject.Components.GetInChildrenOrSelf<Rigidbody>().ApplyImpulseAt( aim.HitPosition, impulse );
						}
					}
				}
			}
		}
	}
}
