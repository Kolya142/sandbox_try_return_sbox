using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class WeldTool
	{
		static public void Weld( SceneTraceResult aim, Playercontroller Player )
		{
			GameObject picker = aim.GameObject;
			if ( picker != null && Player.isMe && !picker.Components.GetInChildrenOrSelf<Collider>().Static && Input.Pressed( "attack1" ) )
			{
				if ( Player.lastObject == null )
				{
					Player.lastObject = picker;
				}
				else
				{
					Player.lastObject.Components.Create<FixedJoint>();
					int lastind = Player.lastObject.Components.GetAll<FixedJoint>().ToArray().Length - 1;
					FixedJoint joint = Player.lastObject.Components.GetAll<FixedJoint>().ToArray()[lastind];
					joint.Body = picker;

					picker.Components.Create<FixedJoint>();
					lastind = picker.Components.GetAll<FixedJoint>().ToArray().Length - 1;
					joint = picker.Components.GetAll<FixedJoint>().ToArray()[lastind];
					joint.Body = Player.lastObject;
					JointLine.Create( picker, Player.lastObject );
					Player.lastObject = null;
				}
			}
		}
	}
}
