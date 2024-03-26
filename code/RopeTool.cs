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
		static public void CreateRope(GameObject a, GameObject b, float Frequency = 5f, float Damping = 0.6f, bool MinLenghtSet = true)
		{
			a.Components.Create<SpringJoint>();
			int lastind = a.Components.GetAll<SpringJoint>().ToArray().Length - 1;
			SpringJoint joint = a.Components.GetAll<SpringJoint>().ToArray()[lastind];
			joint.Body = b;
			joint.Frequency = Frequency;
			joint.Damping = Damping;
			if ( MinLenghtSet ) joint.MinLength = b.Transform.Position.Distance( a.Transform.Position ) * 2f;

			b.Components.Create<SpringJoint>();
			lastind = b.Components.GetAll<SpringJoint>().ToArray().Length - 1;
			joint = b.Components.GetAll<SpringJoint>().ToArray()[lastind];
			joint.Body = a;
			joint.Frequency = Frequency;
			joint.Damping = Damping;
			if ( MinLenghtSet ) joint.MinLength = b.Transform.Position.Distance( a.Transform.Position ) * 2f;

			a.NetworkSpawn();
			b.NetworkSpawn();

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
					Player.lastObjectOffset = picker.Transform.Position - aim.HitPosition;
				}
				else
				{
					GameObject a = new GameObject();
					a.Transform.Position = Player.lastObject.Transform.Position + Player.lastObjectOffset;
					a.Components.Create<FixedJoint>().Body = Player.lastObject;
					a.Components.Create<Rigidbody>();
					Player.lastObject.Components.Create<FixedJoint>().Body = a;

					GameObject b = new GameObject();
					b.Transform.Position = aim.HitPosition;
					b.Components.Create<FixedJoint>().Body = picker;
					b.Components.Create<Rigidbody>();
					picker.Components.Create<FixedJoint>().Body = b;

					CreateRope(a, b);
					Player.lastObject = null;
				}
			}
		}
	}
}
