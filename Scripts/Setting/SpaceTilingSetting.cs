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
                float lightHeight = 5f; // 아래 다 켜면 너무 밝음

                GameObject lightGameObject1 = new GameObject("The Light " +"("+j+", "+i+") Type 1");
                Light lightComp1 = lightGameObject1.AddComponent<Light>();
                lightGameObject1.transform.position = Utility.CastVector2Dto3D(position + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lightHeight,0);
                // lightGameObject2.transform.position = Utility.CastVector2Dto3D(position + 2f*a + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lightHeight,0);
                // lightGameObject3.transform.position = Utility.CastVector2Dto3D(position + 2f*d + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lightHeight,0);
                // lightGameObject4.transform.position = Utility.CastVector2Dto3D(position + 2f*(a - b) + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lightHeight,0);

                float lineHeight = 0.0f;
                float lineSize = 0.05f;

                GameObject lineObject1 = new GameObject("The Line " +"("+j+", "+i+") Type 1");
                GameObject lineObject2 = new GameObject("The Line " +"("+j+", "+i+") Type 2");
                GameObject lineObject3 = new GameObject("The Line " +"("+j+", "+i+") Type 3");
                GameObject lineObject4 = new GameObject("The Line " +"("+j+", "+i+") Type 4");

                LineRenderer lineRenderer1 = lineObject1.AddComponent<LineRenderer>();
                LineRenderer lineRenderer2 = lineObject2.AddComponent<LineRenderer>();
                LineRenderer lineRenderer3 = lineObject3.AddComponent<LineRenderer>();
                LineRenderer lineRenderer4 = lineObject4.AddComponent<LineRenderer>();

                lineRenderer1.SetVertexCount(vertices.Count+1);
                lineRenderer2.SetVertexCount(vertices.Count+1);
                lineRenderer3.SetVertexCount(vertices.Count+1);
                lineRenderer4.SetVertexCount(vertices.Count+1);

                Material particleMat = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
                lineRenderer1.material = particleMat;
                lineRenderer2.material = particleMat;
                lineRenderer3.material = particleMat;
                lineRenderer4.material = particleMat;
                lineRenderer1.SetColors(Color.blue, Color.blue);
                lineRenderer2.SetColors(Color.blue, Color.blue);
                lineRenderer3.SetColors(Color.blue, Color.blue);
                lineRenderer4.SetColors(Color.blue, Color.blue);


                for(int k = 0 ; k < vertices.Count; k++)
                {
                    lineRenderer1.SetPosition(k, Utility.CastVector2Dto3D(vertices[k] + position + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lineHeight,0));
                    lineRenderer2.SetPosition(k, Utility.CastVector2Dto3D(-vertices[k] + position + 2f*a + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lineHeight,0));
                    lineRenderer3.SetPosition(k, Utility.CastVector2Dto3D(-vertices[k] + position + 2f*d + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lineHeight,0));
                    lineRenderer4.SetPosition(k, Utility.CastVector2Dto3D(vertices[k] + position + 2f*(a - b) + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lineHeight,0));
                }

                lineRenderer1.SetPosition(vertices.Count, Utility.CastVector2Dto3D(vertices[0] + position + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lineHeight,0));
                lineRenderer2.SetPosition(vertices.Count, Utility.CastVector2Dto3D(-vertices[0] + position + 2f*a + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lineHeight,0));
                lineRenderer3.SetPosition(vertices.Count, Utility.CastVector2Dto3D(-vertices[0] + position + 2f*d + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lineHeight,0));
                lineRenderer4.SetPosition(vertices.Count, Utility.CastVector2Dto3D(vertices[0] + position + 2f*(a - b) + 2f*(a - c)*j - 2f*(b - d)*i) + new Vector3(0,lineHeight,0));

                lineRenderer1.SetWidth(lineSize, lineSize);
                lineRenderer2.SetWidth(lineSize, lineSize);
                lineRenderer3.SetWidth(lineSize, lineSize);
                lineRenderer4.SetWidth(lineSize, lineSize);

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
