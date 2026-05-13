using System.Collections.Generic;
using UnityEngine;

public class PipeTraceSettings
{
    public float stepSize = 0.001f;
    public float radius = 0.001f;
    public int maxSteps = 30;
    public GameObject prefab;
}

public static class MeshFilterExtensions
{
    public static void PipeTraceStable(this MeshFilter meshFilter, Transform parent, PipeTraceSettings settings)
    {
        var mesh = meshFilter.sharedMesh;
        var t = meshFilter.transform;

        List<Vector3> vertices = new List<Vector3>();

        foreach (var v in mesh.vertices)
            vertices.Add(t.TransformPoint(v));

        // 🔥 bessere Startwahl: Extrempunkt entlang Hauptachse
        Vector3 globalAxis = PCAUtility.GetPrincipalDirection(vertices);
        Vector3 start = GetExtreme(vertices, globalAxis, true);

        Vector3 current = start;
        Vector3 lastDirection = globalAxis;

        HashSet<Vector3> visited = new HashSet<Vector3>();

        for (int i = 0; i < settings.maxSteps; i++)
        {
            var localPoints = GetNearby(vertices, current, settings.radius);

            // 🔥 NEU: nur Punkte vor uns behalten
            localPoints = localPoints.FindAll(p =>
            {
                Vector3 dirToPoint = (p - current).normalized;
                return Vector3.Dot(dirToPoint, lastDirection) > 0.2f; // nur vorwärts!
            });

            if (localPoints.Count < 10)
                break;

            // 🔥 Mittelpunkt
            Vector3 center = GetAverage(localPoints);

            // 🔥 Fortschritt erzwingen (kein Rückspringen)
            if (Vector3.Distance(center, current) < settings.stepSize * 0.3f)
            {
                current += lastDirection * settings.stepSize;
                continue;
            }

            // 🔥 Spawn
            if (settings.prefab != null)
            {
                var obj = Object.Instantiate(settings.prefab, center, Quaternion.identity, parent);
                obj.name = "Point_" + i;
            }

            // 🔥 lokale PCA
            Vector3 direction = PCAUtility.GetPrincipalDirection(localPoints);

            // 🔥 Flip verhindern
            if (Vector3.Dot(direction, lastDirection) < 0)
                direction = -direction;

            direction = Vector3.Slerp(lastDirection, direction, 0.3f).normalized;

            lastDirection = direction;

            // 🔥 Fortschritt erzwingen entlang letzter Richtung
            Vector3 forwardStep = lastDirection * settings.stepSize;

            // Projektion des neuen Centers auf Vorwärtsrichtung
            Vector3 toCenter = center - current;
            float forwardAmount = Vector3.Dot(toCenter, lastDirection);

            if (forwardAmount < settings.stepSize * 0.2f)
            {
                // ❗ Wenn wir nicht vorwärts kommen → zwinge Schritt
                current += forwardStep;
            }
            else
            {
                current = current + lastDirection * forwardAmount;
            }

            // 🔥 visited (optional simpel)
            visited.Add(center);
        }
    }

    static Vector3 GetExtreme(List<Vector3> points, Vector3 axis, bool min)
    {
        float best = min ? float.MaxValue : float.MinValue;
        Vector3 result = points[0];

        foreach (var p in points)
        {
            float proj = Vector3.Dot(p, axis);

            if (min && proj < best)
            {
                best = proj;
                result = p;
            }
            else if (!min && proj > best)
            {
                best = proj;
                result = p;
            }
        }

        return result;
    }

    static Vector3 FindStartPoint(List<Vector3> verts)
    {
        Vector3 min = verts[0];

        foreach (var v in verts)
        {
            if (v.y < min.y) // oder x/z je nach Fall
                min = v;
        }

        return min;
    }

    static List<Vector3> GetNearby(List<Vector3> verts, Vector3 pos, float r)
    {
        List<Vector3> result = new List<Vector3>();
        float r2 = r * r;

        foreach (var v in verts)
        {
            if ((v - pos).sqrMagnitude < r2)
                result.Add(v);
        }

        return result;
    }

    static Vector3 GetAverage(List<Vector3> points)
    {
        Vector3 sum = Vector3.zero;

        foreach (var p in points)
            sum += p;

        return sum / points.Count;
    }

    static Vector3 EstimateDirection(List<Vector3> verts, Vector3 center, float radius)
    {
        Vector3 dir = Vector3.zero;

        foreach (var v in verts)
        {
            Vector3 d = v - center;
            float dist = d.magnitude;

            if (dist > radius * 0.8f && dist < radius * 1.5f)
            {
                dir += d.normalized;
            }
        }

        return dir.normalized;
    }
}

internal class PCAUtility
{
    internal static Vector3 GetPrincipalDirection(List<Vector3> points)
    {
        if (points == null || points.Count < 3)
            return Vector3.forward;

        // 1. Mittelpunkt
        Vector3 mean = Vector3.zero;
        foreach (var p in points)
            mean += p;
        mean /= points.Count;

        // 2. Kovarianzmatrix berechnen
        float xx = 0, xy = 0, xz = 0;
        float yy = 0, yz = 0, zz = 0;

        foreach (var p in points)
        {
            Vector3 d = p - mean;

            xx += d.x * d.x;
            xy += d.x * d.y;
            xz += d.x * d.z;

            yy += d.y * d.y;
            yz += d.y * d.z;

            zz += d.z * d.z;
        }

        // 3. Power Iteration → größter Eigenvektor
        Vector3 direction = Vector3.right;

        for (int i = 0; i < 10; i++) // Iterationen
        {
            Vector3 newDir = new Vector3(
                xx * direction.x + xy * direction.y + xz * direction.z,
                xy * direction.x + yy * direction.y + yz * direction.z,
                xz * direction.x + yz * direction.y + zz * direction.z
            );

            direction = newDir.normalized;
        }

        return direction.normalized;
    }
}