using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.Rendering;

namespace VoxelSystem {

	public class GPUVoxelizer {

		protected const string kKernelKey = "Voxelize";
		protected const string kStartKey = "_Start", kEndKey = "_End", kSizeKey = "_Size";
		protected const string kUnitKey = "_Unit", kHalfUnitKey = "_HalfUnit";
		protected const string kWidthKey = "_Width", kHeightKey = "_Height", kDepthKey = "_Depth";
		protected const string kTriCountKey = "_TrianglesCount";
		protected const string kVertBufferKey = "_VertBuffer", kTriBufferKey = "_TriBuffer";
		protected const string kVoxelBufferKey = "_VoxelBuffer";

        public static int GetNearPow2(float n)
        {
            if(n <= 0) {
                return 0;
            }
            var k = Mathf.CeilToInt(Mathf.Log(n, 2));
            return (int)Mathf.Pow(2, k);
        }

		public static GPUVoxelData Voxelize(ComputeShader voxelizer, Mesh mesh, int count = 32, bool pow2 = false) {
			mesh.RecalculateBounds();

			var vertices = mesh.vertices;
			var vertBuffer = new ComputeBuffer(vertices.Length, Marshal.SizeOf(typeof(Vector3)));
			vertBuffer.SetData(vertices);

			var triangles = mesh.triangles;
			var triBuffer = new ComputeBuffer(triangles.Length, Marshal.SizeOf(typeof(int)));
			triBuffer.SetData(triangles);

			var bounds = mesh.bounds;
			var maxLength = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
			var unit = maxLength / count;
			var size = bounds.size;

            int w, h, d;
            if(!pow2)
            {
                w = Mathf.CeilToInt(size.x / unit);
                h = Mathf.CeilToInt(size.y / unit);
                d = Mathf.CeilToInt(size.z / unit);
            } else  {
                w = GetNearPow2(size.x / unit);
                h = GetNearPow2(size.y / unit);
                d = GetNearPow2(size.z / unit);
            }

			var voxelBuffer = new ComputeBuffer(w * h * d, Marshal.SizeOf(typeof(Voxel_t)));
			var kernel = new Kernel(voxelizer, kKernelKey);

			// send bounds
			voxelizer.SetVector(kStartKey, bounds.min);
			voxelizer.SetVector(kEndKey, bounds.max);
			voxelizer.SetVector(kSizeKey, bounds.size);

			voxelizer.SetFloat(kUnitKey, unit);
			voxelizer.SetFloat(kHalfUnitKey, unit * 0.5f);
			voxelizer.SetInt(kWidthKey, w);
			voxelizer.SetInt(kHeightKey, h);
			voxelizer.SetInt(kDepthKey, d);

			// send mesh data
			voxelizer.SetBuffer(kernel.Index, kVertBufferKey, vertBuffer);
			voxelizer.SetInt(kTriCountKey, triBuffer.count);
			voxelizer.SetBuffer(kernel.Index, kTriBufferKey, triBuffer);
			voxelizer.SetBuffer(kernel.Index, kVoxelBufferKey, voxelBuffer);

			voxelizer.Dispatch(kernel.Index, w / (int)kernel.ThreadX + 1, h / (int)kernel.ThreadY + 1, (int)kernel.ThreadZ);

			// dispose
			vertBuffer.Release();
			triBuffer.Release();

			return new GPUVoxelData(voxelBuffer, w, h, d, unit);
		}

		public static Mesh Build(GPUVoxelData data) {
			var vertices = new List<Vector3>();
			var uvs = new List<Vector2>();
			var triangles = new List<int>();
			var normals = new List<Vector3>();
			var centers = new List<Vector4>();

			var unit = data.UnitLength;

			var up = Vector3.up * unit;
			var hup = up * 0.5f;
			var hbottom = -hup;

			var right = Vector3.right * unit;
			var hright = right * 0.5f;

			var left = -right;
			var hleft = left * 0.5f;

			var forward = Vector3.forward * unit;
			var hforward = forward * 0.5f;
			var back = -forward;
			var hback = back * 0.5f;

			var voxels = data.GetData();
			for(int i = 0, n = voxels.Length; i < n; i++) {
				var v = voxels[i];
				if(v.flag) {
					var center = v.position;

					// back
					CalculatePlane(
						vertices, normals, centers, uvs, triangles,
						center, hback, right, up, Vector3.back
					);

					// right
					CalculatePlane(
						vertices, normals, centers, uvs, triangles,
						center, hright, forward, up, Vector3.right
					);

					// forward
					CalculatePlane(
						vertices, normals, centers, uvs, triangles,
						center, hforward, left, up, Vector3.forward
					);

					// left
					CalculatePlane(
						vertices, normals, centers, uvs, triangles,
						center, hleft, back, up, Vector3.left
					);

					// up
					CalculatePlane(
						vertices, normals, centers, uvs, triangles,
						center, hup, right, forward, Vector3.up
					);

					// down
					CalculatePlane(
						vertices, normals, centers, uvs, triangles,
						center, hbottom, right, back, Vector3.down
					);

				}
			}

			var mesh = new Mesh();
			mesh.indexFormat = IndexFormat.UInt32;
			mesh.vertices = vertices.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.normals = normals.ToArray();
			mesh.tangents = centers.ToArray();
			mesh.SetTriangles(triangles.ToArray(), 0);
			mesh.RecalculateBounds();
			return mesh;
		}

		public static void CalculatePlane (
			List<Vector3> vertices, List<Vector3> normals, List<Vector4> centers, List<Vector2> uvs, List<int> triangles,
			Vector3 center, Vector3 offset, Vector3 right, Vector3 up, Vector3 normal, int rSegments = 2, int uSegments = 2
		) {
			float rInv = 1f / (rSegments - 1);
			float uInv = 1f / (uSegments - 1);

			int triangleOffset = vertices.Count;

			var transformed = center + offset;
			for(int y = 0; y < uSegments; y++) {
				float ru = y * uInv;
				for(int x = 0; x < rSegments; x++) {
					float rr = x * rInv;
					vertices.Add(transformed + right * (rr - 0.5f) + up * (ru - 0.5f));
					normals.Add(normal);
					centers.Add(center);
					uvs.Add(new Vector2(rr, ru));
				}

				if(y < uSegments - 1) {
					var ioffset = y * rSegments + triangleOffset;
					for(int x = 0, n = rSegments - 1; x < n; x++) {
						triangles.Add(ioffset + x);
						triangles.Add(ioffset + x + rSegments);
						triangles.Add(ioffset + x + 1);

						triangles.Add(ioffset + x + 1);
						triangles.Add(ioffset + x + rSegments);
						triangles.Add(ioffset + x + 1 + rSegments);
					}
				}
			}
		}

	}

}

