using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static Vector3 Cast2Dto3D(Vector2 vec2) {
        return new Vector3(vec2.x, 0, vec2.y);
    }

    public static Vector2 Cast3Dto2D(Vector3 vec3) {
        return new Vector2(vec3.x, vec3.z);
    }

    public static Vector2 rotateVector2(Vector2 vec, float degree)
    {
        float _x = vec.x * Mathf.Cos(degree) - vec.y * Mathf.Sin(degree);
        float _y = vec.x * Mathf.Sin(degree) + vec.y * Mathf.Cos(degree);
        return new Vector2(_x, _y);
    }

    public static float sampleUniform(float min, float max) {
        //return a + Random.value * (b - a);
        return Random.Range(min, max);
    }

    public static float sampleNormal(float mu = 0, float sigma = 1, float min = float.MinValue, float max = float.MaxValue) {
        // From: http://stackoverflow.com/questions/218060/random-gaussian-variables
        float r1 = Random.value;
        float r2 = Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(r1)) * Mathf.Sin(2.0f * Mathf.PI * r2); // Random Normal(0, 1)
        float randNormal = mu + randStdNormal * sigma;
        return Mathf.Max(Mathf.Min(randNormal, max), min);
    }

    public static float GetSignedAngle(Vector3 prevDir, Vector3 currDir) {
        return Mathf.Sign(Vector3.Cross(prevDir, currDir).y) * Vector3.Angle(prevDir, currDir);
    }
}
