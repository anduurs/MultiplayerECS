using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FNZ.Shared.Model.Interfaces
{
	public interface ISerializeable
	{
		void Serialize(NetBuffer bw);
		void Deserialize(NetBuffer br);
	}
}

