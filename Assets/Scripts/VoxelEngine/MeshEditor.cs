using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VoxelEngine {
	public class MeshEditor {
		public static Mesh MeshFromVoxel16x16x16(Voxel[,,] voxels) {
			if (voxels.GetLength(0) != 16 || voxels.GetLength(1) != 16 || voxels.GetLength(2) != 16)
				throw new Exception("Must pass a 16x16x16 Voxel array.");
			List<Voxel> VoxelsToRender = new List<Voxel>();
			OccludeCube[,,] renderVoxel = CreateOccludedMap16x16x16(voxels);
			VoxelMeshGenerator meshGenerator = new VoxelMeshGenerator();
			for (int x = 0; x < 16; x++) {
				for (int y = 0; y < 16; y++) {
					for (int z = 0; z < 16; z++) {
						if (voxels[x, y, z].VoxelType != VoxelType.None) {
							if (renderVoxel[x, y, z].left) meshGenerator.AddLeftFace(x, y, z, voxels[x, y, z].VoxelType);
							if (renderVoxel[x, y, z].right) meshGenerator.AddRightFace(x, y, z, voxels[x, y, z].VoxelType);
							if (renderVoxel[x, y, z].up) meshGenerator.AddTopFace(x, y, z, voxels[x, y, z].VoxelType);
							if (renderVoxel[x, y, z].down) meshGenerator.AddBottomFace(x, y, z, voxels[x, y, z].VoxelType);
							if (renderVoxel[x, y, z].front) meshGenerator.AddFrontFace(x, y, z, voxels[x, y, z].VoxelType);
							if (renderVoxel[x, y, z].back) meshGenerator.AddBackFace(x, y, z, voxels[x, y, z].VoxelType);
						}
					}
				}
			}
			return meshGenerator.GenerateMesh();
		}

		public static OccludeCube[,,] CreateOccludedMap16x16x16 (Voxel[,,] voxels) {
			if (voxels.GetLength(0) != 16 || voxels.GetLength(1) != 16 || voxels.GetLength(2) != 16)
				throw new Exception("Must pass a 16x16x16 Voxel array.");
			OccludeCube[,,] voxelIsRendered = new OccludeCube[16, 16, 16];

			for (int x = 0; x < 16; x++) {
				for (int y = 0; y < 16; y++) {
					for (int z = 0; z < 16; z++) {
						voxelIsRendered[x, y, z] = new OccludeCube();
						if (x > 0 && voxels[x-1,y,z].VoxelType != VoxelType.None)			// Left
							voxelIsRendered[x, y, z].left = false;
						else if (x == 0)
						if (x < 15 && voxels[x + 1, y, z].VoxelType != VoxelType.None)		// Right
							voxelIsRendered[x, y, z].right = false;
						if (y > 0 && voxels[x, y - 1, z].VoxelType != VoxelType.None) 
							voxelIsRendered[x, y, z].down = false;
						if (y < 15 && voxels[x, y + 1, z].VoxelType != VoxelType.None)
							voxelIsRendered[x, y, z].up = false;
						if (z > 0 && voxels[x, y, z - 1].VoxelType != VoxelType.None)
							voxelIsRendered[x, y, z].back = false;
						if (z < 15 && voxels[x, y, z + 1].VoxelType != VoxelType.None)
							voxelIsRendered[x, y, z].front = false;
					}
				}
			}
			return voxelIsRendered;
		}

		//public static bool[,,] CreateOccludedMap16x16x16 (Voxel[,,] voxels) {
		//	if (voxels.GetLength(0) != 16 || voxels.GetLength(1) != 16 || voxels.GetLength(2) != 16)
		//		throw new Exception("Must pass a 16x16x16 Voxel array.");
		//	bool[,,] voxelIsRendered = new bool[16, 16, 16];
		//	for (int x = 0; x < 16; x++) {
		//		for (int y = 0; y < 16; y++) {
		//			for (int z = 0; z < 16; z++) {
		//				voxelIsRendered[x, y, z] = true;
		//				if (x > 0 && x < 15 && voxels[x + 1, y, z].VoxelType != (int)VoxelTypes.None && voxels[x - 1, y, z].VoxelType != (int)VoxelTypes.None
		//					&& y > 0 && y < 15 && voxels[x, y + 1, z].VoxelType != (int)VoxelTypes.None && voxels[x, y - 1, z].VoxelType != (int)VoxelTypes.None
		//					&& z > 0 && z < 15 && voxels[x, y, z + 1].VoxelType != (int)VoxelTypes.None && voxels[x , y, z - 1].VoxelType != (int)VoxelTypes.None)
		//					voxelIsRendered[x,y,z] = false; 
		//			}
		//		}
		//	}
		//	return voxelIsRendered;
		//}

		//public static bool[,,] CreateOccludedMap16x16x16 (Voxel[,,] voxels) {
		//	if (voxels.GetLength(0) != 16 || voxels.GetLength(1) != 16 || voxels.GetLength(2) != 16)
		//		throw new Exception("Must pass a 16x16x16 Voxel array.");
		//	bool[,,] voxelIsRendered = new bool[16, 16, 16];


		//	int zlo = 0, zhi = 15;
		//	int xlo = 0, xhi = 15;
		//	int ylo = 0, yhi = 15;
		//	for (int i = 0; i < 8; i++) {
		//		int renderedVoxelCount = 0;
		//		for (int x = 0; x < 16; x++) {			// Front and back sides (x,y)
		//			for (int y = 0; y < 16; y++) {
		//				if (voxels[x,y,zlo].VoxelType != (int)VoxelTypes.None) {
		//					if (zlo == 0 || (zlo > 0 && voxels[x,y,zlo-1].VoxelType == (int)VoxelTypes.None)) {
		//						voxelIsRendered[x, y, zlo] = true;
		//						renderedVoxelCount++;
		//					}
		//				}
		//				if (voxels[x, y, zhi].VoxelType != (int)VoxelTypes.None) {
		//					if (zhi == 15 || (zhi < 15 && voxels[x, y, zhi + 1].VoxelType == (int)VoxelTypes.None)) {
		//						voxelIsRendered[x, y, zhi] = true;
		//						renderedVoxelCount++;
		//					}
		//				}
		//			}
		//		}
		//		zlo++; zhi--;

		//		for (int y = 0; y < 16; y++) {          // Left and right sides (y,z)
		//			for (int z = 0; z < 16; z++) {
		//				if (voxels[xlo, y, z].VoxelType != (int)VoxelTypes.None) {
		//					if (xlo == 0 || (xlo > 0 && voxels[xlo - 1, y, z].VoxelType == (int)VoxelTypes.None)) {
		//						voxelIsRendered[xlo, y, z] = true;
		//						renderedVoxelCount++;
		//					}
		//				}
		//				if (voxels[xhi, y, z].VoxelType != (int)VoxelTypes.None) {
		//					if (xhi == 15 || (xhi < 15 && voxels[xhi + 1, y, z].VoxelType == (int)VoxelTypes.None)) {
		//						voxelIsRendered[xhi, y, z] = true;
		//						renderedVoxelCount++;
		//					}
		//				}
		//			}
		//		}
		//		xlo++; xhi--;

		//		for (int x = 0; x < 16; x++) {          // Top and bottom sides (x,z)
		//			for (int z = 0; z < 16; z++) {
		//				if (voxels[x, ylo, z].VoxelType != (int)VoxelTypes.None) {
		//					if (ylo == 0 || (ylo > 0 && voxels[x, ylo-1, z].VoxelType == (int)VoxelTypes.None)) {
		//						voxelIsRendered[x, ylo, z] = true;
		//						renderedVoxelCount++;
		//					}
		//				}
		//				if (voxels[x, yhi, z].VoxelType != (int)VoxelTypes.None) {
		//					if (yhi == 15 || (yhi < 15 && voxels[x, yhi+1, z].VoxelType == (int)VoxelTypes.None)) {
		//						voxelIsRendered[x, yhi, z] = true;
		//						renderedVoxelCount++;
		//					}
		//				}
		//			}
		//		}
		//		ylo++; yhi--;

		//		if (renderedVoxelCount == 0) return voxelIsRendered;
		//	}
		//	return voxelIsRendered;
		//}

	}

	public class OccludeCube {
		public OccludeCube() {
			up = true;
			down = true;
			left = true;
			right = true;
			front = true;
			back = true;
		}
		public bool up { get; set; }
		public bool down { get; set; }
		public bool left { get; set; }
		public bool right { get; set; }
		public bool front { get; set; }
		public bool back { get; set; }
	}
}
