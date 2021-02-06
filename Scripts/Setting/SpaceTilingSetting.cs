using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceTilingSetting
{
    [Header("Tiling Function Check")]
    public bool useTiling;

    [Header("Common Setting")]
    public int areaWidth;
    public int areaHeight;

    [Header("Predefined Setting")]
    public string name = null;
    public GameObject predefinedSpace;
    
    public float tileLine1Height; // -100 to 100
    public float tileLine1Gradient; // -100 to 100
    public float tileLine2Height; // -100 to 100
    public float tileLine2Gradient; // -100 to 100

    [Header("Procedural Setting")]
    //public ObjectSetting spaceObjectSetting;
    public Vector2 position;
    public List<ObjectSetting> obstacleObjectSettings;

    public Space2D GetSpace(List<Vector2> vertices)
    {
        List<Vector2> tileVertices = new List<Vector2>();
        for(int i = 0 ; i < vertices.Count; i++)
        {
            tileVertices.Add(1f*vertices[i]);
        }


        bool useRegularPolygon = false;
        Object2D spaceObject = new Polygon2DBuilder().SetName(name).SetMode(useRegularPolygon).SetLocalPosition(position).SetVertices(tileVertices).Build();//spaceObjectSetting.GetObject();
        //Object2D spaceObject2 = new Polygon2DBuilder().SetName(name).SetMode(useRegularPolygon).SetLocalPosition(position+new Vector2(20f,0f)).SetVertices(tileVertices).Build();//spaceObjectSetting.GetObject();

        //spaceObject2.gameObject.transform.parent = spaceObject.gameObject.transform;
        List<Object2D> obstacles = new List<Object2D>();
        foreach (ObjectSetting obstacleObjectSetting in obstacleObjectSettings)
            obstacles.Add(obstacleObjectSetting.GetObject());

        return new Space2DBuilder().SetName(name).SetSpaceObject(spaceObject).SetObstacles(obstacles).Build();

    }
}
