using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class InfoTool
	{
		static public void Info( SceneTraceResult aim, Playercontroller Player )
		{
			if ( !Player.isMe || !aim.Hit || aim.Body == null )
				return;
			PhysicsBody body = aim.Body;
			Vector3 Position = body.Position;
			Rotation rotation = body.Rotation;
			Vector3 Velocity = body.Velocity;
			Vector3 AngularVelocity = body.AngularVelocity;
			float Mass = body.Mass;
			string text =
				$"Mass:            {Mass}\n" +
				$"Position:        {Position}\n" +
				$"Velocity:        {Velocity}\n" +
				$"Rotation:        {rotation.Forward}\n" +
				$"AngularVelocity: {AngularVelocity}\n" +
				$"";
			Gizmo.Draw.ScreenText( text, new Vector2( 40, 50 ) );
		}
	}
}
