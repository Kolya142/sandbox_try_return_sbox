using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class BallonTool
	{
		static public void Balloon( SceneTraceResult aim, Playercontroller Player )
		{
			if ( !aim.Hit || aim.Body == null )
				return;
			if ( Input.Pressed( "attack1" ) )
			{
				// Log.Info( 1 );
				PhysicsBody picker = aim.Body;
				// Log.Info( Game.ActiveScene.Components.GetAll<GameController>() );
				GameObject balloon = Game.ActiveScene.Components.GetAll<GameController>().ToArray()[0].BallonPrefab.Clone( aim.HitPosition );
				// Log.Info( 3 ); 

				GameObject a = new GameObject();
				a.Transform.Position = aim.HitPosition;
				a.Components.Create<FixedJoint>().Body = aim.Body.GetGameObject();
				a.Components.Create<Rigidbody>();
				aim.Body.GetGameObject().Components.Create<FixedJoint>().Body = a;

				RopeTool.CreateRope( a, balloon );
			}
			else if ( Input.Pressed( "attack2" ) )
			{
				GameObject balloon = Game.ActiveScene.Components.GetAll<GameController>().ToArray()[0].BallonPrefab.Clone( aim.HitPosition );
			}
		}
	}
}
