using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelSystem.Demo {

    [RequireComponent (typeof(MeshFilter))]
	public class GPUDemo : MonoBehaviour {

        enum MeshType {
            Volume, Surface
        };

        [SerializeField] MeshType type = MeshType.Volume;
		[SerializeField] protected Mesh mesh;
		[SerializeField] protected ComputeShader voxelizer;
		[SerializeField] protected int count = 32;
        [SerializeField] protected bool useUV = false;

		void Start () {
			var data = GPUVoxelizer.Voxelize(voxelizer, mesh, count, (type == MeshType.Volume));
			GetComponent<MeshFilter>().sharedMesh = GPUVoxelizer.Build(data, useUV);
			data.Dispose();
		}

	}

}
