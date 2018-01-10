using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelSystem.Demo {

    [RequireComponent (typeof(MeshFilter))]
	public class GPUDemo : MonoBehaviour {

		[SerializeField] protected Mesh mesh;
		[SerializeField] protected ComputeShader voxelizer;
		[SerializeField] protected int count = 32;

		void Start () {
			var data = GPUVoxelizer.Voxelize(voxelizer, mesh, count);
			GetComponent<MeshFilter>().sharedMesh = GPUVoxelizer.Build(data);
			data.Dispose();
		}

	}

}
