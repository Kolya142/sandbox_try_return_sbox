using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
	public class RemoveTool
	{
		static public List<GameObject> TraceConnections(GameObject start)
		{
			List<GameObject> objects = new();
			List<GameObject> currents = new()
			{
				start
			};
			objects.Add( start );
			while (true)
			{
				// Log.Info( currents.Count );
				if ( currents.Count == 0 )
					break;
				GameObject current = currents.Last();
				currents.Remove( current );
				if (current.Components.GetAll<Joint>().Count() == 0)
				{
					break;
				}
				else
				{
					GameObject last = current;
					List<Joint> joints = current.Components.GetAll<Joint>().ToList();
					// Log.Info( "[" );
					foreach ( Joint joint in joints) { 
						GameObject body = joint.Body;
						// Log.Info( body );
						if ( body == null ||
							objects.Exists( e => e == body ) || currents.Exists( e => e == body ) ||
							(body.Components.Get<Rigidbody>() == null && body.Components.Get<ModelPhysics>() == null))
						{
							// Log.Info( "Not Ok" );
							// Log.Info( objects.Exists( e => e == body ) || currents.Exists( e => e == body ) );
							// Log.Info( (body.Components.Get<Rigidbody>() == null && body.Components.Get<ModelPhysics>() == null) );
							continue;
						}
						// Log.Info( "Ok" );
						objects.Add( body );
						currents.Add( body );
					}
					// Log.Info( "]" );
					// Log.Info( currents.Count );
				}
			}
			return objects;
		}
		static public void Remove( SceneTraceResult aim, Playercontroller Player )
		{
			GameObject picker = aim.GameObject;
			if ( picker != null && Player.isMe && aim.Body.BodyType == PhysicsBodyType.Dynamic )
			{
				if ( Input.Pressed( "attack1" ) )
				{
					picker.Destroy();
				}

				if ( Input.Pressed( "attack2" ) )
				{
					List<GameObject> gameObjects = TraceConnections( picker );
					foreach ( GameObject obj in gameObjects )
					{
						if ( obj.Components.Get<ModelRenderer>() != null ) {
							obj.Components.Get<ModelRenderer>().Tint = Color.Green;
						}
					}
				}
			}
		}
	}
}
