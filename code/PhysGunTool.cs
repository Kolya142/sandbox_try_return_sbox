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
			if ( Input.Pressed( "Reload" ) && Player.isMe )
			{
				Player.model = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Model;
				Player.color = picker.Components.GetInChildrenOrSelf<ModelRenderer>().Tint;
				Player.scale = picker.Transform.Scale;
			}
			// Log.Info(picker);
			if ( body != null && body.BodyType == PhysicsBodyType.Dynamic && Player.isMe )
			{
				if ( Input.Pressed( "attack1" ) )
				{
					Player.moveBody = body;
					Transform transform = new Transform( aim.HitPosition );
					Player.offsetobject = transform.PointToLocal( body.Transform.Position );
					Player.moveBody.MotionEnabled = true;
					Player.distObject = aim.Distance;
				}
			}
			if ( Input.Down( "attack1" ) && Player.moveBody != null )
			{
				Vector3 Position = Player.Transform.Position + Player.Transform.Rotation.Forward * Player.distObject;
				Transform transform = new Transform( Position );
				Position = transform.PointToWorld( Player.offsetobject );
				Player.moveBody.SmoothMove( Position, Time.Delta * 3.0f, Time.Delta );
				// Player.moveBody.Position = Vector3.Lerp( Player.moveBody.Position, Player.Transform.Position + Player.Transform.Rotation.Forward * Player.distObject, Time.Delta * 5f );
				Gizmo.Draw.Color = Color.Cyan;
				Gizmo.Draw.SolidSphere( Player.moveBody.Transform.PointToWorld( Player.offsetobject ), 5f, 50 );
			}
			if ( !Input.Down( "attack1" ) && Player.moveBody != null )
			{
				Player.moveBody = null;
				// Player.moveBody.Position = Vector3.Lerp( Player.moveBody.Position, Player.Transform.Position + Player.Transform.Rotation.Forward * Player.distObject, Time.Delta * 5f );
			}
		}
	}
}
