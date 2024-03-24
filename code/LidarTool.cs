using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class LidarTool
	{
		static public void Lidar( SceneTraceResult aim, Playercontroller Player )
		{
			if ( Player.isMe )
			{
				for ( int i = 0; i < Screen.Width; i += 40 )
				{
					for ( int j = 0; j < Screen.Height + 20; j += 40 )
					{
						Ray ray = Game.ActiveScene.Camera.ScreenPixelToRay( new Vector3( i, j ) );
						SceneTraceResult traceResut = Player.Trace( ray.Position, ray.Forward * 1000 + ray.Position );
						Gizmo.Draw.Color = Color.Magenta;
						Gizmo.Draw.SolidSphere( traceResut.HitPosition, 1f, 4 );
					}
				}
			}
		}
	}
}
