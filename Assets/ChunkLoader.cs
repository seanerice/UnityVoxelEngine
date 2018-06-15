using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

/* Load chunks in on demand.
 * Chunk loading only happens when neccessarry, and updating RenderChunks happens only for 
 * RenderChunks which need to be updated. 
 * There are several functions designed to retrieve appropriate RenderChunks for update.
 */

namespace VoxelEngine {
	class ChunkLoader : MonoBehaviour {
		// Public
		public Material mat;
		public int OrderOfMagnitude = 1;
		public int Seed = 0;
		public int Variance = 10;
		public float Scale = .1f;
		public float Threshold = .5f;

		// Private
		private Chunk[,] Chunks;
		private Queue<RenderChunk> LoadRenderChunkQueue;
		private Queue<RenderChunk> UpdateRenderChunkQueue;

		// Use this for initialization
		void Start () {
			LoadRenderChunkQueue = new Queue<RenderChunk>();
			UpdateRenderChunkQueue = new Queue<RenderChunk>();
			SetChunkSize(OrderOfMagnitude);
			// Add all renderchunks to the queue for loading
			List<Vector2> points = GenerateSpiralPoints(OrderOfMagnitude * OrderOfMagnitude);
			foreach (Vector2 point in points) {
				LoadRenderChunkQueue.Enqueue(Chunks[(int)point.x, (int)point.y].RenderChunks[3]);
			}
			foreach (Vector2 point in points) {
				for (int i = 2; i >= 0; i--) {
					LoadRenderChunkQueue.Enqueue(Chunks[(int)point.x, (int)point.y].RenderChunks[i]);
				}
			}
		}

		// Update is called once per frame
		void Update () {
			if (UpdateRenderChunkQueue.Count > 0) {
				RenderChunk rc = UpdateRenderChunkQueue.Dequeue();
				rc.RefreshChunkMesh();
			}
			else if (LoadRenderChunkQueue.Count > 0) {
				RenderChunk rc = LoadRenderChunkQueue.Dequeue();
				StartCoroutine(RenderChunkUpdate(rc));
			}
		}

		IEnumerator RenderChunkUpdate(RenderChunk rc) {
			rc.UpdateRenderChunk(Seed, Variance, Threshold, Scale);
			//rc.GeneratePerlinNoise(Seed, Scale, Threshold);
			//rc.GenerateHeightmapTopology(Seed, 10, Scale);
			rc.RefreshChunkMesh();
			rc.InitializeGameObject(mat);
			yield return 0;
		}

		public void SetChunkSize(float o) {
			int order = (int)o;
			Chunks = new Chunk[order, order];
			OrderOfMagnitude = order;
			int chunksLength = 16 * OrderOfMagnitude;
			for (int x = 0; x < OrderOfMagnitude; x++) {
				for (int z = 0; z < OrderOfMagnitude; z++) {
					Vector3 lowerChunkCoord = new Vector3(x*16 - chunksLength/2, 0, z*16 - chunksLength/2);
					Chunks[x,z] = new Chunk(lowerChunkCoord);
				}
			}
		}

		public List<Vector2> GenerateSpiralPoints(int points) {
			List<Vector2> fin = new List<Vector2>();
			Vector2[] directions = {Vector2.right, Vector2.up, Vector2.left, Vector2.down };
			Vector2 center = new Vector2((int)Mathf.Sqrt(points) / 2 - 1, (int)Mathf.Sqrt(points) / 2 - 1);
			fin.Add(center);
			int magnitude = 1;
			int direction = 0;
			int i = 0;
			while (true) {
				for (int mi = 0; mi < 2; mi++) {
					for (int magi = 0; magi < magnitude; magi++) {
						center = center + directions[direction];
						Debug.Log(center);
						fin.Add(center);
						i++;
						if (i == points-1) return fin;
					}
					direction = (direction + 1) % 4;
				}
				magnitude++;
			}
		}
	}
}
