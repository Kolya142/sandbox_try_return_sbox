using Sandbox.Services;
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
			if ( !Player.isMe )
				return;
			GameObject picker = aim.GameObject;
			PhysicsBody body = aim.Body;
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
				// Player.ModelLoad( Player.model.ResourceName, false );
				// Player.model.BoneCount
				if ( Player.model.BoneCount > 1 )
				{
					newobject.Components.Create<SkinnedModelRenderer>();
					newobject.Components.GetInChildrenOrSelf<SkinnedModelRenderer>().Model = Player.model;
					newobject.Components.Create<ModelPhysics>();
					newobject.Components.GetInChildrenOrSelf<ModelPhysics>().Model = Player.model;
					newobject.Components.GetInChildrenOrSelf<ModelPhysics>().Renderer = newobject.Components.GetInChildrenOrSelf<SkinnedModelRenderer>();
					newobject.Components.GetInChildrenOrSelf<SkinnedModelRenderer>().Tint = Player.color;
					Log.Info( Player.model.Name );
					Stats.Increment( "ctsp", 1 );
				}
				else
				{
					newobject.Components.Create<ModelRenderer>();
					newobject.Components.GetInChildrenOrSelf<ModelRenderer>().Model = Player.model;
					newobject.Components.GetInChildrenOrSelf<ModelRenderer>().Tint = Player.color;
					Log.Info( Player.model.Name );
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
					newobject.Components.Create<Rigidbody>();
				}
				newobject.NetworkSpawn( Player.Network.OwnerConnection );
					/*
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
					}*/
				}
			if ( picker != null && Input.Pressed( "Reload" ) && Player.isMe && picker.Components.GetInChildrenOrSelf<ModelRenderer>() != null )
			{
				Player.model = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Model;
				Player.color = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint;
				Player.scale = picker.Transform.Scale;
			}
			// Log.Info(picker);
			if ( body != null && body.BodyType != PhysicsBodyType.Static && Player.isMe && Player.moveBody == null )
			{
				if ( Input.Down( "attack1" ) )
				{
					Player.moveBody = body;
					Transform transform = new Transform( aim.HitPosition );
					Player.offsetobject = transform.PointToLocal( body.Transform.Position );
					Player.moveBody.Locking = default;
					Player.moveBody.MotionEnabled = true;
					Player.distObject = aim.Distance;
					Player.angles_object = Player.moveBody.Rotation.Angles();
				}
			}
			if ( Input.Down( "attack1" ) && Player.moveBody != null )
			{
				Vector3 Position = Player.EyePosition() + Player.EyeRotatation.Forward * Player.distObject;
				Transform transform = new Transform( Position );
				Position = transform.PointToWorld( Player.offsetobject );
				Player.moveBody.SmoothMove( Position, Time.Delta * 3 * (Player.moveBody.Mass / 100000f), Time.Delta );
				// Player.moveBody.Position = Vector3.Lerp( Player.moveBody.Position, Player.Transform.Position + Player.Transform.Rotation.Forward * Player.distObject, Time.Delta * 5f );
				Gizmo.Draw.Color = Color.Cyan;
				Gizmo.Draw.SolidSphere( Player.moveBody.Position - Player.offsetobject, 5f, 50 ); 

				Gizmo.Draw.Color = Color.Cyan;
				Gizmo.Draw.SolidSphere( Position, 5f, 50 );

				Vector3 Start = Player.moveBody.Position - Player.offsetobject;
				Vector3 End = Position;
				float dist = Start.Distance( End );
				Vector3 delta = ( Start - End ).Normal;
				for ( float n = 0f; n < dist; n += 10f )
				{
					Gizmo.Draw.Color = Color.Cyan;
					Gizmo.Draw.SolidSphere( Start - delta * n, 2f, 50 );
				}
				Player.moveBody.Rotation = Rotation.Lerp( Player.moveBody.Rotation, Player.angles_object.ToRotation(), Time.Delta * 6f );
				Player.angles_object = Angles.Lerp( Player.angles_object, Player.moveBody.Rotation.Angles(), Time.Delta * 2f );

				Player.distObject += 2f * Input.MouseWheel.y * (Input.Down("run") ? 2f : 1f) * (Input.Down( "duck" ) ? 0.5f : 1f);
			}
			if ( Input.Down( "attack1" ) && Input.Pressed( "attack2" ) )
			{
				Player.moveBody.BodyType = PhysicsBodyType.Keyframed;
				Player.moveBody.Velocity = default;
				PhysicsLock locking = new PhysicsLock();
				locking.X = true;
				locking.Y = true;
				locking.Z = true;
				locking.Yaw = true;
				locking.Pitch = true;
				locking.Roll = true;
				Player.moveBody.Locking = locking;
			}
			if ( Input.Down( "attack1" ) && Input.Down( "use" ) )
			{
				Player.angles_object += Input.AnalogLook * 0.5f;
			}
			if ( !Input.Down( "attack1" ) )
			{
				Player.moveBody = null;
				// Player.moveBody.Position = Vector3.Lerp( Player.moveBody.Position, Player.Transform.Position + Player.Transform.Rotation.Forward * Player.distObject, Time.Delta * 5f );
			}
		}
	}
}
