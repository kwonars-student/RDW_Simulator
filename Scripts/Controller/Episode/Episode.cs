using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Episode
{
    protected static int totalID = 0;
    protected int id;
    protected Vector2? currentTargetPosition;
    protected int currentEpisodeIndex;
    protected int episodeLength;
    public GameObject targetPrefab = null;
    protected GameObject targetObject = null;

    public Episode() { // 기본 생성자
        id = totalID++;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
        this.episodeLength = 0;
    }

    public Episode(int episodeLength) // 생성자
    {
        id = totalID++;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
        this.episodeLength = episodeLength;
    }

    public void ResetEpisode()
    {
        id = totalID++;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
    }

    public int GetCurrentEpisodeIndex()
    {
        return currentEpisodeIndex;
    }

    public int GetEpisodeLength()
    {
        return episodeLength;
    }

    public int getID()
    {
        return id;
    }

    protected void InstaniateTarget()
    {
        targetObject = GameObject.Instantiate(targetPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Virtual Space").transform); // TODO: Continous simulation을 할 때 삭제하는 기존 Virtual Space Object를 parent로 삼아서 target도 render 되지 않는 문제 발생
        targetObject.transform.localPosition = Utility.CastVector2Dto3D(currentTargetPosition.Value) + new Vector3(0, 1, 0);
    }

    public bool IsNotEnd()
    {
        if (currentEpisodeIndex < episodeLength)
            return true;
        else
            return false;
    }

    public void DeleteTarget()
    {
        GameObject.Destroy(targetObject);
        currentEpisodeIndex += 1;
        currentTargetPosition = null;
    }

    public void ReLocateTarget()
    {
        GameObject.Destroy(targetObject);
        currentTargetPosition = null;
    }

    public Vector2 GetTarget(Transform2D virtualUserTransform, Space2D virtualSpace)
    {
        if (!currentTargetPosition.HasValue)
        {
            GenerateEpisode(virtualUserTransform, virtualSpace);
            if(targetPrefab != null) InstaniateTarget();
        }

        return currentTargetPosition.Value;
    }

    protected virtual void GenerateEpisode(Transform2D virtualUserTransform, Space2D virtualSpace) { }
}
