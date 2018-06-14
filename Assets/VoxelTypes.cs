using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VoxelEngine {
	enum VoxelTypes : int {
		None,
		Grass,
		Stone
	}

	public static class VoxelUVs {
		public static Vector2 GrassTop = new Vector2(0, .9375f);
		public static Vector2 Stone = new Vector2(.0625f, .9375f);
		public static Vector2 Dirt = new Vector2(.125f, .9375f);
		public static Vector2 GrassSide = new Vector2(.1875f, .9375f);
	}
}
