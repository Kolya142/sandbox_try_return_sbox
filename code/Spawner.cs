using Sandbox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class SpawnTool
	{
		static public void Spawn( SceneTraceResult aim, Playercontroller Player )
		{
			if ( !aim.Hit )
				return;
			if ( Input.Pressed( "attack1" ) )
			{
				Stats.Increment( "what", 1 );
				Stats.Increment( "ctsp", 1 );
				GameObject car = Game.ActiveScene.Components.GetAll<GameController>().ToArray()[0].CarPrefab.Clone( aim.HitPosition );
				car.NetworkSpawn();
			}
			if ( Input.Pressed( "nextEntity" ) )
			{

			}
		}
	}
}
