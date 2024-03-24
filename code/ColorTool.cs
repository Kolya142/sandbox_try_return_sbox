using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class ColorTool
	{
		static public void Color( SceneTraceResult aim, Playercontroller Player )
		{
			GameObject picker = aim.GameObject;
			PhysicsBody body = aim.Body;
			if ( picker != null && Player.isMe && body.BodyType != PhysicsBodyType.Static )
			{
				if ( Input.Pressed( "attack1" ) )
				{
					if ( !Player.DefaultColors.ContainsKey( picker ) )
					{
						Player.DefaultColors.Add( picker, picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint );
					}
					if ( Player.currentColorIndex.ContainsKey( picker ) )
					{
						Player.currentColorIndex[picker]++;
						Player.currentColorIndex[picker] = Player.currentColorIndex[picker] % Player.cycleColors.Count;
					}
					else
					{
						Player.currentColorIndex[picker] = 0;
					}
					picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint = Player.cycleColors[Player.currentColorIndex[picker]];
				}
				else if ( Input.Pressed( "attack2" ) && Player.DefaultColors.ContainsKey( picker ) )
				{
					Player.currentColorIndex[picker] = 0;
					picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint = Player.DefaultColors[picker];
				}
			}
		}
	}
}
