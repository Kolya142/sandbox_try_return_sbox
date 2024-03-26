﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class LightTool
	{
		static public void Light( SceneTraceResult aim, Playercontroller Player )
		{
			if ( !aim.Hit || aim.Body == null )
				return;
			if ( Input.Pressed( "attack1" ) )
			{
				// Log.Info( 1 );
				PhysicsBody picker = aim.Body;
				// Log.Info( Game.ActiveScene.Components.GetAll<GameController>() );
				GameObject light = new GameObject();
				light.Transform.Position = aim.HitPosition + aim.Normal * 5f;
				var renderer = light.Components.Create<ModelRenderer>();
				renderer.Model = Model.Load( "models/light/light_tubular.vmdl" );
				light.Components.Create<ModelCollider>().Model = Model.Load( "models/light/light_tubular.vmdl" );
				light.Components.Create<Rigidbody>();
				GameObject lightE = new GameObject();
				lightE.Transform.Position = Vector3.Up * 10f;
				PointLight lightC = lightE.Components.Create<PointLight>();
				lightC.Radius = 256;
				lightC.LightColor = Color.Random;
				renderer.Tint = lightC.LightColor;
				renderer.RenderType = ModelRenderer.ShadowRenderType.Off;
				lightC.Shadows = true;
				lightE.Parent = light;
				// Log.Info( 3 ); 

				GameObject a = new GameObject();
				a.Transform.Position = aim.HitPosition;
				a.Components.Create<FixedJoint>().Body = aim.Body.GetGameObject();
				a.Components.Create<Rigidbody>();
				aim.Body.GetGameObject().Components.Create<FixedJoint>().Body = a;

				RopeTool.CreateRope( a, light, 0.7f, 5f, false );
			}
			else if ( Input.Pressed( "attack2" ) )
			{
				GameObject light = new GameObject();
				light.Transform.Position = aim.HitPosition + aim.Normal * 5f;
				var renderer = light.Components.Create<ModelRenderer>();
				renderer.Model = Model.Load( "models/light/light_tubular.vmdl" );
				light.Components.Create<ModelCollider>().Model = Model.Load( "models/light/light_tubular.vmdl" );
				light.Components.Create<Rigidbody>();
				GameObject lightE = new GameObject();
				lightE.Transform.Position = Vector3.Up * 10f;
				PointLight lightC = lightE.Components.Create<PointLight>();
				lightC.Radius = 256;
				lightC.LightColor = Color.Random;
				renderer.Tint = lightC.LightColor;
				renderer.RenderType = ModelRenderer.ShadowRenderType.Off;
				lightC.Shadows = true;
				lightE.Parent = light;
				lightE.NetworkSpawn();
				light.NetworkSpawn();
			}
		}
	}
}
