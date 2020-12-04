using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static Vector3 Cast2Dto3D(Vector2 vec2, float height = 0) {
        return new Vector3(vec2.x, height, vec2.y);
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

    public static void SyncDirection(Object2D virtualUser, Object2D realUser, Vector2 virtualTargetDirection, Vector2 realTargetDirection) // 회전 오차로 인한 시뮬레이션 정확도 저하를 막기 위해 Direction을 동기화 시켜줌
    {
        if (virtualTargetDirection.magnitude > 1)
            virtualTargetDirection = virtualTargetDirection.normalized;
        if (realTargetDirection.magnitude > 1)
            realTargetDirection = realTargetDirection.normalized;

        //Debug.Log(string.Format("Before virtualUser.transform: {0}", virtualUser.transform));

        virtualUser.transform.forward = virtualTargetDirection;
        realUser.transform.forward = realTargetDirection;

        //Debug.Log(string.Format("After virtualUser.transform.forward: {0}", virtualUser.transform));

        if(virtualUser.gameObject != null) virtualUser.gameObject.transform.forward = Utility.Cast2Dto3D(virtualTargetDirection);
        if(realUser.gameObject != null) realUser.gameObject.transform.forward = Utility.Cast2Dto3D(realTargetDirection);
    }

    //public static void SyncPosition(Object2D virtualUser, Object2D realUser, float virtualTargetDistance, float realTargetDistance)
    //{
    //    virtualUser.transform.localPosition = virtualTargetPosition;
    //    realUser.transform.localPosition = realTargetPosition;

    //    virtualTargetPosition = virtualUser.transform.localPosition + virtualTargetDirection * initialDistance;
    //    realTargetPosition = realUser.transform.localPosition + realTargetDirection * initialDistance;

    //    if (virtualUser.gameObject != null) virtualUser.gameObject.transform.localPosition = Utility.Cast2Dto3D(virtualTargetPosition);
    //    if (realUser.gameObject != null) realUser.gameObject.transform.localPosition = Utility.Cast2Dto3D(realTargetPosition);
    //}

    public static void SyncPosition(Object2D virtualUser, Object2D realUser, Vector2 virtualTargetPosition, Vector2 realTargetPosition)
    {
        virtualUser.transform.localPosition = virtualTargetPosition;
        realUser.transform.localPosition = realTargetPosition;

        if (virtualUser.gameObject != null) virtualUser.gameObject.transform.localPosition = Utility.Cast2Dto3D(virtualTargetPosition);
        if (realUser.gameObject != null) realUser.gameObject.transform.localPosition = Utility.Cast2Dto3D(realTargetPosition);
    }

    public static float sampleUniform(float min, float max) {
        //return a + Random.value * (b - a);
        //Random.InitState(System.Convert.ToInt32(System.DateTime.Now.ToString("MMddHHmmss")));
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
