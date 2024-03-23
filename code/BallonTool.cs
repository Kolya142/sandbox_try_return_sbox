﻿using System;
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
			if ( !aim.Hit || aim.GameObject == null )
				return;
			if ( Input.Pressed( "attack1" ) )
			{
				// Log.Info( 1 );
				GameObject picker = aim.GameObject;
				// Log.Info( Game.ActiveScene.Components.GetAll<GameController>() );
				GameObject balloon = Game.ActiveScene.Components.GetAll<GameController>().ToArray()[0].BallonPrefab.Clone( aim.HitPosition );
				// Log.Info( 3 ); 
				RopeTool.CreateRope( picker, balloon );
			}
			else if ( Input.Pressed( "attack2" ) )
			{
				GameObject balloon = Game.ActiveScene.Components.GetAll<GameController>().ToArray()[0].BallonPrefab.Clone( aim.HitPosition );
			}
		}
	}
}