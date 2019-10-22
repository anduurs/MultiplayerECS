using FNZ.Server.Model.World;
using FNZ.Shared.Model;
using FNZ.Shared.Model.Entity;
using FNZ.Shared.Model.Entity.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace FNZ.Server.Model
{
	public class ServerEntityFactory
	{
        private Dictionary<Type, Type> m_CompDataToCompDictionary = new Dictionary<Type, Type>();

		private ServerWorld m_World;

		public ServerEntityFactory(ServerWorld world)
		{
			m_World = world;

            BuildTable();
        }

		public FNEEntity CreatePlayer(float2 position, string name)
		{
            var playerEntity = new FNEEntity();

			playerEntity.Init(position, EntityType.PLAYER, "player");

			playerEntity.Enabled = false;

			var componentsFromXML = DataBank.Instance.GetData<FNEEntityData>(playerEntity.entityNameDef).components;

			foreach (var compData in componentsFromXML)
			{
                var comp = playerEntity.AddComponent(m_CompDataToCompDictionary[compData.GetType()], compData);
            }

			//var nameComp = entity.AddComponent<NameComponent>();
			//nameComp.entityName = playerName;

			m_World.AddPlayerEntity(playerEntity);

			return playerEntity;
		}

        private void BuildTable()
        {
            var allSharedTypes = typeof(FNEComponentData).Assembly.GetTypes();
            var allComponentDatatypes = allSharedTypes.Where(t =>
                t.IsSubclassOf(typeof(FNEComponentData))
            ).ToList();

            foreach (var type in allComponentDatatypes)
            {
                AddServerComponentType(type);
            }
        }

        private void AddServerComponentType(Type dataType)
        {
            var allServerTypes = typeof(ServerEntityFactory).Assembly.GetTypes();
            var matchingcomponents = allServerTypes.Where(t =>
                t.BaseType.GenericTypeArguments.Contains(dataType)
            ).ToList();

            if(matchingcomponents.Count == 0)
            {
                Debug.LogError("WARNNIG: " + dataType + " Does not have a matching server component!");
            }
            else if (matchingcomponents.Count > 1)
            {
                Debug.LogError("WARNNIG: " + dataType + " Has more than one matching server component!");
            }

            m_CompDataToCompDictionary.Add(dataType, matchingcomponents.First());
        }
    }
}

