using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Episode
{
    protected static int totalID = 0;
    protected int id;
    //protected SimulationController simulationController;
    //protected Space2D virtualSpace;
    protected Vector2? currentTargetPosition;
    protected int currentEpisodeIndex;
    protected int episodeLength;
    protected GameObject targetPrefab = null;
    protected GameObject targetObject = null;

    public Episode() {
        id = totalID++;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
        this.episodeLength = 0;
        //simulationController = null;
        //virtualSpace = null;
    }

    public Episode(int episodeLength)
    {
        id = totalID++;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
        this.episodeLength = episodeLength;
    }

    //public Episode(Space2D virtualSpace, int episodeLength)
    //{
    //    id = totalID++;
    //    this.episodeLength = episodeLength;
    //    currentEpisodeIndex = 0;
    //    currentTargetPosition = null;
    //    simulationController = null;
    //    this.virtualSpace = virtualSpace;
    //}

    //public void SetReferences(SimulationController simulationController)
    //{
    //    this.simulationController = simulationController;
    //}

    //public Episode(int episodeLength) {
    //    id = totalID++;
    //    this.episodeLength = episodeLength;
    //    currentEpisodeIndex = 0;
    //    currentTargetPosition = null;
    //    simulationController = null;
    //}

    public int getID()
    {
        return id;
    }

    //public void SetReferences(SimulationController simulationController, Space2D virtualSpace, GameObject targetPrefab) {
    //    this.simulationController = simulationController;
    //    this.virtualSpace = virtualSpace;
    //    this.targetPrefab = targetPrefab;
    //}

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
    }
}
