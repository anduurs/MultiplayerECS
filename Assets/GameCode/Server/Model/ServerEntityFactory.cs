using FNZ.Server.Model.World;
using FNZ.Shared.Model;
using FNZ.Shared.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace FNZ.Server.Model
{
	public class ServerEntityFactory
	{
		private ServerWorld m_World;

		public ServerEntityFactory(ServerWorld world)
		{
			m_World = world;
		}

		public FNEEntity CreatePlayer(float2 position, string name)
		{
			var playerEntity = new FNEEntity();

			playerEntity.Init(position, EntityType.PLAYER, "player");

			playerEntity.enabled = false;

			var componentsFromXML = DataBank.Instance.GetData<FNEEntityData>(playerEntity.entityNameDef).components;

			foreach (var compData in componentsFromXML)
			{
				if (!compData.IsDataOnly())
				{
					var comp = playerEntity.AddComponent(compData.GetComponentType(), compData);
				}
			}

			//var nameComp = entity.AddComponent<NameComponent>();
			//nameComp.entityName = playerName;

			m_World.AddPlayerEntity(playerEntity);

			return playerEntity;
		}
	}
}

