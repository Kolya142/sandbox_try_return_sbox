using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class RopeTool
	{
		static public void Rope( SceneTraceResult aim, Playercontroller Player )
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
					Player.lastObject.Components.Create<SpringJoint>();
					int lastind = Player.lastObject.Components.GetAll<SpringJoint>().ToArray().Length - 1;
					SpringJoint joint = Player.lastObject.Components.GetAll<SpringJoint>().ToArray()[lastind];
					joint.Body = picker;
					joint.Frequency = 5f;
					joint.Damping = 0.6f;
					joint.MinLength = picker.Transform.Position.Distance( Player.lastObject.Transform.Position ) * 2f;

					picker.Components.Create<SpringJoint>();
					lastind = picker.Components.GetAll<SpringJoint>().ToArray().Length - 1;
					joint = picker.Components.GetAll<SpringJoint>().ToArray()[lastind];
					joint.Body = Player.lastObject;
					joint.Frequency = 5f;
					joint.Damping = 0.6f;
					joint.MinLength = picker.Transform.Position.Distance( Player.lastObject.Transform.Position ) * 2f;
					JointLine.Create( picker, Player.lastObject );
					Player.lastObject = null;
				}
			}
		}
	}
}
