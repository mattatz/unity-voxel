unity-voxel
=====================

Voxelize mesh algorithm in Unity. (includes GPU and CPU voxelizers.)

![Demo](https://raw.githubusercontent.com/mattatz/unity-voxel/master/Captures/Demo.gif)

## GPUVoxelParticleSystem.scene

![GPUVoxelParticleSystem](https://raw.githubusercontent.com/mattatz/unity-voxel/master/Captures/GPUVoxelParticleSystem.gif)

the Demo for GPU Particle with Geometry shader. (only tested on windows10 (GTX 1060))

Inspired by Keijiro Takahashi works⚡️ [StandardGeometryShader](https://github.com/keijiro/StandardGeometryShader) & [KvantSpray]( https://github.com/keijiro/KvantSpray)

## Usage

with GPU Voxelizer (recommended)
```cs
GPUVoxelData data = GPUVoxelizer.Voxelize(
    voxelizer,  // ComputeShader (Voxelizer.compute)
    mesh,       // a target mesh
    64          // # of voxels for largest AABB bounds
);

// build voxel cubes integrated mesh
GetComponent<MeshFilter>().sharedMesh = GPUVoxelizer.Build(data);

// release a voxel buffer
data.Dispose();
```

with CPU Voxelizer
```cs
// Voxelize target mesh with CPU Voxelizer

List<Voxel> voxels = Voxelizer.Voxelize(
    mesh,   // a target mesh
    20      // # of voxels for largest AABB bounds
);
```

## Compatibility

tested on Unity 2017.0.3, windows10 (GTX 1060), macOS (metal).

## Sources

- Triangle mesh voxelization / Wolfire Games Blog - http://blog.wolfire.com/2009/11/Triangle-mesh-voxelization

- Möller–Trumbore intersection algorithm - https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm

- keijiro/StandardGeometryShader - https://github.com/keijiro/StandardGeometryShader

- keijiro/KvantSpray - https://github.com/keijiro/KvantSpray

- Post Processing Stack - https://www.assetstore.unity3d.com/jp/#!/content/83912
