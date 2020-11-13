using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceSetting
{
    public ObjectSetting spaceObject;
    public List<ObjectSetting> obstacleObjects;
    private Space2D spaceInstance;

    //public bool usePredefinedSpace;
    //public bool usePredefinedOrigin;
    //public Vector2 originPosition; // simulationManager local 2D 좌표계 기준
    //public float originRotation;
    //public Vector2 size; // n각형으로 일반화 어떻게 시키지?

    public Space2D GetSpace2D()
    {
        if (spaceInstance == null) 
        {
            List<Object2D> obstacles = new List<Object2D>();
            foreach (ObjectSetting obstacleObject in obstacleObjects)
            {
                obstacles.Add(obstacleObject.GetObject());
            }

            Object2D space = spaceObject.GetObject();
            spaceInstance = new Space2D(space, obstacles);

        }

        return spaceInstance;
    }
}
