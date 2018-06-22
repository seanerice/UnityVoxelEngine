using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VoxelEngine {

    public abstract class VoxelEnum : IComparable {
        public readonly int Id;
        public readonly Vector2 TopUV;
        public readonly Vector2 BottomUV;
        public readonly Vector2 LeftUV;
        public readonly Vector2 RightUV;
        public readonly Vector2 FrontUV;
        public readonly Vector2 BackUV;

        protected VoxelEnum () { }

        protected VoxelEnum (int id, Vector2 top, Vector2 bottom, Vector2 side)
        {
            Id = id;
            TopUV = top;
            BottomUV = bottom;
            LeftUV = side;
            RightUV = side;
            FrontUV = side;
            BackUV = side;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }

    public class VoxelType : VoxelEnum{
        public static VoxelType None = new VoxelType(0);
        public static VoxelType Dirt = new VoxelType(1, VoxelUVs.Dirt);
        public static VoxelType Grass = new VoxelType(2, VoxelUVs.GrassTop, VoxelUVs.Dirt, VoxelUVs.GrassSide);
        public static VoxelType Stone = new VoxelType(3, VoxelUVs.Stone);
        public static VoxelType Sand = new VoxelType(4);
        public static VoxelType Gravel = new VoxelType(5);
        public static VoxelType Wood = new VoxelType(6);
        public static VoxelType Glass = new VoxelType(7);

        protected VoxelType() { }

        public VoxelType(int id) : base (id, Vector2.zero, Vector2.zero, Vector2.zero) { }

        public VoxelType(int id, Vector2 top, Vector2 bottom, Vector2 side) : base (id, top, bottom, side) { }

        public VoxelType(int id, Vector2 all) : base(id, all, all, all) { }
    }



	public static class VoxelUVs {
		public static Vector2 GrassTop = new Vector2(0, .9375f);
		public static Vector2 Stone = new Vector2(.0625f, .9375f);
		public static Vector2 Dirt = new Vector2(.125f, .9375f);
		public static Vector2 GrassSide = new Vector2(.1875f, .9375f);
	}
}
