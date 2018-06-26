using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelEngine {
	public class Voxel {
		public Vector3 GlobalPosition { get; set; }
		public Vector3 LocalPosition { get; set; }
		public VoxelType VoxelType { get; set; }

		public Voxel Up = null, Down = null, Left = null, Right = null, Front = null, Back = null;
		public OccludeCube Occlude = new OccludeCube (false);
		public bool Visited = false;
	}
}
