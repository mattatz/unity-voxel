using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace mattatz.VoxelSystem {

    [RequireComponent (typeof(MeshFilter))]
    public class Demo : MonoBehaviour {

        [SerializeField] int count = 10;
        List<Voxel> voxels;

        void Start () {
            var filter = GetComponent<MeshFilter>();
            voxels = Voxelizer.Voxelize(filter.mesh, count);
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


