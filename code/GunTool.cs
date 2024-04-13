using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Sandbox.Component;

namespace Sandbox
{
	public class GunTool
	{
		static public void Gun( SceneTraceResult aim, Playercontroller Player )
		{
			if ( !Player.IsProxy )
			{
				Player.viewgun.Transform.Position = Player.EyePosition();

				Player.viewgun.Transform.Rotation = Player.EyeRotatation;

				Player.gun.Components.Get<ModelRenderer>( includeDisabled: true ).RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
			}
			if ( Player.viewgun != null )
				Player.viewgun.Components.Get<ModelRenderer>( includeDisabled: true ).Enabled = true; 
			if ( Player.gun != null )
				Player.gun.Components.Get<ModelRenderer>( includeDisabled: true ).Enabled = true;

			Player.citizenAnimationHelper.HoldType = Citizen.CitizenAnimationHelper.HoldTypes.Pistol;
			if ( Input.Pressed( "attack1" ) )
			{
				if ( aim.Hit )
				{
					GameObject hitObject = aim.GameObject;
					PhysicsBody hitBody = aim.Body;
					if ( hitObject != null )
					{
						Player.viewgun.Components.Get<SkinnedModelRenderer>().Set( "fire", true );
						Player.ShootAnim();
						if ( hitObject.Components.GetInChildrenOrSelf<Playercontroller>() != null )
						{
							hitObject.Components.GetInChildrenOrSelf<Playercontroller>().Damage(4.5f);
						}
						if ( hitObject.Components.GetInChildrenOrSelf<Balloon>() != null )
						{
							// hitObject.Destroy();
							var particle = new SceneParticles( Player.Scene.SceneWorld, "particles/tool_hit.vpcf" );
							particle.SetControlPoint( 0, aim.HitPosition );
							particle.SetControlPoint( 0, Rotation.Identity );
							Player.particles.Add( particle );
						}
						if ( hitBody != null )
						{
							hitBody.ApplyImpulseAt( aim.HitPosition, aim.Direction * 1000.0f * aim.Body.Mass.Clamp( 0, 1000 ) );
						}
						if ( Player.DecalEffect is not null )
						{
							var decal = Player.DecalEffect.Clone( new Transform( aim.HitPosition + aim.Normal * 2.0f, Rotation.LookAt( -aim.Normal, Vector3.Random ), Game.Random.Float( 0.8f, 1.2f ) ) );
							decal.Components.Create<SelfDestroyComponent>().time = 20f;
							decal.SetParent( aim.GameObject );
						}
						var damage = new DamageInfo( 10f, Player.GameObject, Player.gun );
						damage.Position = aim.HitPosition;
						damage.Shape = aim.Shape;

						foreach ( var damageable in aim.GameObject.Components.GetAll<IDamageable>() )
						{
							damageable.OnDamage( damage );
						}
					}
				}
			}
		}
	}
}
