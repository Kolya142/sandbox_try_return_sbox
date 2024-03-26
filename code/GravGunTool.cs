using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class GravGunTool
	{
		static public void GravGun( SceneTraceResult aim, Playercontroller Player )
		{
			PhysicsBody body = aim.Body;
			if ( body != null && Player.isMe && (body.BodyType == PhysicsBodyType.Dynamic || Player.moveBody != null) && aim.Distance < 500f )
			{
				if ( Input.Pressed( "attack2" ) )
				{
					if ( Player.moveBody == null )
					{
						Player.moveBody = body;
						Player.moveBody.BodyType = PhysicsBodyType.Keyframed;
					}
					else
					{
						Player.moveBody = null;
						Player.moveBody.BodyType = PhysicsBodyType.Keyframed;
					}
				}
				else if ( Input.Pressed( "attack1" ) )
				{
					if ( Player.moveBody == null )
						Player.moveBody = body;
					Player.moveBody.BodyType = PhysicsBodyType.Dynamic;
					Player.moveBody.ApplyImpulse( Player.Transform.Rotation.Forward * 1000000f );
					Player.moveBody = null;
				}
			}
			else if (Player.moveBody != null)
			{
				if ( Input.Pressed( "attack2" ) )
				{
					Player.moveBody = null;
					Player.moveBody.BodyType = PhysicsBodyType.Keyframed;
				}
				if ( Input.Pressed( "attack1" ) )
				{
					Player.moveBody.BodyType = PhysicsBodyType.Dynamic;
					Player.moveBody.ApplyImpulse( Player.Transform.Rotation.Forward * 1000000f );
					Player.moveBody = null;
				}
			}
			if ( Player.moveBody != null )
			{
				Player.moveBody.Position = Player.EyePosition() + Player.EyeRotatation.Forward * 150f;
				// Player.moveBody.Rotation = Rotation.Identity;
			}
		}
	}
}
