using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Polygon2D : Object2D
{
    private List<Vector2> vertices; // local 좌표계 기준
    private Vector2 squareCenter; // local 좌표계 기준
    private List<Vector2> crossBoundaryPoints; // local 좌표계 기준

    public Polygon2D() : base() // 기본 생성자
    {
        vertices = new List<Vector2>();
        vertices.Add(new Vector2(0.5f, 0.5f));
        vertices.Add(new Vector2(-0.5f, 0.5f));
        vertices.Add(new Vector2(-0.5f, -0.5f));
        vertices.Add(new Vector2(0.5f, -0.5f));

        crossBoundaryPoints = new List<Vector2>();
        squareCenter = GetSquareCenter();
        crossBoundaryPoints = GetCrossBoundaryPoints();
    }

    public Polygon2D(Polygon2D otherObject, string name = null) : base(otherObject, name) // 복사 생성자
    {
        this.vertices = new List<Vector2>(otherObject.vertices);
        this.squareCenter = GetSquareCenter();
        this.crossBoundaryPoints = GetCrossBoundaryPoints();
    }

    public Polygon2D(GameObject prefab, string name, Vector2 localPosition, float localRotation, Vector2 localScale, Object2D parentObject = null, List<Vector2> vertices = null) : base(prefab, name, localPosition, localRotation, localScale, parentObject) // vertex 위치를 직접 지정 하여 polygon을 생성하는 생성자
    {
        if(prefab == null)
            this.vertices = new List<Vector2>(vertices);

        this.squareCenter = GetSquareCenter();
        this.crossBoundaryPoints = GetCrossBoundaryPoints();
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
        this.squareCenter = GetSquareCenter();
        this.crossBoundaryPoints = GetCrossBoundaryPoints();
    }

    public Polygon2D(GameObject prefab) : base(prefab) // 참조 생성자
    {
        Mesh objectMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        Vector2[] projectedVertices = Utility.ProjectionVertices(objectMesh.vertices);
        Graph connectionGraph = Utility.GetConnectionGraph(projectedVertices, objectMesh.triangles);

        this.vertices = new List<Vector2>();
        this.vertices = connectionGraph.FindOutline(true);
        this.squareCenter = GetSquareCenter();
        this.crossBoundaryPoints = GetCrossBoundaryPoints();
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

    public Vector2 GetCrossBoundaryVertex(int index, Space relativeTo)
    {
        // int realIndex = Utility.mod(index, vertices.Count);

        if (relativeTo == Space.Self)
            return crossBoundaryPoints[index];
        else
            return this.transform2D.TransformPointToGlobal(crossBoundaryPoints[index]);
    }

    public Vector2 GetSquareCenter()
    {
        Vector2 v_max = Vector2.zero;
        Vector2 v_min = Vector2.zero;
        Vector2 c = Vector2.zero;

        //Debug.Log(vertices.Count);
        for (int i = 0; i < vertices.Count; i++)
        {
            if(v_max.x <= vertices[i].x)
            {
                v_max.x = vertices[i].x;
            }
            if(v_min.x >= vertices[i].x)
            {
                v_min.x = vertices[i].x;
            }

            if(v_max.y <= vertices[i].y)
            {
                v_max.y = vertices[i].y;
            }
            if(v_min.y >= vertices[i].y)
            {
                v_min.y = vertices[i].y;
            }
        }

        c = 0.5f*(v_max + v_min);

        return c;
    }

    public List<Vector2> GetCrossBoundaryPoints()
    {
        List<Vector2> possibleCrossPoints = new List<Vector2>();
        List<Vector2> crossPoints = new List<Vector2>(); // 순서: 상 하 좌 우
        Vector2 c = this.squareCenter;
        List<float> temp1 = new List<float>();
        List<float> temp2 = new List<float>();
        List<float> temp3 = new List<float>();
        List<float> temp4 = new List<float>();
        int p=0;

        for (int i = 0; i < vertices.Count; i++)
        {
            // Debug.Log(vertices[0].x+ vertices[0].y);
            // Debug.Log(vertices[1].x + vertices[1].y);
            // Debug.Log(vertices[2].x + vertices[2].y);
            // Debug.Log(vertices[3].x + vertices[3].y);
            if (i-1 < 0)
            {
                p = vertices.Count - 1;
            }
            else
            {
                p = i-1;
            }

            if( c.x == vertices[p].x )
            {
                possibleCrossPoints.Add(vertices[p]);
            }
            else if(c.x == vertices[i].x)
            {
                possibleCrossPoints.Add(vertices[i]);
            }
            else if( (c.x - vertices[p].x)*(c.x - vertices[i].x) < 0 )
            {
                if(vertices[i].x == vertices[p].x)
                {
                    Debug.Log("Cannot Devide by Zero!");
                }
                else
                {
                    possibleCrossPoints.Add(new Vector2(c.x, vertices[p].y + (c.x-vertices[p].x)*(vertices[i].y - vertices[p].y)/(vertices[i].x - vertices[p].x)));
                    // Vector2 a = new Vector2(c.x, vertices[p].y + (c.x - vertices[p].x) * (vertices[i].y - vertices[p].y) / (vertices[i].x - vertices[p].x));
                    // Debug.Log(a.x + a.y);
                }
            }

            if( c.y == vertices[p].y )
            {
                possibleCrossPoints.Add(vertices[p]);
            }
            else if(c.y == vertices[i].y)
            {
                possibleCrossPoints.Add(vertices[i]);
            }
            else if( (c.y - vertices[p].y)*(c.y - vertices[i].y) < 0 )
            {
                if(vertices[i].y == vertices[p].y)
                {
                    Debug.Log("Cannot Devide by Zero!");
                }
                else
                {
                    possibleCrossPoints.Add(new Vector2(vertices[p].x + (c.y-vertices[p].y)*(vertices[i].x - vertices[p].x)/(vertices[i].y - vertices[p].y), c.y));
                    //Debug.Log(new Vector2(vertices[p].x + (c.y-vertices[p].y)*(vertices[i].x - vertices[p].x)/(vertices[i].y - vertices[p].y), c.y));
                    // Debug.Log(vertices[i]);
                }
            }
        }

        for (int i = 0; i < possibleCrossPoints.Count; i++)
        {
            //Debug.Log(possibleCrossPoints[i]);
            if(possibleCrossPoints[i].x == c.x && possibleCrossPoints[i].y > c.y)
            {
                temp1.Add(possibleCrossPoints[i].y);
            }
            else if(possibleCrossPoints[i].x == c.x && possibleCrossPoints[i].y < c.y)
            {
                temp2.Add(possibleCrossPoints[i].y);
            }
            else if(possibleCrossPoints[i].y == c.y && possibleCrossPoints[i].x < c.x)
            {
                temp3.Add(possibleCrossPoints[i].x);
            }
            else if(possibleCrossPoints[i].y == c.y && possibleCrossPoints[i].x > c.x)
            {
                temp4.Add(possibleCrossPoints[i].x);
            }
            
        }

        crossPoints.Add(new Vector2(c.x, temp1.Min()));
        crossPoints.Add(new Vector2(c.x, temp2.Max()));
        crossPoints.Add(new Vector2(temp3.Min(), c.y));
        crossPoints.Add(new Vector2(temp4.Max(), c.y));

        return crossPoints;
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
        Vector2[] vertices2DArray = new Vector2[this.vertices.Count];
        Vector3[] vertices3DArray = new Vector3[this.vertices.Count];

        for (int i = 0; i < this.vertices.Count; i++)
        {
            vertices2DArray[i] = this.vertices[i];
        }

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2DArray);
        int[] triangles = tr.Triangulate();

        for (int i = 0; i < this.vertices.Count; i++)
        {
            vertices3DArray[i] = Utility.CastVector2Dto3D(GetVertex(i, Space.Self));//this.vertices[i];
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices3DArray;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

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
        //Debug.Log(vertices[3]);
        for (int i = 0; i < n; i++)
        {
            Vector3 vec1 = Utility.CastVector2Dto3D(GetVertex(i, Space.World));
            Vector3 vec2 = Utility.CastVector2Dto3D(GetVertex(i+1, Space.World));
            Debug.DrawLine(vec1, vec2, color);
        }

        // Debug.Log(crossBoundaryPoints[0]);
        // Debug.Log(crossBoundaryPoints[1]);
        // Debug.Log(crossBoundaryPoints[2]);
        // Debug.Log(crossBoundaryPoints[3]);

        Vector3 cp1 = Utility.CastVector2Dto3D(GetCrossBoundaryVertex(0, Space.World));
        Vector3 cp2 = Utility.CastVector2Dto3D(GetCrossBoundaryVertex(1, Space.World));
        Vector3 cp3 = Utility.CastVector2Dto3D(GetCrossBoundaryVertex(2, Space.World));
        Vector3 cp4 = Utility.CastVector2Dto3D(GetCrossBoundaryVertex(3, Space.World));
        Debug.DrawLine(cp1, cp2, color);
        Debug.DrawLine(cp3, cp4, color);
    }
}
