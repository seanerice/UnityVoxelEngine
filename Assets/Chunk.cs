using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VoxelEngine {
	public class Chunk {
		private Vector3 LowerGlobalCoord;
		private const int numRenderChunks = 8;
		public RenderChunk[] RenderChunks;

		public Chunk (Vector3 lowerCoord) {
			LowerGlobalCoord = lowerCoord;
			RenderChunks = new RenderChunk[numRenderChunks];
			for (int i = 0; i < numRenderChunks; i++) {
				RenderChunks[i] = new RenderChunk(new Vector3(LowerGlobalCoord.x, i * 16, LowerGlobalCoord.z));
			}
		}

		public RenderChunk GetRenderChunkByCoord(Vector3 coord) {
			return RenderChunks[(int)coord.y / numRenderChunks];
		}

		public void Destroy() {
			foreach (RenderChunk rc in RenderChunks) {
				Mesh.Destroy(rc.RenderMesh);
				GameObject.Destroy(rc.RenderObject);
				rc.MarkedForDestruction = true;
			}
		}
	}
}
