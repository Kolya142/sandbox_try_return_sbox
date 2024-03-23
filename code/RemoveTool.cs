﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class RemoveTool
	{
		static public void Remove( SceneTraceResult aim, Playercontroller Player )
		{
			GameObject picker = aim.GameObject;
			if ( picker != null && Player.isMe && !picker.Components.GetInChildrenOrSelf<Collider>().Static )
			{
				if ( Input.Pressed( "attack1" ) )
				{
					picker.Destroy();
				}
			}
		}
	}
}
