using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VoxelEngine {
	class VoxelMeshGenerator {
		Vector3[] Vertecies;
		Vector3[] Normals;
		Vector2[] UVs;
		int[] Triangles;

		int VertexCount;
		int TriCount;
		int NumSides;

		int length = 1, width = 1, height = 1;
		List<Vector3> VertexList;
		List<Vector3> NormalList;
		List<Vector2> UVList;
		List<int> TriangleList;

		// Vertecies
		Vector3 p0, p1, p2, p3, p4, p5, p6, p7;
		// Normals
		Vector3 up = Vector3.up;
		Vector3 down = Vector3.down;
		Vector3 front = Vector3.forward;
		Vector3 back = Vector3.back;
		Vector3 left = Vector3.left;
		Vector3 right = Vector3.right;
		//UVs
		Vector2 _00 = new Vector2(0f, 0f);
		Vector2 _10 = new Vector2(1f, 0f);
		Vector2 _01 = new Vector2(0f, 1f);
		Vector2 _11 = new Vector2(1f, 1f);
		Vector2 _16thUp = new Vector2(0, .0625f);
		Vector2 _16thRight = new Vector2(.0625f, 0);

		public VoxelMeshGenerator() {
			VertexList = new List<Vector3>();
			NormalList = new List<Vector3>();
			UVList = new List<Vector2>();
			TriangleList = new List<int>();
			VertexCount = 0;
			TriCount = 0;
			NumSides = 0;

			p0 = new Vector3(-length * .5f, -width * .5f, height * .5f);
			p1 = new Vector3(length * .5f, -width * .5f, height * .5f);
			p2 = new Vector3(length * .5f, -width * .5f, -height * .5f);
			p3 = new Vector3(-length * .5f, -width * .5f, -height * .5f);
			p4 = new Vector3(-length * .5f, width * .5f, height * .5f);
			p5 = new Vector3(length * .5f, width * .5f, height * .5f);
			p6 = new Vector3(length * .5f, width * .5f, -height * .5f);
			p7 = new Vector3(-length * .5f, width * .5f, -height * .5f);
		}

		public Mesh GenerateMesh() {
			Mesh mesh = new Mesh();
			RemoveDuplicateVerts();
			mesh.vertices = VertexList.ToArray();
			mesh.normals = NormalList.ToArray();
			mesh.uv = UVList.ToArray();
			mesh.triangles = TriangleList.ToArray();
			mesh.RecalculateBounds();
			return mesh;
		}

		public void RemoveDuplicateVerts () {

		}

		public void AddLeftFace(int x, int y, int z, VoxelType vt) {
			Vector3 localPos = new Vector3(x, y, z);
			VertexList.Add(p7 + localPos);
			VertexList.Add(p4 + localPos);
			VertexList.Add(p0 + localPos);
			VertexList.Add(p3 + localPos);

			NormalList.Add(left);
			NormalList.Add(left);
			NormalList.Add(left);
			NormalList.Add(left);

            Vector2 uvLower = vt.LeftUV;

			UVList.Add(uvLower + _16thRight + _16thUp);
			UVList.Add(uvLower + _16thUp);
			UVList.Add(uvLower);
			UVList.Add(uvLower + _16thRight);

			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);
			TriangleList.Add(0 + 4 * NumSides);
			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(2 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);

			NumSides++;
		}

		public void AddRightFace (int x, int y, int z, VoxelType vt) {
			Vector3 localPos = new Vector3(x, y, z);
			VertexList.Add(p5 + localPos);
			VertexList.Add(p6 + localPos);
			VertexList.Add(p2 + localPos);
			VertexList.Add(p1 + localPos);

			NormalList.Add(right);
			NormalList.Add(right);
			NormalList.Add(right);
			NormalList.Add(right);

            Vector2 uvLower = vt.RightUV;

			UVList.Add(uvLower + _16thRight + _16thUp);
			UVList.Add(uvLower + _16thUp);
			UVList.Add(uvLower);
			UVList.Add(uvLower + _16thRight);

			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);
			TriangleList.Add(0 + 4 * NumSides);
			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(2 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);

			NumSides++;
		}

		public void AddTopFace (int x, int y, int z, VoxelType vt) {
			Vector3 localPos = new Vector3(x, y, z);
			VertexList.Add(p7 + localPos);
			VertexList.Add(p6 + localPos);
			VertexList.Add(p5 + localPos);
			VertexList.Add(p4 + localPos);

			NormalList.Add(up);
			NormalList.Add(up);
			NormalList.Add(up);
			NormalList.Add(up);

            Vector2 uvLower = vt.TopUV;

			UVList.Add(uvLower + _16thRight + _16thUp);
			UVList.Add(uvLower + _16thUp);
			UVList.Add(uvLower);
			UVList.Add(uvLower + _16thRight);

			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);
			TriangleList.Add(0 + 4 * NumSides);
			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(2 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);

			NumSides++;
		}

		public void AddBottomFace (int x, int y, int z, VoxelType vt) {
			Vector3 localPos = new Vector3(x, y, z);
			VertexList.Add(p0 + localPos);
			VertexList.Add(p1 + localPos);
			VertexList.Add(p2 + localPos);
			VertexList.Add(p3 + localPos);

			NormalList.Add(down);
			NormalList.Add(down);
			NormalList.Add(down);
			NormalList.Add(down);

			Vector2 uvLower = vt.BottomUV;

			UVList.Add(uvLower + _16thRight + _16thUp);
			UVList.Add(uvLower + _16thUp);
			UVList.Add(uvLower);
			UVList.Add(uvLower + _16thRight);

			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);
			TriangleList.Add(0 + 4 * NumSides);
			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(2 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);

			NumSides++;
		}

		public void AddFrontFace (int x, int y, int z, VoxelType vt) {
			Vector3 localPos = new Vector3(x, y, z);
			VertexList.Add(p4 + localPos);
			VertexList.Add(p5 + localPos);
			VertexList.Add(p1 + localPos);
			VertexList.Add(p0 + localPos);

			NormalList.Add(front);
			NormalList.Add(front);
			NormalList.Add(front);
			NormalList.Add(front);

            Vector2 uvLower = vt.FrontUV;

			UVList.Add(uvLower + _16thRight + _16thUp);
			UVList.Add(uvLower + _16thUp);
			UVList.Add(uvLower);
			UVList.Add(uvLower + _16thRight);

			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);
			TriangleList.Add(0 + 4 * NumSides);
			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(2 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);

			NumSides++;
		}

		public void AddBackFace (int x, int y, int z, VoxelType vt) {
			Vector3 localPos = new Vector3(x, y, z);
			VertexList.Add(p6 + localPos);
			VertexList.Add(p7 + localPos);
			VertexList.Add(p3 + localPos);
			VertexList.Add(p2 + localPos);

			NormalList.Add(back);
			NormalList.Add(back);
			NormalList.Add(back);
			NormalList.Add(back);

            Vector2 uvLower = vt.BackUV;

			UVList.Add(uvLower + _16thRight + _16thUp);
			UVList.Add(uvLower + _16thUp);
			UVList.Add(uvLower);
			UVList.Add(uvLower + _16thRight);

			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);
			TriangleList.Add(0 + 4 * NumSides);
			TriangleList.Add(3 + 4 * NumSides);
			TriangleList.Add(2 + 4 * NumSides);
			TriangleList.Add(1 + 4 * NumSides);

			NumSides++;
		}
	}
}
