using FNZ.Client.Model.World;
using FNZ.Shared.Model.Entity.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FNZ.Client.Model
{
	public class ClientEntityFactory
	{
        private Dictionary<Type, Type> m_CompDataToCompDictionary = new Dictionary<Type, Type>();

        private ClientWorld m_World;

        public ClientEntityFactory(ClientWorld world)
        {
            m_World = world;

            BuildTable();
        }

        private void BuildTable()
        {
            var allSharedTypes = typeof(FNEComponentData).Assembly.GetTypes();
            var allComponentDatatypes = allSharedTypes.Where(t =>
                t.IsSubclassOf(typeof(FNEComponentData))
            ).ToList();

            foreach (var type in allComponentDatatypes)
            {
                AddClientComponentType(type);
            }
        }

        private void AddClientComponentType(Type dataType)
        {
            var allClientTypes = typeof(ClientEntityFactory).Assembly.GetTypes();
            var matchingComponent = allClientTypes.Where(t =>
                t.BaseType.GenericTypeArguments.Contains(dataType)
            ).ToList();

            if (matchingComponent.Count == 0)
            {
                Debug.LogError("WARNNIG: " + dataType + " Does not have a matching client component!");
                return;
            }else if (matchingComponent.Count > 1)
            {
                Debug.LogError("WARNNIG: " + dataType + " Has more than one matching client component!");
                return;
            }

            m_CompDataToCompDictionary.Add(dataType, matchingComponent.First());
        }
    }
}

