using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class JointLine
	{
		static public void Create( GameObject a, GameObject b )
		{
			GameObject Line = new();
			Line.Components.Create<LineRenderer>();
			Line.Components.GetInChildrenOrSelf<LineRenderer>().Color = Color.Cyan;
			List<GameObject> points = new();
			points.Add( a );
			points.Add( b );
			Line.Components.GetInChildrenOrSelf<LineRenderer>().Points = points;
			List<Curve.Frame> frames = new();
			frames.Add( new Curve.Frame( 0, 1 ) );
			frames.Add( new Curve.Frame( 1, 1 ) );
			Line.Components.GetInChildrenOrSelf<LineRenderer>().Width = new Curve( frames );
			Line.NetworkSpawn();
		}
	}
}
