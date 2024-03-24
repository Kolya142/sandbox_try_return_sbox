﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class CreateTerryTool
	{
		static public void CreateTerry( SceneTraceResult aim, Playercontroller Player )
		{
			if ( !aim.Hit  )
				return;
			if ( Input.Pressed( "attack1" ) )
			{
				GameObject terry = new GameObject();
				terry.Transform.Position = aim.HitPosition;

				SkinnedModelRenderer skinnedModelRenderer = terry.Components.Create<SkinnedModelRenderer>();
				skinnedModelRenderer.Model = Model.Load( "models/citizen/citizen.vmdl" );

				ModelPhysics modelPhysics = terry.Components.Create<ModelPhysics>();
				modelPhysics.Model = Model.Load( "models/citizen/citizen.vmdl" );
				modelPhysics.Renderer = skinnedModelRenderer;

				LocalCloth cloth = terry.Components.Create<LocalCloth>();
				cloth.BodyTarget = skinnedModelRenderer;
			}
			if ( Input.Pressed( "attack2" ) )
			{
				GameObject terry = new GameObject();
				terry.Transform.Position = aim.HitPosition;

				SkinnedModelRenderer skinnedModelRenderer = terry.Components.Create<SkinnedModelRenderer>();
				skinnedModelRenderer.Model = Model.Load( "models/citizen/citizen.vmdl" );

				ModelPhysics modelPhysics = terry.Components.Create<ModelPhysics>();
				modelPhysics.Model = Model.Load( "models/citizen/citizen.vmdl" );
				modelPhysics.Renderer = skinnedModelRenderer;
			}
		}
	}
}
