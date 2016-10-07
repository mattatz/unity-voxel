unity-voxel
=====================

Voxelize mesh algorithm in Unity.

![Demo](https://raw.githubusercontent.com/mattatz/unity-voxel/master/Captures/Demo.png)

## Usage

```cs
// Voxelize target mesh
Mesh mesh = GetComponent<MeshFilter>().mesh;

List<Voxel> voxels = Voxelizer.Voxelize(mesh, 20); // 20 is # of voxel for largest AABB bounds
```

## Sources

- Triangle mesh voxelization / Wolfire Games Blog - http://blog.wolfire.com/2009/11/Triangle-mesh-voxelization

- Möller–Trumbore intersection algorithm - https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
