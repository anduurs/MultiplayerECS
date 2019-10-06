using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Shared.Net
{
	public enum SequenceChannel
	{
		DEFAULT = 0,
		WORLD_STATE = 1,
		WORLD_SETUP = 2,
		ENTITY_STATE = 3,
		PLAYER_STATE = 4,
		ITEM_STATE = 5,
		DEBUG = 6,
	}

	public enum PacketType
	{
		WORLD_SETUP = 0,
		REQUEST_WORLD_SPAWN = 1,
	}
}

