using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class ScaleTool
{
	static public void Scale( SceneTraceResult aim, Playercontroller Player )
	{
		GameObject picker = aim.GameObject;
		if ( picker != null && Player.isMe )
		{
			picker.Transform.Scale *= new Vector3( Input.MouseWheel.y * 0.1f + 1f );
		}
	}
}
