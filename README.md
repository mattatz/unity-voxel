unity-voxel
=====================

Voxelize mesh algorithm in Unity. (includes GPU and CPU voxelizers.)

![Demo](https://raw.githubusercontent.com/mattatz/unity-voxel/master/Captures/Demo.gif)

## GPUVoxelParticleSystem

![GPUVoxelParticleSystem](https://raw.githubusercontent.com/mattatz/unity-voxel/master/Captures/GPUVoxelParticleSystem.gif)

the Demo for GPU Particle with geometry shader. (only tested on windows10 (GTX 1060))

Inspired by Keijiro Takahashi works⚡️ [StandardGeometryShader](https://github.com/keijiro/StandardGeometryShader) & [KvantSpray]( https://github.com/keijiro/KvantSpray)

## GPUVoxelMosaic

![GPUVoxelMosaicLevels](https://raw.githubusercontent.com/mattatz/unity-voxel/master/Captures/GPUVoxelMosaicLevels.gif)

![GPUVoxelMosaic](https://raw.githubusercontent.com/mattatz/unity-voxel/master/Captures/GPUVoxelMosaic.gif)

the Demo to update the resolution of voxels in realtime. (only tested on windows10 (GTX 1060))

## GPUVoxelSkinnedMesh

![GPUVoxelSkinnedMesh](https://raw.githubusercontent.com/mattatz/unity-voxel/master/Captures/GPUVoxelSkinnedMesh.gif)

Sample a mesh from SkinnedRenderer in every frame and voxelize it in realtime.

the human model and animation from [asset store](https://assetstore.unity.com/packages/3d/animations/raw-mocap-data-for-mecanim-5330).

## Usage

with GPU Voxelizer (recommended)
```cs
GPUVoxelData data = GPUVoxelizer.Voxelize(
    voxelizer,  // ComputeShader (Voxelizer.compute)
    mesh,       // a target mesh
    64,         // # of voxels for largest AABB bounds
    true        // flag to fill in volume or not; if set flag to false, sample a surface only
);

// build voxel cubes integrated mesh
GetComponent<MeshFilter>().sharedMesh = VoxelMesh.Build(data.GetData(), data.UnitLength, useUV);

// build 3D texture represent a volume by voxels.
RenderTexture volumeTexture = GPUVoxelizer.BuildTexture3D(
  voxelizer,
  data,
  texture,    // Texture2D to color voxels based on uv coordinates in voxels
  RenderTextureFormat.ARGBFloat,
  FilterMode.Bilinear
);

// need to release a voxel buffer
data.Dispose();
```

with CPU Voxelizer
```cs
// Voxelize target mesh with CPU Voxelizer

List<Voxel> voxels = CPUVoxelizer.Voxelize(
    mesh,   // a target mesh
    20      // # of voxels for largest AABB bounds
);
```

## Compatibility

Tested on Unity 2018.3.0f2, windows10 (GTX 1060), macOS (metal, not compatible with GPU Particle Demo).

## Sources

- Triangle mesh voxelization / Wolfire Games Blog - http://blog.wolfire.com/2009/11/Triangle-mesh-voxelization

- Möller–Trumbore intersection algorithm - https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm

- keijiro/StandardGeometryShader - https://github.com/keijiro/StandardGeometryShader

- keijiro/KvantSpray - https://github.com/keijiro/KvantSpray

- Post Processing Stack - https://www.assetstore.unity3d.com/jp/#!/content/83912
