using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace VoxelSystem.Demo {

    public class CPUDemo : MonoBehaviour {

		[SerializeField] protected Mesh mesh;
        [SerializeField] int count = 10;
        List<Voxel> voxels;

        void Start () {
            voxels = Voxelizer.Voxelize(mesh, count);
            voxels.ForEach(voxel => {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = transform;
                cube.transform.localPosition = voxel.position;
                cube.transform.localScale = voxel.size * Vector3.one;
                cube.transform.localRotation = Quaternion.identity;
            });
        }
        
        // void Update () {}

        void OnDrawGizmos () {
            if (voxels == null) return;

            Gizmos.matrix = transform.localToWorldMatrix;
            voxels.ForEach(voxel => {
                Gizmos.DrawCube(voxel.position, voxel.size * Vector3.one);
            });
        }

    }

}


