﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VoxelEngine {
	public class RenderChunk {
		public readonly Vector3 LowerGlobalCoord;

		// Private
		private Vector3 RenderChunkSize = new Vector3(16, 16, 16);
		private Voxel[,,] Voxels;
		public GameObject RenderObject;
		public Mesh RenderMesh = new Mesh();
		private MeshFilter MeshFilt;
		private MeshCollider MeshColl;

		public bool MarkedForDestruction { get; set; }

		// Constructor
		public RenderChunk (Vector3 lowerCoord) {
			// Field initialization
			RenderChunkSize = new Vector3(16, 16, 16);
			LowerGlobalCoord = lowerCoord;
            //Debug.Log(LowerGlobalCoord);
			Voxels = new Voxel[(int)RenderChunkSize.x, (int)RenderChunkSize.y, (int)RenderChunkSize.z];
			InitializeEmptyVoxels();
		}

		public void RefreshChunkMesh () {
			if (!MarkedForDestruction) {
				Mesh.Destroy(RenderMesh);
				RenderMesh = MeshEditor.MeshFromVoxel16x16x16(Voxels);
				MeshFilt.mesh = RenderMesh;
				MeshColl.sharedMesh = RenderMesh;
			}
		}

		public void InitializeGameObject (Material mat) {
			if (!MarkedForDestruction) { 
				GameObject go = new GameObject("Chunk");
				go.transform.position = LowerGlobalCoord;
				MeshFilt = go.AddComponent<MeshFilter>();
				MeshRenderer mr = go.AddComponent<MeshRenderer>();
				MeshColl = go.AddComponent<MeshCollider>();

				MeshColl.sharedMesh = RenderMesh;
				MeshFilt.mesh = RenderMesh;
				mr.material = mat;
				Shader sh = go.GetComponent<Shader>();
				GameObject.Destroy(RenderObject);
				RenderObject = go;
			}
		}

		public void InitializeEmptyVoxels () {
			for (int x = 0; x < RenderChunkSize.x; x++) {
				for (int y = 0; y < RenderChunkSize.y; y++) {
					for (int z = 0; z < RenderChunkSize.z; z++) {
						Voxels[x, y, z] = new Voxel();
						Voxels[x, y, z].VoxelType = VoxelType.None;
						Voxels[x, y, z].GlobalPosition = new Vector3(LowerGlobalCoord.x + x, LowerGlobalCoord.y + y, LowerGlobalCoord.z + z);
						Voxels[x, y, z].LocalPosition = new Vector3(x, y, z);
					}
				}
			}
		}

		public void GenerateProceduralTerrain(int seed, int variance, float threshold, float scale, int grassThickness) {
			for (int x = 0; x < 16; x++) {
				for (int y = 0; y < 16; y++) {
					for (int z = 0; z < 16; z++) {
						Voxels[x, y, z].VoxelType = Terrain(new Vector3(x, y, z), seed, variance, scale, grassThickness, threshold);
					}
				}
			}
		}

		public VoxelType Terrain(Vector3 localPosition, int seed, int heightVariance, float scale, int grassThickness, float threshold) {
			Vector3 globalPos = LocalToGlobal(localPosition);
			int height = (int)(Mathf.PerlinNoise((globalPos.x + 1000 + seed) * scale, (globalPos.z + 1000 + seed) * scale) * heightVariance) + 48;
			if (globalPos.y >= height && globalPos.y <= height + grassThickness) {
				return VoxelType.Grass;
			} else if (globalPos.y < height) {
				return VoxelType.Stone;
				// Stone, caves, ores
			}
			return VoxelType.None;
		}

		//public int[,] GenerateHeightmapTopology (int seed, int variance, float scale) {
		//	int[,] topologyHeightmap = new int[16, 16];
		//	for (int x = 0; x < RenderChunkSize.x; x++) {
		//		for (int z = 0; z < RenderChunkSize.z; z++) {
		//			Vector3 globalPos = LocalToGlobal(new Vector3(x, 0, z));
		//			int height = (int)(Mathf.PerlinNoise((globalPos.x + 1000 + seed) * scale, (globalPos.z + 1000 + seed) * scale) * variance);
		//			topologyHeightmap[x, z] = height;
		//			Voxels[x, height, z].VoxelType = VoxelType.Grass;
		//		}
		//	}
		//	return topologyHeightmap;
		//}

		//public void GenerateCaves (int seed, float scale, float threshold) {
		//	for (int x = 0; x < RenderChunkSize.x; x++) {
		//		for (int z = 0; z < RenderChunkSize.z; z++) {
		//			for (int y = 0; y < RenderChunkSize.y; y++) {
		//				Vector3 globalPos = Voxels[x,y,z].GlobalPosition;
		//				if (PerlinNoise3D((globalPos.x + 1000 + seed)*scale,(globalPos.y + 1000 + seed) * scale, (globalPos.z + 1000 + seed) * scale) > threshold) {
		//					Voxels[x, y, z].VoxelType = VoxelType.None;
		//					//Debug.Log(globalPos);
		//				}
		//			}
		//		}
		//	}
		//}

		//public void GenerateHeightMapTerrain (int seed, float scale, float threshold, int variance, int grassLayerThickness = 5) {
		//	int[,] topologyHeightMap = GenerateHeightmapTopology(seed, variance, scale);
		//	for (int x = 0; x < RenderChunkSize.x; x++) {
		//		for (int z = 0; z < RenderChunkSize.z; z++) {
		//			if (LowerGlobalCoord.y == 48) {
		//				for (int y = 0; y < topologyHeightMap[x, z]; y++) {
		//					Voxels[x, y, z].VoxelType = VoxelType.Stone;
		//				}
		//				for (int y = topologyHeightMap[x, z]; y < topologyHeightMap[x, z] + grassLayerThickness; y++) {
		//					Voxels[x, y, z].VoxelType = VoxelType.Grass;
		//				}
		//			} else if (LowerGlobalCoord.y < 48){
		//				for (int y = 0; y < RenderChunkSize.y; y++) {
		//					Voxels[x, y, z].VoxelType = VoxelType.Stone;
		//				}
		//			}
					
		//		}
		//	}
		//}

        public bool AddBlock(Vector3 globalPosition, VoxelType voxelType)
        {
            Vector3 local = GlobalToIndex(globalPosition);
            if (local.x >= 0 && local.x < 16 && local.y >= 0 && local.y < 16 && local.z >= 0 && local.z < 16 && voxelType != VoxelType.None)    // Must be valid index, valid voxeltype
            {
                if (Voxels[(int)local.x, (int)local.y, (int)local.z].VoxelType == VoxelType.None)                                               // Add to only a null voxel
                {
                    Voxels[(int)local.x, (int)local.y, (int)local.z].VoxelType = voxelType;
                    return true;
                }   
            }
            return false;
        }

        public bool RemoveBlock(Vector3 globalPosition)
        {
            Vector3 local = GlobalToIndex(globalPosition);
            //Debug.Log(local);
            if (local.x >= 0 && local.x < 16 && local.y >= 0 && local.y < 16 && local.z >= 0 && local.z < 16)   // Valix index
            {
                if (Voxels[(int)local.x, (int)local.y, (int)local.z].VoxelType != VoxelType.None)               // Remove from a non-null voxel.
                {
                    Voxels[(int)local.x, (int)local.y, (int)local.z].VoxelType = VoxelType.None;
                    return true;
                }
            }
            return false;
        }

		public Vector3 LocalToGlobal (Vector3 local) {
			return local + LowerGlobalCoord;
		}

        public Vector3 GlobalToIndex(Vector3 globalPos)
        {
            //Debug.Log(LowerGlobalCoord);
            return globalPos - LowerGlobalCoord;
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
