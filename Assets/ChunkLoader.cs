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
		public int RenderDistance = 2;
		public int Seed = 0;
		public int Variance = 10;
		public float Scale = .1f;
		public float Threshold = .5f;

		// Private
		private Dictionary<Vector2, Chunk> Chunks;
		private Queue<RenderChunk> LoadRenderChunkQueue;
		private Queue<RenderChunk> UpdateRenderChunkQueue;
		private Vector2 CenterChunkPos;

		// Use this for initialization
		void Start () {
			CenterChunkPos = GlobalPosToChunkCoord(transform.position);
			Chunks = new Dictionary<Vector2, Chunk>();
			LoadRenderChunkQueue = new Queue<RenderChunk>();
			UpdateRenderChunkQueue = new Queue<RenderChunk>();


			LoadNewChunks();

			//Vector2[] keys = Chunks.Keys.ToArray();
			//foreach(Vector2 key in keys) {
			//	LoadRenderChunkQueue.Enqueue(Chunks[key].RenderChunks[3]);
			//	LoadRenderChunkQueue.Enqueue(Chunks[key].RenderChunks[4]);
			//	LoadRenderChunkQueue.Enqueue(Chunks[key].RenderChunks[5]);
			//}

			// SetChunkSize(OrderOfMagnitude);
			// Add all renderchunks to the queue for loading
			//List<Vector2> points = GenerateSpiralPoints(OrderOfMagnitude * OrderOfMagnitude);
			//foreach (Vector2 point in points) {
			//	LoadRenderChunkQueue.Enqueue(Chunks[(int)point.x, (int)point.y].RenderChunks[3]);
			//}
			//foreach (Vector2 point in points) {
			//	for (int i = 2; i >= 0; i--) {
			//		LoadRenderChunkQueue.Enqueue(Chunks[(int)point.x, (int)point.y].RenderChunks[i]);
			//	}
			//}
		}

		// Update is called once per frame
		void Update () {
			Vector2 currentChunkPos = GlobalPosToChunkCoord(transform.position);
			if (currentChunkPos != CenterChunkPos) {
				LoadNewChunks();
				CenterChunkPos = currentChunkPos;
			}

			if (UpdateRenderChunkQueue.Count > 0) {						// Existing chunks
				RenderChunk rc = UpdateRenderChunkQueue.Dequeue();
				rc.RefreshChunkMesh();
			}
			else if (LoadRenderChunkQueue.Count > 0) {					// New chunks
				RenderChunk rc = LoadRenderChunkQueue.Dequeue();
				StartCoroutine(RenderChunkUpdate(rc));
			}


		}

		void LoadNewChunks() {
			Debug.Log(GlobalPosToChunkCoord(transform.position));
			Vector2[] spiralPoints = GenerateSpiralPoints(((RenderDistance-1)*2+1) * ((RenderDistance - 1) * 2 + 1)).ToArray();
			List<Vector2> existingKeys = Chunks.Keys.ToList();
			List<Vector2> newKeys = new List<Vector2>();

			Vector2 offset = new Vector2(-RenderDistance + 1, -RenderDistance + 1);
			foreach (Vector2 index in spiralPoints) {
				Chunk chunk;
				Vector2 chunkPos = GlobalPosToChunkCoord(transform.position) + index + offset;
				if (!Chunks.TryGetValue(chunkPos, out chunk)) {
					Vector3 lowerCoord = new Vector3(chunkPos.x * 16, 0, chunkPos.y * 16);
					Chunks.Add(chunkPos, new Chunk(lowerCoord));
					newKeys.Add(chunkPos);
				} else {
					existingKeys.Remove(chunkPos);
				}
				Debug.Log(chunkPos);
			}

			foreach (Vector2 key in newKeys) {
				LoadRenderChunkQueue.Enqueue(Chunks[key].RenderChunks[3]);
				LoadRenderChunkQueue.Enqueue(Chunks[key].RenderChunks[4]);
				LoadRenderChunkQueue.Enqueue(Chunks[key].RenderChunks[5]);
			}

			foreach (Vector2 key in existingKeys) {
				DestroyOldChunk(key);
				LoadRenderChunkQueue.
			}
		}

		public void DestroyOldChunk(Vector2 chunkPos) {
			Chunk chunk;
			if (Chunks.TryGetValue(chunkPos, out chunk)) {
				chunk.Destroy();
				Chunks.Remove(chunkPos);
			}
		}

		IEnumerator RenderChunkUpdate(RenderChunk rc) {
			rc.GenerateProceduralTerrain(Seed, Variance, Threshold, Scale, 5);
			//rc.GeneratePerlinNoise(Seed, Scale, Threshold);
			//rc.GenerateHeightmapTopology(Seed, 10, Scale);
			rc.RefreshChunkMesh();
			rc.InitializeGameObject(mat);
			yield return 0;
		}

		//public void SetChunkSize(float o) {
		//	int order = (int)o;
		//	Chunks = new Chunk[order, order];
		//	OrderOfMagnitude = order;
		//	int chunksLength = 16 * OrderOfMagnitude;
		//	for (int x = 0; x < OrderOfMagnitude; x++) {
		//		for (int z = 0; z < OrderOfMagnitude; z++) {
		//			Vector3 lowerChunkCoord = new Vector3(x*16 - chunksLength/2, 0, z*16 - chunksLength/2);
		//			Chunks[x,z] = new Chunk(lowerChunkCoord);
		//		}
		//	}
		//}

		public List<Vector2> GenerateSpiralPoints(int points) {
			List<Vector2> fin = new List<Vector2>();
			Vector2[] directions = {Vector2.right, Vector2.up, Vector2.left, Vector2.down };
			Vector2 center = new Vector2((int)Mathf.Sqrt(points) / 2, (int)Mathf.Sqrt(points) / 2);
			fin.Add(center);
			int magnitude = 1;
			int direction = 0;
			int i = 0;
			while (true) {
				for (int mi = 0; mi < 2; mi++) {
					for (int magi = 0; magi < magnitude; magi++) {
						center = center + directions[direction];
						//Debug.Log(center);
						fin.Add(center);
						i++;
						if (i == points-1) return fin;
					}
					direction = (direction + 1) % 4;
				}
				magnitude++;
			}
		}

		public List<Vector2> GenerateBFSPoints(int sizex, int sizey, int startx, int starty) {

		}

		public Vector2 GlobalPosToChunkCoord(Vector3 globalPos) {
			return new Vector2(Mathf.Floor(globalPos.x/16), Mathf.Floor(globalPos.z/16));
		}
	}
}
