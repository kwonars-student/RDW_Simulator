using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2D : Object2D
{
    private List<Vector2> vertices; // local 좌표계 기준

    public Polygon2D() : base() // 기본 생성자
    {
        vertices = new List<Vector2>();
        vertices.Add(new Vector2(0.5f, 0.5f));
        vertices.Add(new Vector2(-0.5f, 0.5f));
        vertices.Add(new Vector2(-0.5f, -0.5f));
        vertices.Add(new Vector2(0.5f, -0.5f));
    }

    public Polygon2D(Polygon2D otherObject, string name = null) : base(otherObject, name) // 복사 생성자
    {
        this.vertices = new List<Vector2>(otherObject.vertices);
    }

    public Polygon2D(GameObject prefab, string name, Vector2 localPosition, float localRotation, Vector2 localScale, Object2D parentObject = null, List<Vector2> vertices = null) : base(prefab, name, localPosition, localRotation, localScale, parentObject) // vertex 위치를 직접 지정 하여 polygon을 생성하는 생성자
    {
        if(prefab == null)
            this.vertices = new List<Vector2>(vertices);
    }

    public Polygon2D(GameObject prefab, string name, Vector2 localPosition, float localRotation, Vector2 localScale, int count, float size, Object2D parentObject = null) : base(prefab, name, localPosition, localRotation, localScale, parentObject) // n각형과 size를 지정하여 polygon을 생성하는 방식 생성자
    {
        if(prefab == null) // TODO: 현재 4각형만 지원
        {
            vertices = new List<Vector2>();
            vertices.Add(new Vector2(size / 2, size / 2));
            vertices.Add(new Vector2(-size / 2, size / 2));
            vertices.Add(new Vector2(-size / 2, -size / 2));
            vertices.Add(new Vector2(size / 2, -size / 2));
        }
    }

    public Polygon2D(GameObject prefab) : base(prefab) // 참조 생성자
    {
        Mesh objectMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        Vector2[] projectedVertices = Utility.ProjectionVertices(objectMesh.vertices);
        Graph connectionGraph = Utility.GetConnectionGraph(projectedVertices, objectMesh.triangles);

        this.vertices = new List<Vector2>();
        this.vertices = connectionGraph.FindOutline(true);
    }

    public override Object2D Clone(string name = null)
    {
        Polygon2D copied = new Polygon2D(this, name);
        return copied;
    }

    public override string ToString()
    {
        if (vertices == null)
            return "";
        else
            return base.ToString() + string.Format("vertices: {0}\n", string.Join(",", vertices));
    }

    public List<Vector2> GetVertices()
    {
        return vertices;
    }

    public Edge2D GetEdge(int startIndex, Space relativeTo)
    {
        Vector2 p1 = GetVertex(startIndex, relativeTo);
        Vector2 p2 = GetVertex(startIndex + 1, relativeTo);

        return new Edge2D(p1, p2);
    }

    public Vector2 GetVertex(int index, Space relativeTo)
    {
        int realIndex = Utility.mod(index, vertices.Count);

        if (relativeTo == Space.Self)
            return vertices[realIndex];
        else
            return this.transform2D.TransformPointToGlobal(vertices[realIndex]);
    }

    public Vector2 GetInnerVertex(int index, float distance, Space relativeTo)
    {
        Vector2 directionToPrevious = (GetVertex(index - 1, relativeTo) - GetVertex(index, relativeTo)).normalized;
        Vector2 directionToNext = (GetVertex(index + 1, relativeTo) - GetVertex(index, relativeTo)).normalized;
        float currentInnerAngle = Vector2.SignedAngle(directionToPrevious, directionToNext);

        if (currentInnerAngle == 0 || currentInnerAngle == 180)
        {
            Vector2 directionToPreviousInner = (GetInnerVertex(index - 1, distance, relativeTo) - GetVertex(index, relativeTo));
            float sign = Mathf.Sign(Vector2.SignedAngle(directionToPrevious, directionToPreviousInner));
            Vector2 directionToMiddle = Utility.RotateVector2(directionToPrevious, sign * 90).normalized;

            distance *= 0.5f;
            return GetVertex(index, relativeTo) + directionToMiddle * Mathf.Abs(distance);
        }
        else
        {
            Vector2 directionToMiddle = ((directionToPrevious + directionToNext) / 2).normalized;
            if (currentInnerAngle < 0)
                directionToMiddle = -directionToMiddle;

            return GetVertex(index, relativeTo) + directionToMiddle * distance;
        }
    }

    public override void Initialize(GameObject prefab, string name, Vector2 localPosition, float localRotation, Vector2 localScale, Transform parent)
    {
        base.Initialize(prefab, name, localPosition, localRotation, localScale, parent);

        if (prefab != null)
        {
            Mesh objectMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            Vector2[] projectedVertices = Utility.ProjectionVertices(objectMesh.vertices);
            Graph connectionGraph = Utility.GetConnectionGraph(projectedVertices, objectMesh.triangles);

            this.vertices = new List<Vector2>();
            this.vertices = connectionGraph.FindOutline(true);
        }
    }

    public override Mesh GenerateMesh(bool useOutNormal, float height)
    {
        Vector3[] vertices = null;
        int[] triangles = null;
        Vector3[] normals = null;

        int n = this.vertices.Count;
        vertices = new Vector3[6 * n]; // vertex 지정
        vertices[0] = vertices[7] = vertices[19] = Utility.CastVector2Dto3D(GetVertex(0, Space.Self)); // 음... 하드 코딩
        vertices[1] = vertices[6] = vertices[11] = Utility.CastVector2Dto3D(GetVertex(1, Space.Self));
        vertices[2] = vertices[10] = vertices[14] = Utility.CastVector2Dto3D(GetVertex(2, Space.Self));
        vertices[3] = vertices[15] = vertices[18] = Utility.CastVector2Dto3D(GetVertex(3, Space.Self));
        vertices[4] = vertices[16] = vertices[20] = Utility.CastVector2Dto3D(GetVertex(0, Space.Self), height);
        vertices[5] = vertices[8] = vertices[21] = Utility.CastVector2Dto3D(GetVertex(1, Space.Self), height);
        vertices[9] = vertices[13] = vertices[22] = Utility.CastVector2Dto3D(GetVertex(2, Space.Self), height);
        vertices[12] = vertices[17] = vertices[23] = Utility.CastVector2Dto3D(GetVertex(3, Space.Self), height);

        if (useOutNormal) // 바깥쪽에서 보이고 싶을 경우
        {
            triangles = new int[] // index 지정
            {
                1,2,3,3,0,1, // 바깥쪽으로 face 생성 
                5,6,7,7,4,5,
                9,10,11,11,8,9,
                13,12,15,15,14,13,
                17,16,19,19,18,17,
                21,20,23,23,22,21
            };

            normals = new Vector3[6 * n]; // normal 지정
            normals[0] = normals[1] = normals[2] = normals[3] = new Vector3(0, -1, 0); // 음... 하드 코딩
            normals[4] = normals[5] = normals[6] = normals[7] = new Vector3(0, 0, 1);
            normals[8] = normals[9] = normals[10] = normals[11] = new Vector3(-1, 0, 0);
            normals[12] = normals[13] = normals[14] = normals[15] = new Vector3(0, 0, -1);
            normals[16] = normals[17] = normals[18] = normals[19] = new Vector3(1, 0, 0);
            normals[20] = normals[21] = normals[22] = normals[23] = new Vector3(0, 1, 0);

        }
        else // 안쪽에서 보이고 싶을 경우
        {
            triangles = new int[] // index 지정
            {
                1,0,3,3,2,1,  // 안쪽으로 face 생성 
                5,4,7,7,6,5,
                9,8,11,11,10,9,
                13,14,15,15,12,13,
                17,18,19,19,16,17,
            };

            normals = new Vector3[6 * n]; // normal 지정
            normals[0] = normals[1] = normals[2] = normals[3] = new Vector3(0, 1, 0); // 음... 하드 코딩
            normals[4] = normals[5] = normals[6] = normals[7] = new Vector3(0, 0, -1);
            normals[8] = normals[9] = normals[10] = normals[11] = new Vector3(1, 0, 0);
            normals[12] = normals[13] = normals[14] = normals[15] = new Vector3(0, 0, 1);
            normals[16] = normals[17] = normals[18] = normals[19] = new Vector3(-1, 0, 0);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        return mesh;
    }

    public override bool IsIntersect(Object2D targetObject) // global 좌표계로 변환시킨 후 비교 
    {
        if (targetObject is Polygon2D)
        {
            Polygon2D polygon = (Polygon2D)targetObject;

            for(int i=0; i< polygon.GetVertices().Count; i++)
            {
                Edge2D otherEdge = polygon.GetEdge(i, Space.World);

                if (this.IsIntersect(otherEdge, Space.World))
                    return true;
            }

            return false;
        }
        else if (targetObject is LineSegment2D)
        {
            LineSegment2D line = (LineSegment2D)targetObject;
            Edge2D targetLine = line.ChangeToEdge(Space.World);

            return this.IsIntersect(targetLine, Space.World);
        }
        else if (targetObject is Circle2D)
        {
            return targetObject.IsIntersect(this);
        }
        else
        {
            throw new System.NotImplementedException();
        }
    }

    public override bool IsIntersect(Edge2D targetLine, Space relativeTo, string option = "default") // targetLine 은 relativeTo 좌표계에 있다고 가정
    {
        int numOfIntersect = 0;

        for (int i = 0; i < vertices.Count; i++)
        {
            Edge2D boundary = GetEdge(i, relativeTo);

            if (boundary.CheckIntersect(targetLine, 0.01f, option) == Intersect.EXIST)
                numOfIntersect += 1;
        }

        if (numOfIntersect == 0)
            return false;
        else
            return true;
    }

    public override bool IsInside(Object2D targetObject, float bound = 0) // global 좌표계로 변환시킨 후 비교
    {
        Vector2 globalTargetPosition = targetObject.transform2D.position;

        return this.IsInside(globalTargetPosition, Space.World, bound);
        //if (IsInside(globalTargetPosition, Space.World, bound))
        //{
        //    return !IsIntersect(targetObject);
        //}
        //else
        //{
        //    return false;
        //}
    }

    public override bool IsInside(Vector2 targetPoint, Space relativeTo, float bound = 0) // targetLine 은 relativeTo 좌표계에 있다고 가정
    {
        Ray2D ray = new Ray2D(targetPoint, Vector2.right);
        int numOfIntersect = 0;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 p1 = GetInnerVertex(i, bound, relativeTo);
            Vector2 p2 = GetInnerVertex(i + 1, bound, relativeTo);

            //if (relativeTo == Space.World)
            //{
            //    p1 = this.transform2D.TransformPointToGlobal(p1);
            //    p2 = this.transform2D.TransformPointToGlobal(p2);
            //}

            Edge2D boundary = new Edge2D(p1, p2);

            if (boundary.CheckIntersect(ray, 0.001f) == Intersect.EXIST)
                numOfIntersect += 1;
        }


        if (numOfIntersect % 2 == 0)
            return false;
        else
            return true;
    }    

    public override void DebugDraw(Color color)
    {
        int n = this.vertices.Count;
        for (int i = 0; i < n; i++)
        {
            Vector3 vec1 = Utility.CastVector2Dto3D(GetVertex(i, Space.World));
            Vector3 vec2 = Utility.CastVector2Dto3D(GetVertex(i+1, Space.World));
            Debug.DrawLine(vec1, vec2, color);
        }
    }
}
