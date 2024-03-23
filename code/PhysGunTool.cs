using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class PhysGunTool
	{
		static public void PhysGun( SceneTraceResult aim, Playercontroller Player )
		{
			GameObject picker = aim.GameObject;
			if ( Input.Pressed( "attack2" ) && !Input.Down( "attack1" ) && Player.isMe )
			{
				var particle = new SceneParticles( Player.Scene.SceneWorld, "particles/createeffect.vpcf" );
				particle.SetControlPoint( 0, aim.HitPosition );
				particle.SetControlPoint( 0, Rotation.Identity );
				// particles.Add( particle );
				Sound.PlayFile( SoundFile.Load( "Sounds/balloon_pop_cute.sound" ) );
				Log.Info( "got \"entitiescount\" stat" );
				Sandbox.Services.Stats.Increment( "entitiescount", 1 );
				GameObject newobject = new GameObject( true, "spawned" );
				newobject.Transform.Position = aim.HitPosition;
				newobject.Transform.Scale = Player.scale;
				newobject.Components.Create<ModelRenderer>();
				newobject.Components.GetInChildrenOrSelf<ModelRenderer>().Model = Player.model;
				if ( Player.model == Model.Cube )
				{
					newobject.Components.Create<BoxCollider>();
				}
				else if ( Player.model == Model.Sphere )
				{
					newobject.Components.Create<SphereCollider>();
					newobject.Components.GetInChildrenOrSelf<SphereCollider>().Radius *= 2;
				}
				else if ( Player.model.Name == "models/dev/plane.vmdl" )
				{
					newobject.Components.Create<BoxCollider>();
					newobject.Components.GetInChildrenOrSelf<BoxCollider>().Scale = new Vector3( 100, 100, 3 );
				}
				else
				{
					newobject.Components.Create<ModelCollider>();
					newobject.Components.GetInChildrenOrSelf<ModelCollider>().Model = Player.model;
				}
				newobject.Components.GetInChildrenOrSelf<ModelRenderer>().Tint = Player.color;
				Log.Info( Player.model.Name );
				newobject.Components.Create<Rigidbody>();
			}
			if ( Input.Pressed( "Reload" ) && Player.isMe )
			{
				Player.model = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Model;
				Player.color = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint;
				Player.scale = picker.Transform.Scale;
			}
			// Log.Info(picker);
			if ( picker != null && Player.isMe )
			{

				float dist = MathF.Sqrt(
									MathF.Pow( aim.HitPosition.x - Player.Transform.Position.x, 2 ) +
									MathF.Pow( aim.HitPosition.y - Player.Transform.Position.y, 2 ) +
									MathF.Pow( aim.HitPosition.z - Player.Transform.Position.z, 2 )
									);
				if ( Input.Down( "attack1" ) && Player.CanDrag )
				{
					if ( Player.moveObject == null )
					{
						if ( !picker.Components.GetInChildrenOrSelf<Collider>().Static )
						{
							Player.moveObject = picker;
							Player.distObject = dist;
							Player.lastobjectPos = picker.Transform.Position;
							Player.offsetobject = aim.HitPosition - picker.Transform.Position;
							Rigidbody moveBody = Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
							if ( moveBody != null )
								moveBody.MotionEnabled = false;
						}
					}
					else
					{
						Player.distObject += Input.MouseWheel.y * 6f * (Input.Down( "Run" ) ? 2f : 1f);
						Rigidbody moveBody = Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
						Player.moveObject.Transform.Position = Player.Transform.Position + Player.Transform.Rotation.Forward * Player.distObject - Player.offsetobject;
						if ( moveBody != null )
						{
							Vector3 velocity = moveBody.Velocity;
							Vector3.SmoothDamp( Player.lastobjectPos, Player.moveObject.Transform.Position, ref velocity, 0.9f, Time.Delta );
							moveBody.Velocity = velocity;
							Player.lastobjectPos = picker.Transform.Position;
							if ( Input.Pressed( "attack2" ) )
							{
								moveBody.Velocity = Vector3.Zero;
								moveBody.AngularVelocity = Vector3.Zero;
								moveBody.AngularDamping = 0;
								Player.CanDrag = false;
								var particle = new SceneParticles( Player.Scene.SceneWorld, "particles/physgun_freeze.vpcf" );
								particle.SetControlPoint( 0, aim.HitPosition );
								particle.SetControlPoint( 0, Rotation.Identity );
								Player.particles.Add( particle );
							}
						}
						if ( Input.Pressed( "Use" ) )
							Player.angles_object = Player.moveObject.Transform.Rotation.Angles();
						if ( Input.Down( "Use" ) )
						{
							// Log.Info( Input.AnalogLook.Forward );
							Player.angles_object += Input.AnalogLook * 0.5f;
							Rotation rotation = Player.moveObject.Transform.Rotation;
							Player.moveObject.Transform.Rotation = Rotation.Lerp( rotation, Player.angles_object.ToRotation(), Time.Delta * 16f );
						}
					}
				}
				if ( !Input.Down( "attack1" ) && Player.isMe )
				{
					if ( Player.moveObject != null && Player.CanDrag )
					{
						Rigidbody moveBody = Player.moveObject.Components.GetInChildrenOrSelf<Rigidbody>();
						if ( moveBody != null )
						{
							moveBody.MotionEnabled = true;
							moveBody.PhysicsBody.Enabled = true;
							//moveBody.Enabled = true;
						}
						if ( moveBody.Velocity.Length > 1000f )
						{
							Log.Info( "got \"velua\" stat" );
							Sandbox.Services.Stats.Increment( "velya", 1 );
						}
					}
					Player.CanDrag = true;
					Player.moveObject = null;
				}
			}
		}
	}
}
