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
            for (int i = 0; i < numRenderChunks; i++)
            {
                if (i > 0) RenderChunks[i].Down = RenderChunks[i - 1];
                if (i < numRenderChunks - 1) RenderChunks[i].Up = RenderChunks[i + 1];
            }
        }

		public RenderChunk GetRenderChunkByCoord(Vector3 coord) {
            //Debug.Log((int)coord.y / 16);
			return RenderChunks[(int)coord.y / 16];
		}

		public void Load(int seed, int variance, float threshold, float scale, int grassThickness, Material mat) {
			foreach (RenderChunk rc in RenderChunks) {
				rc.GenerateProceduralTerrain(seed, variance, threshold, scale, grassThickness);
                rc.InitializeGameObject(mat);
			}
			
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
