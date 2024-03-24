using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class RopeTool
	{
		static public void CreateRope(GameObject a, GameObject b)
		{
			a.Components.Create<SpringJoint>();
			int lastind = a.Components.GetAll<SpringJoint>().ToArray().Length - 1;
			SpringJoint joint = a.Components.GetAll<SpringJoint>().ToArray()[lastind];
			joint.Body = b;
			joint.Frequency = 5f;
			joint.Damping = 0.6f;
			joint.MinLength = b.Transform.Position.Distance( a.Transform.Position ) * 2f;

			b.Components.Create<SpringJoint>();
			lastind = b.Components.GetAll<SpringJoint>().ToArray().Length - 1;
			joint = b.Components.GetAll<SpringJoint>().ToArray()[lastind];
			joint.Body = a;
			joint.Frequency = 5f;
			joint.Damping = 0.6f;
			joint.MinLength = b.Transform.Position.Distance( a.Transform.Position ) * 2f;
			JointLine.Create( b, a );
		}
		static public void Rope( SceneTraceResult aim, Playercontroller Player )
		{
			GameObject picker = aim.GameObject;
			if ( picker != null && Player.isMe && aim.Body.BodyType != PhysicsBodyType.Static && Input.Pressed( "attack1" ) )
			{
				if ( Player.lastObject == null )
				{
					Player.lastObject = picker;
				}
				else
				{
					CreateRope(Player.lastObject, picker);
					Player.lastObject = null;
				}
			}
		}
	}
}
