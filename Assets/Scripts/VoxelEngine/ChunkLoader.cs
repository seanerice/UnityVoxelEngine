using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading;

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
		private Queue<Chunk> LoadChunkQueue;
		private Queue<RenderChunk> UpdateRenderChunkQueue;
		private Queue<RenderChunk> InitializeRenderChunkQueue;
		private Vector2 CenterChunkPos;
		private Camera ThisCamera;
        private float ellapsedTickTime;

		void Start () {
			CenterChunkPos = GlobalPosToChunkCoord(transform.position);
			Chunks = new Dictionary<Vector2, Chunk>();
			LoadChunkQueue = new Queue<Chunk>();
			UpdateRenderChunkQueue = new Queue<RenderChunk>();
			InitializeRenderChunkQueue = new Queue<RenderChunk>();
			ThisCamera = gameObject.GetComponentInChildren<Camera>();

			LoadNewChunks();
		}

		void Update () {

            Vector2 currentChunkPos = GlobalPosToChunkCoord(transform.position);
            if (currentChunkPos != CenterChunkPos)
            {
                Debug.Log("Loading New Chunk: " + currentChunkPos);
                LoadNewChunks();
            }

            if (LoadChunkQueue.Count > 0)
            {                       // New chunks
                Chunk chunk = LoadChunkQueue.Dequeue();
                chunk.Load(Seed, Variance, Threshold, Scale, 5, mat);
            }
            if (UpdateRenderChunkQueue.Count > 0)
            {                     // Existing chunks
                RenderChunk rc = UpdateRenderChunkQueue.Peek();
                if (rc.TerrainGenerated) {
                    UpdateRenderChunkQueue.Dequeue();
                    rc.Render();
                }
                //rc.RefreshChunkMesh();
                //Debug.Log("Refreshed renderchunk");
            }

            ellapsedTickTime += Time.deltaTime;
            if (ellapsedTickTime >= 1/20) {
                RenderVisibleChunks();
                ellapsedTickTime = 0;
            }
        }

		void RenderVisibleChunks() {
			Vector3 offset = new Vector3(8, 8, 8);
            Vector3 pos = transform.position;
			foreach (Vector2 key in Chunks.Keys.OrderBy(k => Mathf.Sqrt( Mathf.Pow(pos.x-(k.x*16), 2) + Mathf.Pow(pos.z-(k.y*16), 2)))) {
                Chunk ch = Chunks[key];
				foreach (RenderChunk rc in ch.RenderChunks) {
					if (rc.Render()) return;
				}
			}
		}

		void LoadNewChunks() {
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
			}

			Vector3 lowerCoordOffset = new Vector3(8, 8, 8);
			foreach (Vector2 key in newKeys) {
                Chunk chunk = Chunks[key];
                Chunk chunkFront, chunkBack, chunkLeft, chunkRight;
                Chunks.TryGetValue(key + Vector2.down, out chunkFront);
                Chunks.TryGetValue(key + Vector2.up, out chunkBack);
                Chunks.TryGetValue(key + Vector2.left, out chunkLeft);
                Chunks.TryGetValue(key + Vector2.right, out chunkRight);
                for (int i = 0; i < 8; i++) {
                    RenderChunk rc = chunk.RenderChunks[i];
                    if (chunkFront != null) {
                        rc.Front = chunkFront.RenderChunks[i];
                        chunkFront.RenderChunks[i].Back = rc;
                    }
                    if (chunkBack != null) {
                        rc.Back = chunkBack.RenderChunks[i];
                        chunkBack.RenderChunks[i].Front = rc;
                    }
                    if (chunkLeft != null) {
                        rc.Left = chunkLeft.RenderChunks[i];
                        chunkLeft.RenderChunks[i].Right = rc;
                    }
                    if (chunkRight != null) {
                        rc.Right = chunkRight.RenderChunks[i];
                        chunkRight.RenderChunks[i].Left = rc;
                    }
                }
				LoadChunkQueue.Enqueue(Chunks[key]);
			}

			foreach (Vector2 key in existingKeys) { // At this point existing keys contains only chunks to be destroyed
				DestroyOldChunk(key);
			}

            Vector2 currentChunkPos = GlobalPosToChunkCoord(transform.position);
            CenterChunkPos = currentChunkPos;
            RenderChunk bfsStartChunk = Chunks[currentChunkPos].GetRenderChunkByCoord(transform.position);
            Vector3 vindex = bfsStartChunk.GlobalToIndex(transform.position);
            bfsStartChunk.BfsVoxelQueue.Enqueue(bfsStartChunk.Voxels[(int)vindex.x, (int)vindex.y, (int)vindex.z]);

            Vector3 pos = transform.position;
			foreach (Vector2 key in Chunks.Keys.OrderBy(k => Mathf.Sqrt( Mathf.Pow(pos.x-(k.x*16), 2) + Mathf.Pow(pos.z-(k.y*16), 2)))) {
                Chunk ch = Chunks[key];
				foreach (RenderChunk rc in ch.RenderChunks) {
                    UpdateRenderChunkQueue.Enqueue(rc);
				}
			}
        }

		public void DestroyOldChunk(Vector2 chunkPos) {
			Chunk chunk;
			if (Chunks.TryGetValue(chunkPos, out chunk)) {
				chunk.Destroy();
				Chunks.Remove(chunkPos);
			}
		}

		public void SetRenderDistance(float f) {
			RenderDistance = (int)f;
			ForceFullChunkRefresh();
		}

		public void ForceFullChunkRefresh() {
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();
			foreach (Chunk chunk in Chunks.Values) {
				chunk.Destroy();
			}
			stopwatch.Stop();
			Debug.Log(stopwatch.ElapsedMilliseconds);
			stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Chunks.Clear();
			LoadChunkQueue.Clear();
			UpdateRenderChunkQueue.Clear();
			stopwatch.Stop();
			Debug.Log(stopwatch.ElapsedMilliseconds);

			LoadNewChunks();
		}

        public bool AddBlock(Vector3 globalPos, VoxelType voxelType)
        {
            Vector2 chunkPos = GlobalPosToChunkCoord(globalPos);
            Chunk chunk = Chunks[chunkPos];
            RenderChunk renderChunk = chunk.GetRenderChunkByCoord(globalPos);
            if (renderChunk.AddBlock(globalPos, voxelType))
            {
                UpdateRenderChunkQueue.Enqueue(renderChunk);
                return true;
            }
            return false;
        }

        public bool RemoveBlock(Vector3 globalPos)
        {
            Vector2 chunkPos = GlobalPosToChunkCoord(globalPos);
            Chunk chunk = Chunks[chunkPos];
            RenderChunk renderChunk = chunk.GetRenderChunkByCoord(globalPos);
            if (renderChunk.RemoveBlock(globalPos))
            {
                UpdateRenderChunkQueue.Enqueue(renderChunk);
                return true;
            }
            return false;
        }

		public bool PositionInFrustum(Vector3 position, float radius) {

			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(ThisCamera);

			for (int i = 0; i < 6; i++) {
				Vector3 norm = planes[i].normal;
				if (norm.x * position.x + norm.y * position.y + norm.z * position.z + planes[i].distance <= -radius) {
					return false;
				}
			}

			return true;
		}

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

		public static Vector2 GlobalPosToChunkCoord(Vector3 globalPos) {
			return new Vector2(Mathf.Floor(globalPos.x/16), Mathf.Floor(globalPos.z/16));
		}
	}
}
