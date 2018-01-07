using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace VoxelSystem {

    public class Voxel {
        public Vector3 position;
        public float size;
        public Voxel (Vector3 position, float size) {
            this.position = position;
            this.size = size;
        }
    }

    public class Voxelizer {

        public class Triangle {
            public Vector3 a, b, c;
            public Triangle (Vector3 a, Vector3 b, Vector3 c) {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            // https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
            public bool Hit (Ray ray, out float distance) {
                distance = -1f;

                var e1 = this.b - this.a;
                var e2 = this.c - this.a;
                var P = Vector3.Cross(ray.direction, e2);
                var det = Vector3.Dot(e1, P);

                if (det > -float.Epsilon && det < float.Epsilon) return false;

                float invDet = 1f / det;

                var T = ray.origin - this.a;
                var u = Vector3.Dot(T, P) * invDet;
                if (u < 0f || u > 1f) return false;

                var Q = Vector3.Cross(T, e1);
                var v = Vector3.Dot(ray.direction, Q * invDet);
                if (v < 0f || u + v > 1f) return false;

                var t = Vector3.Dot(e2, Q) * invDet;
                if(t > float.Epsilon) {
                    distance = t;
                    return true;
                }

                return false;
            }
        }

        public class HitResult {
            public Triangle triangle;
            public float distance;
            public HitResult(Triangle triangle, float distance) {
                this.triangle = triangle;
                this.distance = distance;
            }
        }

        // http://blog.wolfire.com/2009/11/Triangle-mesh-voxelization
        public static List<Voxel> Voxelize (Mesh mesh, int count = 10) {
            var voxels = new List<Voxel>();

            mesh.RecalculateBounds();
            var bounds = mesh.bounds;
            float maxLength = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            float unit = maxLength / count;

            var vertices = mesh.vertices;
            var indices = mesh.triangles;

            var triangles = new List<Triangle>();
            for(int i = 0, n = indices.Length; i < n; i += 3) {
                triangles.Add(
                    new Triangle(
                        vertices[indices[i]],
                        vertices[indices[i + 1]],
                        vertices[indices[i + 2]]
                    )
                );
            }

            var start = bounds.min;
            var end = bounds.max;

            var hunit = unit * 0.5f;
            for(float y = start.y; y <= end.y; y += unit) {
                for(float x = start.x; x <= end.x; x += unit) {
                    var ray = new Ray(new Vector3(x + hunit, y + hunit, start.z - hunit), Vector3.forward);
                    List<HitResult> results;
                    if(Hit(ray, triangles, out results)) {
                        voxels.AddRange(Build(ray, results, unit));
                    }
                }
            }

            return voxels;
        }

        static List<Voxel> Build (Ray ray, List<HitResult> results, float unit) {
            var voxels = new List<Voxel>();

            for (int i = 0, n = results.Count; i < n; i++) {
                if(i % 2 == 0) {
                    if(i == n - 1) { // last
                        voxels.Add(new Voxel(ray.origin + Grid(results[i].distance, unit) * ray.direction, unit));
                    }
                } else {
                    var from = Grid(results[i - 1].distance, unit);
                    var to = Grid(results[i].distance, unit);
                    for(float distance = from; distance < to; distance += unit) {
                        voxels.Add(new Voxel(ray.origin + distance * ray.direction, unit));
                    }
                }
            }

            return voxels;
        }

        static float Grid (float distance, float unit) {
            return Mathf.FloorToInt(distance / unit) * unit;
        }

        static bool Hit(Ray ray, List<Triangle> triangles, out List<HitResult> results) {
            results = new List<HitResult>();
            for (int i = 0, n = triangles.Count; i < n; i++) {
                float distance;
                if(triangles[i].Hit(ray, out distance)) {
                    results.Add(new HitResult(triangles[i], distance));
                }
            }
            results = results.OrderBy(r => r.distance).ToList();
            return results.Count > 0;
        }

    }

}


