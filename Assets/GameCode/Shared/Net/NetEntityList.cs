using FNZ.Shared.Model.Entity;
using UnityEngine;

namespace FNZ.Shared.Net
{
	public class NetEntityList
	{
		public FNEEntity[] list;

		public NetEntityList(int num)
		{
			list = new FNEEntity[num];
		}

		public int GetNumOfSyncedEntities()
		{
			int count = 0;

			for (int i = 0; i < list.Length; i++)
			{
				if (list[i] != null) count++;
			}

			return count;
		}

		public int GetSize()
		{
			return list.Length;
		}

		public void Add(FNEEntity e)
		{
			bool foundEmptySlot = false;

			for (int i = 1; i < list.Length; i++)
			{
				if (list[i] == null)
				{
					list[i] = e;
					e.entityNetId = i;
					foundEmptySlot = true;
					break;
				}
			}

			if (!foundEmptySlot)
			{
				FNEEntity[] newList = new FNEEntity[list.Length * 2];

				for (int i = 1; i < list.Length; i++)
				{
					newList[i] = list[i];
				}

				newList[list.Length] = e;
				e.entityNetId = list.Length;
				list = newList;
			}

			if (e.entityNetId == 0)
			{
				Debug.LogError("WAT");
			}
		}

		public void Add(FNEEntity e, int NetEntityId)
		{
			if (NetEntityId == 0) Debug.LogError("The NetAPI entityNameDef given to Add was 0 which is not allowed");
			if (list.Length <= NetEntityId)
			{
				int doubleTimes = NetEntityId / list.Length;
				FNEEntity[] newList = new FNEEntity[list.Length * doubleTimes * 2];

				for (int i = 0; i < list.Length; i++)
				{
					newList[i] = list[i];
				}

				newList[NetEntityId] = e;
				e.entityNetId = NetEntityId;
				list = newList;
			}
			else
			{
				list[NetEntityId] = e;
				e.entityNetId = NetEntityId;
			}

			if (e.entityNetId == 0)
			{
				Debug.LogError("WAT AGAIN");
			}
		}

		public void Remove(int index)
		{
			if (index == 0) Debug.LogError("The NetAPI entityNameDef given to Remove was 0 which is not allowed");
			list[index] = null;
		}

		public FNEEntity GetEntity(int index)
		{
			if (index == 0) Debug.LogError("The NetAPI entityNameDef given to GetEntity was 0 which is not allowed");
			if (index >= list.Length) return null;
			return list[index];
		}
	}
}

