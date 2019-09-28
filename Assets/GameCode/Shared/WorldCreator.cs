using System;
using System.Linq;
using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

namespace FNZ.Shared
{
	public class WorldCreator
	{
		public static List<ComponentSystemBase> CreateWorldAndSystemsFromAssemblies(World world, params string[] assemblyNames)
		{
			World.Active = world;
			var systems = new List<ComponentSystemBase>();

			foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var assemblyName in assemblyNames)
				{
					if (ass.GetName().Name == assemblyName)
					{
						if (ass.ManifestModule.ToString() == "Microsoft.CodeAnalysis.Scripting.dll")
							continue;
						var allTypes = ass.GetTypes();

						var systemTypes = allTypes.Where(
							t => t.IsSubclassOf(typeof(ComponentSystemBase)) &&
							!t.IsAbstract &&
							!t.ContainsGenericParameters &&
							t.GetCustomAttributes(typeof(DisableAutoCreationAttribute), true).Length > 0);

						foreach (var type in systemTypes)
						{
							var system = GetBehaviourManagerAndLogException(world, type);
							if (system != null)
								systems.Add(system);
						}
					}
				}
			}

			return systems;
		}

		public static ComponentSystemBase GetBehaviourManagerAndLogException(World world, Type type)
		{
			try
			{
				Debug.Log("Found System" + type.FullName);
				return world.GetOrCreateSystem(type);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			return null;
		}
	}
}

