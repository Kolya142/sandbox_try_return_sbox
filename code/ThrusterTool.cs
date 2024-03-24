using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class ThrusterTool
	{
		static public void Thruster( SceneTraceResult aim, Playercontroller Player )
		{
			if ( !Input.Pressed( "attack1" ) || aim.GameObject == null || aim.Body == null || aim.Body.BodyType == PhysicsBodyType.Static || aim.GameObject.Components.GetInChildrenOrSelf<Thruster>() != null )
				return;
			GameObject thruster = new GameObject();
			thruster.Transform.Position = aim.HitPosition;
			thruster.Transform.Rotation = Rotation.LookAt( aim.Normal ) * Rotation.From( new Angles( 90, 0, 0 ) );

			ModelRenderer modelRenderer = thruster.Components.Create<ModelRenderer>();
			modelRenderer.Model = Model.Load( "models/thruster/thrusterprojector.vmdl" );

			ModelCollider modelCollider = thruster.Components.Create<ModelCollider>();
			modelCollider.Model = Model.Load( "models/thruster/thrusterprojector.vmdl" );

			thruster.Components.Create<Thruster>();

			thruster.Components.Create<Rigidbody>();

			FixedJoint fixedJoint1 = thruster.Components.Create<FixedJoint>();
			fixedJoint1.Body = aim.Body.GetGameObject();

			FixedJoint fixedJoint2 = aim.Body.GetGameObject().Components.Create<FixedJoint>();
			fixedJoint2.Body = thruster;

		}
	}
}
