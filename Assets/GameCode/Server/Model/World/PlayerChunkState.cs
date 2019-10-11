using FNZ.Shared.Model.Entity;
using System;
using System.Collections.Generic;

namespace FNZ.Server.Model.World
{
	public class PlayerChunkState
	{
		public List<WorldChunk> CurrentlyLoadedChunks = new List<WorldChunk>();
		public List<WorldChunk> CurrentlyActiveChunks = new List<WorldChunk>();

		public List<WorldChunk> ChunksAwaitingLoad = new List<WorldChunk>();
		public List<Tuple<WorldChunk, long>> ChunksAwaitingUnload = new List<Tuple<WorldChunk, long>>();

		public List<WorldChunk> ChunksSentForLoadAwaitingConfirm = new List<WorldChunk>();
		public List<WorldChunk> ChunksSentForUnloadAwaitingConfirm = new List<WorldChunk>();

		public List<FNEEntity> entitiesToLoad = new List<FNEEntity>();
		public List<FNEEntity> entitiesToUnload = new List<FNEEntity>();

		public bool IsChunkInLoadedState(WorldChunk chunk)
		{
			return
				CurrentlyLoadedChunks.Contains(chunk)
				|| ChunksSentForLoadAwaitingConfirm.Contains(chunk)
				|| ChunksAwaitingUnload.FindAll(t => t.Item1 == chunk).Count == 1;
		}
	}
}

