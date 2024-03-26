using Sandbox.Services;
using System;
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
				Stats.Increment( "ctsp", 1 );
				GameObject terry = new GameObject();
				terry.Transform.Position = aim.HitPosition;

				SkinnedModelRenderer skinnedModelRenderer = terry.Components.Create<SkinnedModelRenderer>();
				skinnedModelRenderer.Model = Model.Load( "models/citizen/citizen.vmdl" );

				ModelPhysics modelPhysics = terry.Components.Create<ModelPhysics>();
				modelPhysics.Model = Model.Load( "models/citizen/citizen.vmdl" );
				modelPhysics.Renderer = skinnedModelRenderer;

				PlayerCloth cloth = terry.Components.Create<PlayerCloth>();
				cloth.BodyTarget = skinnedModelRenderer;
				cloth.cloth = Player.Network.OwnerConnection.GetUserData( "avatar" );
				terry.NetworkSpawn( Player.Network.OwnerConnection );
			}
			if ( Input.Pressed( "attack3" ) )
			{
				Stats.Increment( "ctsp", 1 );
				GameObject terry = new GameObject();
				terry.Transform.Position = aim.HitPosition;

				SkinnedModelRenderer skinnedModelRenderer = terry.Components.Create<SkinnedModelRenderer>();
				skinnedModelRenderer.Model = Model.Load( "models/citizen/citizen.vmdl" );

				ModelPhysics modelPhysics = terry.Components.Create<ModelPhysics>();
				modelPhysics.Model = Model.Load( "models/citizen/citizen.vmdl" );
				modelPhysics.Renderer = skinnedModelRenderer;

				LocalCloth cloth = terry.Components.Create<LocalCloth>();
				cloth.BodyTarget = skinnedModelRenderer;
				terry.NetworkSpawn( Player.Network.OwnerConnection );
			}
			if ( Input.Pressed( "attack2" ) )
			{
				Stats.Increment( "ctsp", 1 );
				GameObject terry = new GameObject();
				terry.Transform.Position = aim.HitPosition;

				SkinnedModelRenderer skinnedModelRenderer = terry.Components.Create<SkinnedModelRenderer>();
				skinnedModelRenderer.Model = Model.Load( "models/citizen/citizen.vmdl" );

				ModelPhysics modelPhysics = terry.Components.Create<ModelPhysics>();
				modelPhysics.Model = Model.Load( "models/citizen/citizen.vmdl" );
				modelPhysics.Renderer = skinnedModelRenderer;
				terry.NetworkSpawn( Player.Network.OwnerConnection );
			}
		}
	}
}
