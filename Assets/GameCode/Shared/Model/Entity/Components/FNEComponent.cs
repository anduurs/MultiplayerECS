using FNZ.Shared.Model.Entity;
using FNZ.Shared.Model.Interfaces;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FNZ.Shared.Model.Entity.Components
{
    public enum FNEComponentMessage
	{
		REACHED_TARGET,
		ZERO_HEALTH,
	}

	public abstract class FNEComponentData : DataComponent
	{
        public abstract void Serialize(NetBuffer bufferWriter);
        public abstract void Deserialize(NetBuffer bufferReader);
        public abstract ushort GetSizeInBytes();
    }

    public abstract class FNEComponent<T> where T : FNEComponentData
    {
        public FNEEntity parent;
		protected T m_Data;
		protected bool m_Enabled = true;

		public virtual void Init() { }

		public virtual void Receive(FNEComponentMessage message) { }

		public virtual void Serialize(NetBuffer writer) { }

		public virtual void Deserialize(NetBuffer reader) { }

		public T GetData() { return m_Data as T; }

		public void SetData(T data) { m_Data = data; }

		public void Enable() { m_Enabled = true; }

		public void Disable() { m_Enabled = false; }

		public bool IsEnabled() { return m_Enabled; }
	}
}

