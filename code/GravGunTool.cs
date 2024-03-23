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
			GameObject picker = aim.GameObject;
			if ( picker != null && Player.isMe && !picker.Components.GetInChildrenOrSelf<Collider>().Static && aim.Distance < 500f )
			{
				if ( Input.Pressed( "attack2" ) )
				{
					if ( Player.moveObject == null )
					{
						Player.moveObject = picker;
						if ( Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
						{
							Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = false;
						}
					}
					else
					{
						if ( Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
						{
							Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = true;
						}
						Player.moveObject = null;
					}
				}
				else if ( Input.Pressed( "attack1" ) )
				{
					if ( Player.moveObject == null )
						Player.moveObject = picker;
					if ( Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
					{
						Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>().MotionEnabled = true;
						Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>().Velocity = Player.Transform.Rotation.Forward * 1000f;
					}
					Player.moveObject = null;
				}
			}
			if ( Player.moveObject != null )
			{
				Player.moveObject.Transform.Position = Player.Transform.Position + Player.Transform.Rotation.Forward * 150f;
				Player.moveObject.Transform.Rotation = Rotation.Identity;
				if ( Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
				{
					Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>().Velocity = Vector3.Zero;
				}
			}
		}
	}
}
