using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpaceTilingSetting
{
    [Header("Tiling Function Check")]
    public bool tileMode;

    [Header("Common Setting")]
    public int Horizontal;
    public int Vertical;

    [Header("Predefined Setting")]
    public string name = null;
    public GameObject predefinedSpace;
    
    public float CrossingXAxis_Rotation; // -360 to +360
    public float CrossingYAxis_Rotation; // -360 to +360
    public Vector2 CrossingPointMovementInfo;

    [Header("Procedural Setting")]
    public ObjectSetting spaceObjectSetting;
    public Vector2 position;
    public List<ObjectSetting> obstacleObjectSettings;

    public Space2D GetSpace(List<Vector2> vertices)
    {
        bool useRegularPolygon = true;

        List<float> crossingAxisRotationInfo = new List<float>();
        crossingAxisRotationInfo.Add(CrossingXAxis_Rotation);
        crossingAxisRotationInfo.Add(CrossingYAxis_Rotation);

        List<int> tileAreaSetting = new List<int>();
        tileAreaSetting.Add(Horizontal);
        tileAreaSetting.Add(Vertical);

        Polygon2D basicTile = new Polygon2DBuilder().SetMode(!useRegularPolygon).SetTileMode(tileMode).SetLocalPosition(Vector2.zero).SetCrossingInfo(crossingAxisRotationInfo, CrossingPointMovementInfo).SetVertices(vertices).Build();
        List<Vector2> crossBoundaryPoints = basicTile.CalculateCrossBoundaryPoints();

        // Vector2 a = new Vector2(2.5f,0f);
        // Vector2 b = new Vector2(0f,1f);
        // Vector2 c = new Vector2(-2.5f,0f);
        // Vector2 d = new Vector2(0f,-1.8f);

        Vector2 a = crossBoundaryPoints[0]; // 오
        Vector2 b = crossBoundaryPoints[1]; // 위
        Vector2 c = crossBoundaryPoints[2]; // 왼
        Vector2 d = crossBoundaryPoints[3]; // 아래
        // Debug.Log(a);
        // Debug.Log(b);
        // Debug.Log(c);
        // Debug.Log(d);

        List<Vector2> tileCrossingVectors = new List<Vector2>();
        tileCrossingVectors.Add(a);
        tileCrossingVectors.Add(b);
        tileCrossingVectors.Add(c);
        tileCrossingVectors.Add(d);
        
        List<Vector2> flippedVertices = new List<Vector2>();
        for(int k = 0 ; k < vertices.Count; k++)
        {
            flippedVertices.Add(-1f*vertices[k]);
        }

        List<int> tileTypeIndex = new List<int>();
        tileTypeIndex.Add(0);
        tileTypeIndex.Add(1);
        tileTypeIndex.Add(2);
        tileTypeIndex.Add(3);


        // Object2D tile1 = new Polygon2DBuilder().SetName(name+"1").SetMode(!useRegularPolygon).SetLocalPosition(position).SetVertices(vertices).Build();//spaceObjectSetting.GetObject();
        // Object2D tile2 = new Polygon2DBuilder().SetName(name+"2").SetMode(!useRegularPolygon).SetLocalPosition(position+new Vector2(10f,0f)).SetVertices(tileVertices).Build();//spaceObjectSetting.GetObject();
        //Object2D spaceObject2 = new Polygon2DBuilder().SetName(name).SetMode(useRegularPolygon).SetLocalPosition(position+new Vector2(20f,0f)).SetVertices(tileVertices).Build();//spaceObjectSetting.GetObject();
        

        //Rendering Codes
        List<Object2D> Tiles = new List<Object2D>();
        for(int i = 0; i < Vertical; i++)
        {
            for(int j = 0; j < Horizontal; j++)
            {

                // List<Vector2> type1TileVertices = new List<Vector2>();
                // for(int k = 0 ; k < vertices.Count; k++)
                // {
                //     type1TileVertices.Add(vertices[k] + 2f*(a - c)*j - 2f*(b - d)*i);
                // }
                // List<Vector2> type2TileVertices = new List<Vector2>();
                // for(int k = 0 ; k < vertices.Count; k++)
                // {
                //     type2TileVertices.Add(-1f*vertices[k] + 2f*a + 2f*(a - c)*j - 2f*(b - d)*i);
                // }
                // List<Vector2> type3TileVertices = new List<Vector2>();
                // for(int k = 0 ; k < vertices.Count; k++)
                // {
                //     type3TileVertices.Add(-1f*vertices[k] + 2f*d + 2f*(a - c)*j - 2f*(b - d)*i);
                // }
                // List<Vector2> type4TileVertices = new List<Vector2>();
                // for(int k = 0 ; k < vertices.Count; k++)
                // {
                //     type4TileVertices.Add(vertices[k] + 2f*(a - b) + 2f*(a - c)*j - 2f*(b - d)*i);
                // }

                Object2D tileType1 = new Polygon2DBuilder().SetName("Tile Type1 at ("+i+","+j+")").SetTileType(tileTypeIndex[0]).SetMode(!useRegularPolygon).SetTileMode(tileMode).SetLocalPosition(position + 2f*(a - c)*j - 2f*(b - d)*i).SetCrossingInfo(crossingAxisRotationInfo, CrossingPointMovementInfo).SetVertices(vertices).Build();
                Object2D tileType2 = new Polygon2DBuilder().SetName("Tile Type2 at ("+i+","+j+")").SetTileType(tileTypeIndex[1]).SetMode(!useRegularPolygon).SetTileMode(tileMode).SetLocalPosition(position + 2f*a + 2f*(a - c)*j - 2f*(b - d)*i).SetCrossingInfo(crossingAxisRotationInfo, CrossingPointMovementInfo).SetVertices(flippedVertices).Build();
                Object2D tileType3 = new Polygon2DBuilder().SetName("Tile Type3 at ("+i+","+j+")").SetTileType(tileTypeIndex[2]).SetMode(!useRegularPolygon).SetTileMode(tileMode).SetLocalPosition(position + 2f*d + 2f*(a - c)*j - 2f*(b - d)*i).SetCrossingInfo(crossingAxisRotationInfo, CrossingPointMovementInfo).SetVertices(flippedVertices).Build();
                Object2D tileType4 = new Polygon2DBuilder().SetName("Tile Type4 at ("+i+","+j+")").SetTileType(tileTypeIndex[3]).SetMode(!useRegularPolygon).SetTileMode(tileMode).SetLocalPosition(position + 2f*(a - b) + 2f*(a - c)*j - 2f*(b - d)*i).SetCrossingInfo(crossingAxisRotationInfo, CrossingPointMovementInfo).SetVertices(vertices).Build();
                // Debug.Log("tileType1: "+tileType1.transform2D.localPosition);
                // Debug.Log("tileType2: "+tileType2.transform2D.localPosition);
                // Debug.Log("tileType3: "+tileType3.transform2D.localPosition);
                // Debug.Log("tileType4: "+tileType4.transform2D.localPosition);
                Tiles.Add(tileType1);
                Tiles.Add(tileType2);
                Tiles.Add(tileType3);
                Tiles.Add(tileType4);
            }
        }
        // Tiles.Add(tile1);
        // Tiles.Add(tile2);

        Object2D TiledVirtualSpace = new Object2DBuilder().SetName(name).SetLocalPosition(position).Build();

        //spaceObject2.gameObject.transform.parent = spaceObject.gameObject.transform;
        List<Object2D> obstacles = new List<Object2D>();
        foreach (ObjectSetting obstacleObjectSetting in obstacleObjectSettings)
            obstacles.Add(obstacleObjectSetting.GetObject());

        return new Space2DBuilder().SetName(name).SetParentSpaceObject(TiledVirtualSpace).SetSpaceObjects(Tiles).SetTileCrossingVectors(tileCrossingVectors).SetTileAreaSetting(tileAreaSetting).SetObstacles(obstacles).Build();

    }
}
