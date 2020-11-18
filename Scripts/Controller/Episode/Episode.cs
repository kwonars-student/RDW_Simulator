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

    public Episode() {
        id = totalID++;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
        this.episodeLength = 0;
    }

    public Episode(int episodeLength)
    {
        id = totalID++;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
        this.episodeLength = episodeLength;
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

    protected virtual void GenerateEpisode(Transform2D virtualUserTransform, Space2D virtualSpace) {}

    protected void InstaniateTarget() {
        targetObject = GameObject.Instantiate(targetPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Virtual Space").transform);
        targetObject.transform.localPosition = Utility.Cast2Dto3D(currentTargetPosition.Value) + new Vector3(0, 1, 0);
    }

    public bool IsNotEnd() {
        if (currentEpisodeIndex < episodeLength)
            return true;
        else
            return false;
    }

    public Vector2 GetTarget(Transform2D virtualUserTransform, Space2D virtualSpace) {
        if (!currentTargetPosition.HasValue) {
            GenerateEpisode(virtualUserTransform, virtualSpace);
            //InstaniateTarget();
        }

        return currentTargetPosition.Value;
    }

    public void DeleteTarget() {
        GameObject.Destroy(targetObject);
        currentEpisodeIndex += 1;
        currentTargetPosition = null;
        //RedirectedUnit.debugTargetPositionList.Clear();
        //RedirectedUnit.debugVirtualPositionList.Clear();
        //RedirectedUnit.debugRealPositionList.Clear();
        //RDWSimulationManager.remainTime = 0;
    }
}
