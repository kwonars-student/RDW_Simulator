using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Episode
{
    protected static int totalID = 0;
    protected int id;
    protected SimulationController simulationController;
    protected Space2D virtualSpace;
    protected Vector2? currentTargetPosition;
    protected int currentEpisodeIndex;
    protected int episodeLength;
    protected GameObject targetPrefab = null;
    protected GameObject targetObject = null;

    public Episode() {
        id = totalID++;
        this.episodeLength = 0;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
        simulationController = null;
        virtualSpace = null;
    }

    public Episode(int episodeLength) {
        id = totalID++;
        this.episodeLength = episodeLength;
        currentEpisodeIndex = 0;
        currentTargetPosition = null;
        simulationController = null;
    }

    public int getID()
    {
        return id;
    }

    public void SetReferences(SimulationController simulationController, Space2D virtualSpace, GameObject targetPrefab) {
        this.simulationController = simulationController;
        this.virtualSpace = virtualSpace;
        this.targetPrefab = targetPrefab;
    }

    protected virtual void GenerateEpisode() {}

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

    public Vector3 GetTarget() {
        if (!currentTargetPosition.HasValue) {
            GenerateEpisode();
            InstaniateTarget();
        }

        return Utility.Cast2Dto3D(currentTargetPosition.Value);
    }

    public void DeleteTarget() {
        GameObject.Destroy(targetObject);
        currentEpisodeIndex += 1;
        currentTargetPosition = null;
    }
}
