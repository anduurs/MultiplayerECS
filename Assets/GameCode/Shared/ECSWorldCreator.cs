using System;
using System.Linq;
using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;
using System.Reflection;

namespace FNZ.Shared
{
	public class ECSWorldCreator
	{
		public static List<Type> GetSystemsFromAssemblies(World world, params string[] assemblyNames)
		{
			World.Active = world;
			var systemTypes = new List<Type>();

			foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var assemblyName in assemblyNames)
				{
					if (ass.GetName().Name == assemblyName)
					{
						if (ass.ManifestModule.ToString() == "Microsoft.CodeAnalysis.Scripting.dll")
							continue;
						var allTypes = ass.GetTypes();

						var types = allTypes.Where(
							t => t.IsSubclassOf(typeof(ComponentSystemBase)) &&
							!t.IsAbstract &&
							!t.ContainsGenericParameters &&
							t.GetCustomAttributes(typeof(DisableAutoCreationAttribute), true).Length == 0);

						foreach (var type in types)
						{
							systemTypes.Add(type);
						}
					}
				}
			}

			return systemTypes;
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

		static MethodInfo insertManagerIntoSubsystemListMethod = 
			typeof(ScriptBehaviourUpdateOrder).GetMethod("InsertManagerIntoSubsystemList", 
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

		public static void UpdatePlayerLoop(World world)
		{
			var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
			if (ScriptBehaviourUpdateOrder.CurrentPlayerLoop.subSystemList != null)
				playerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;
			if (world != null)
			{
				for (var i = 0; i < playerLoop.subSystemList.Length; ++i)
				{
					int subsystemListLength = playerLoop.subSystemList[i].subSystemList.Length;
					if (playerLoop.subSystemList[i].type == typeof(FixedUpdate))
					{
						var newSubsystemList = new PlayerLoopSystem[subsystemListLength + 1];
						for (var j = 0; j < subsystemListLength; ++j)
							newSubsystemList[j] = playerLoop.subSystemList[i].subSystemList[j];
						var mgr = world.GetOrCreateSystem<SimulationSystemGroup>();
						var genericMethod = insertManagerIntoSubsystemListMethod.MakeGenericMethod(mgr.GetType());
						genericMethod.Invoke(null, new object[] { newSubsystemList, subsystemListLength + 0, mgr });
						playerLoop.subSystemList[i].subSystemList = newSubsystemList;
					}
					else if (playerLoop.subSystemList[i].type == typeof(Update))
					{
						var newSubsystemList = new PlayerLoopSystem[subsystemListLength + 1];
						for (var j = 0; j < subsystemListLength; ++j)
							newSubsystemList[j] = playerLoop.subSystemList[i].subSystemList[j];
						var mgr = world.GetOrCreateSystem<PresentationSystemGroup>();
						var genericMethod = insertManagerIntoSubsystemListMethod.MakeGenericMethod(mgr.GetType());
						genericMethod.Invoke(null, new object[] { newSubsystemList, subsystemListLength + 0, mgr });
						playerLoop.subSystemList[i].subSystemList = newSubsystemList;
					}
					else if (playerLoop.subSystemList[i].type == typeof(Initialization))
					{
						var newSubsystemList = new PlayerLoopSystem[subsystemListLength + 1];
						for (var j = 0; j < subsystemListLength; ++j)
							newSubsystemList[j] = playerLoop.subSystemList[i].subSystemList[j];
						var mgr = world.GetOrCreateSystem<InitializationSystemGroup>();
						var genericMethod = insertManagerIntoSubsystemListMethod.MakeGenericMethod(mgr.GetType());
						genericMethod.Invoke(null, new object[] { newSubsystemList, subsystemListLength + 0, mgr });
						playerLoop.subSystemList[i].subSystemList = newSubsystemList;
					}
				}
			}

			ScriptBehaviourUpdateOrder.SetPlayerLoop(playerLoop);
		}
	}
}

