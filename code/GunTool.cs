﻿using System;
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
			Player.gun.Transform.Position = Player.EyePosition();

			Player.gun.Transform.Rotation = Player.EyeRotatation;

			Player.gun.Transform.Position += Player.EyeRotatation.Forward * 40f;
			Player.gun.Transform.Position += Player.EyeRotatation.Right * 20f;
			Player.gun.Transform.Position += Player.EyeRotatation.Down * 20f;
			if ( Input.Pressed( "attack1" ) )
			{
				if ( aim.Hit )
				{
					GameObject hitObject = aim.GameObject;
					if ( hitObject != null )
					{
						if ( hitObject.Components.GetInChildrenOrSelf<Playercontroller>() != null )
						{
							hitObject.Components.GetInChildrenOrSelf<Playercontroller>().Damage(4.5f);
						}
						if ( hitObject.Components.GetInChildrenOrSelf<Balloon>() != null )
						{
							hitObject.Destroy();
							var particle = new SceneParticles( Player.Scene.SceneWorld, "particles/tool_hit.vpcf" );
							particle.SetControlPoint( 0, aim.HitPosition );
							particle.SetControlPoint( 0, Rotation.Identity );
							Player.particles.Add( particle );
						}

						if ( hitObject.Components.GetInChildrenOrSelf<Rigidbody>() != null )
						{
							float mass = hitObject.Components.GetInChildrenOrSelf<Rigidbody>().PhysicsBody.Mass / 50;
							Vector3 impulse = Player.Transform.Rotation.Forward * 9000000f / hitObject.Transform.Scale.Length / mass;
							hitObject.Components.GetInChildrenOrSelf<Rigidbody>().ApplyImpulseAt( aim.HitPosition, impulse );
						}
						var damage = new DamageInfo( 50f, Player.GameObject, Player.gun );
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
