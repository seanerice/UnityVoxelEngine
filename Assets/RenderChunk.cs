using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VoxelEngine {
	public class RenderChunk {
		public Mesh mesh { get; set; }
		public readonly Vector3 LowerGlobalCoord;

		// Private
		private Vector3 RenderChunkSize = new Vector3(16, 16, 16);
		private Voxel[,,] Voxels;
		public GameObject RenderObject;
		private Mesh RenderMesh;

		// Constructor
		public RenderChunk (Vector3 lowerCoord) {
			// Field initialization
			RenderChunkSize = new Vector3(16, 16, 16);
			LowerGlobalCoord = lowerCoord;
			Voxels = new Voxel[(int)RenderChunkSize.x, (int)RenderChunkSize.y, (int)RenderChunkSize.z];
			InitializeEmptyVoxels();
		}

		public void RefreshChunkMesh () {
			mesh = MeshEditor.MeshFromVoxel16x16x16(Voxels);
		}

		public void InitializeGameObject (Material mat) {
			GameObject go = new GameObject("Chunk");
			go.transform.position = LowerGlobalCoord;
			MeshFilter mf = go.AddComponent<MeshFilter>();
			MeshRenderer mr = go.AddComponent<MeshRenderer>();
			MeshCollider mc = go.AddComponent<MeshCollider>();
			mc.sharedMesh = mesh;
			mf.mesh = mesh;
			mr.material = mat;

			GameObject.Destroy(RenderObject);
			RenderObject = go;
		}

		public void InitializeEmptyVoxels () {
			for (int x = 0; x < RenderChunkSize.x; x++) {
				for (int y = 0; y < RenderChunkSize.y; y++) {
					for (int z = 0; z < RenderChunkSize.z; z++) {
						Voxels[x, y, z] = new Voxel();
						Voxels[x, y, z].VoxelType = (int)VoxelTypes.None;
						Voxels[x, y, z].GlobalPosition = LowerGlobalCoord + new Vector3(x, y, z);
						Voxels[x, y, z].LocalPosition = new Vector3(x, y, z);
					}
				}
			}
		}

		public void GenerateProceduralTerrain(int seed, int variance, float threshold, float scale, int grassThickness) {
			if (LowerGlobalCoord.y <= 48) {
				GenerateHeightMapTerrain(seed, scale, threshold, variance);
				GenerateCaves(seed, scale, threshold);
			}
		}

		public int[,] GenerateHeightmapTopology (int seed, int variance, float scale) {
			int[,] topologyHeightmap = new int[16, 16];
			for (int x = 0; x < RenderChunkSize.x; x++) {
				for (int z = 0; z < RenderChunkSize.z; z++) {
					Vector3 globalPos = LocalToGlobal(new Vector3(x, 0, z));
					int height = (int)(Mathf.PerlinNoise((globalPos.x + 1000 + seed) * scale, (globalPos.z + 1000 + seed) * scale) * variance);
					topologyHeightmap[x, z] = height;
					Voxels[x, height, z].VoxelType = (int)VoxelTypes.Grass;
				}
			}
			return topologyHeightmap;
		}

		public void GenerateCaves (int seed, float scale, float threshold) {
			for (int x = 0; x < RenderChunkSize.x; x++) {
				for (int z = 0; z < RenderChunkSize.z; z++) {
					for (int y = 0; y < RenderChunkSize.y; y++) {
						Vector3 globalPos = Voxels[x,y,z].GlobalPosition;
						if (PerlinNoise3D((globalPos.x + 1000 + seed)*scale,(globalPos.y + 1000 + seed) * scale, (globalPos.z + 1000 + seed) * scale) > threshold) {
							Voxels[x, y, z].VoxelType = (int)VoxelTypes.None;
							//Debug.Log(globalPos);
						}
					}
				}
			}
		}

		public void GenerateHeightMapTerrain (int seed, float scale, float threshold, int variance, int grassLayerThickness = 5) {
			int[,] topologyHeightMap = GenerateHeightmapTopology(seed, variance, scale);
			for (int x = 0; x < RenderChunkSize.x; x++) {
				for (int z = 0; z < RenderChunkSize.z; z++) {
					if (LowerGlobalCoord.y == 48) {
						for (int y = 0; y < topologyHeightMap[x, z]; y++) {
							Voxels[x, y, z].VoxelType = (int)VoxelTypes.Stone;
						}
						for (int y = topologyHeightMap[x, z]; y < topologyHeightMap[x, z] + grassLayerThickness; y++) {
							Voxels[x, y, z].VoxelType = (int)VoxelTypes.Grass;
						}
					} else if (LowerGlobalCoord.y < 48){
						for (int y = 0; y < RenderChunkSize.y; y++) {
							Voxels[x, y, z].VoxelType = (int)VoxelTypes.Stone;
						}
					}
					
				}
			}
		}

		public Vector3 LocalToGlobal (Vector3 local) {
			return local + LowerGlobalCoord;
		}

		public Voxel GetVoxel (int x, int y, int z) {
			try {
				return Voxels[x, y, z];
			} catch (Exception) {
				Debug.Log("Probably out of bounds exception.");
			}
			return null;
		}

		public static float PerlinNoise3D (float x, float y, float z) {
			float xy = Mathf.PerlinNoise(x, y);
			float yz = Mathf.PerlinNoise(y, z);
			float xz = Mathf.PerlinNoise(x, z);

			float yx = Mathf.PerlinNoise(y, x);
			float zy = Mathf.PerlinNoise(z, y);
			float zx = Mathf.PerlinNoise(z, x);

			return (xy + yz + xz + yx + zy + zx) / 6f;
		}
	}
}
