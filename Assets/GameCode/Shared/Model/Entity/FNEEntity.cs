using FNZ.Shared.Model.Components;
using FNZ.Shared.Model.Interfaces;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace FNZ.Shared.Model.Entity
{
	public static class EntityType
	{
		public const string TILE = "Tile";
		public const string EDGE_OBJECT = "EdgeObject";
		public const string FLOATING_POINT_OBJECT = "FloatpointObject";
		public const string TILE_OBJECT = "TileObject";
		public const string PROJECTILE = "Projectile";
		public const string ENEMY = "Enemy";
		public const string VEHICLE = "Vehicle";
	}

	public class FNEEntity : ISerializeable
	{
		public FNEEntityData data;
		public string entityNameDef;
		public string entityType;

		public float2 position;
		public float3 scale;
		public float rotationZdegrees;

		public int entityNetId = -1;

		public List<FNEComponent> components = new List<FNEComponent>();

		public void Init(Vector2 position, string entityType, string entityNameDef)
		{
			//AddComponent<PositionComponent>();
			//AddComponent<RotationComponent>();

			this.entityType = entityType;
			this.entityNameDef = entityNameDef;
			this.position = position;

			data = DataBank.Instance.GetData<FNEEntityData>(entityNameDef);
		}

		public void SendComponentMessage(FNEComponentMessage message)
		{
			foreach (var t in components)
			{
				t.Receive(message);
			}
		}

		public T AddComponent<T>() where T : FNEComponent, new()
		{
			T newComp = new T();

			foreach (var comp in components)
			{
				if (comp.GetType() == typeof(T))
				{
					Debug.LogError("COMPONENT ADDED TWICE TO ENTITY!");
				}
			}

			components.Add(newComp);
			newComp.parent = this;

			newComp.Init();

			return newComp;
		}

		public void RemoveComponent<T>()
		{
			components.RemoveAll(c => c is T);
		}

		public T GetComponent<T>() where T : FNEComponent
		{
			foreach (var comp in components)
			{
				if (comp is T)
					return comp as T;
			}

			return default(T);
		}

		public bool HasComponent<T>()
		{
			return components.FindAll(c => c is T).Count > 0;
		}

		public void Serialize(NetBuffer bw)
		{
			bw.Write(IdTranslator.Instance.GetId<FNEEntityData>(entityNameDef));

			foreach (var comp in components)
			{
				comp.Serialize(bw);
			}
		}

		public void Deserialize(NetBuffer br)
		{
			entityNameDef = IdTranslator.Instance.GetNameDef<FNEEntityData>(br.ReadUInt16());

			foreach (var comp in components)
			{
				comp.Deserialize(br);
			}
		}
	}
}

