﻿using FNZ.Shared.Model.Entity;
using FNZ.Shared.Model.Interfaces;
using Lidgren.Network;
using System;

namespace FNZ.Shared.Model.Components
{
	public enum FNEComponentMessage
	{
		REACHED_TARGET,
		ZERO_HEALTH,
	}

	public abstract class FNEComponentData : DataComponent
	{
		public abstract bool IsDataOnly();
		public abstract Type GetComponentType();
	}

	public abstract class FNEComponent : ISerializeable
	{
		public FNEEntity parent;
		protected FNEComponentData m_Data;
		protected bool m_Enabled = true;

		public virtual void Init() { }

		public virtual void Receive(FNEComponentMessage message) { }

		public virtual void Serialize(NetBuffer writer) { }

		public virtual void Deserialize(NetBuffer reader) { }

		public abstract ushort GetSizeInBytes();

		public T GetData<T>() where T : FNEComponentData { return m_Data as T; }

		public void SetData(FNEComponentData data) { m_Data = data; }

		public void Enable() { m_Enabled = true; }

		public void Disable() { m_Enabled = false; }

		public bool IsEnabled() { return m_Enabled; }
	}
}
