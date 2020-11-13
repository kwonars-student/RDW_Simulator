using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.UIElements;

public class Space2D 
{
    public Object2D space;
    public List<Object2D> obstacles;

    public Space2D()
    {
        this.space = new Object2D();
        this.obstacles = new List<Object2D>();
    }

    public Space2D(Space2D otherSpace)
    {
        this.space = new Object2D(otherSpace.space);
        this.obstacles = new List<Object2D>(otherSpace.obstacles);
    }

    public Space2D(Object2D space, List<Object2D> obstacles)
    {
        if(space is Polygon2D) // TODO: 부모와 자식 클래스 형변환이 올바르게 되도록 정리
            this.space = new Polygon2D((Polygon2D)space);
        else
            this.space = new Object2D(space);

        this.obstacles = new List<Object2D>(obstacles);
    }

    // TODO: 임시적인 Instantiate
    public void Instantiate(string name, Material spaceMaterial, Material obstacleMaterial, Vector2 size, Vector2 origin)
    {
        // object의 이름, material, 크기 그리고 위치를 입력 받아서 직사각형의 공간 object를 생성하는 함수
        space.gameObject = new GameObject(); // GameObject 생성
        space.gameObject.name = name; // 이름 바꾸기
        space.gameObject.AddComponent<MeshFilter>(); // MeshFilter 컴포넌트 부착
        space.gameObject.AddComponent<MeshRenderer>(); // MeshRenderer 컴포넌트 부착
        //gameObject.transform.SetParent(this.transform); // RDWSimulationManager 밑에 두기
        space.gameObject.transform.localPosition = new Vector3(origin.x, 0, origin.y);

        Vector3[] vertices = new Vector3[] // 공간 object에 대한 vertex 좌표값 지정
        {
                new Vector3(-size.x / 2, 0, -size.y / 2),
                new Vector3(-size.x / 2, 0, size.y / 2),
                new Vector3(size.x / 2, 0, size.y / 2),
                new Vector3(size.x / 2, 0, -size.y / 2)
        };

        int[] triangles = new int[] // triangle index 지정
        {
                0, 1, 2,
                2, 3, 0
        };

        Vector2[] uv = new Vector2[] // uv 값 지정
        {
                new Vector2(0,0),
                new Vector2(0,1),
                new Vector2(1,1),
                new Vector2(1,0)
        };

        Mesh mesh = space.gameObject.GetComponent<MeshFilter>().mesh; // 위의 값들을 가지고 공간 object에 대한 mesh 초기화
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        space.gameObject.GetComponent<MeshRenderer>().material = spaceMaterial; // 공간 object에 대한 material 초기화

        InstntiateObstacle(obstacleMaterial);
    }

    public void InstntiateObstacle(Material obstacleMaterial)
    {

        for(int i=0; i<obstacles.Count; i++)
        {

            Polygon2D temp = (Polygon2D)obstacles[i];

            temp.gameObject = new GameObject();
            temp.gameObject.transform.parent = space.gameObject.transform;
            temp.gameObject.name = "obstacle_" + i;
            temp.gameObject.AddComponent<MeshFilter>(); // MeshFilter 컴포넌트 부착
            temp.gameObject.AddComponent<MeshRenderer>(); // MeshRenderer 컴포넌트 부착
            temp.gameObject.transform.localPosition = Utility.Cast2Dto3D(temp.transform.localPosition);
            temp.gameObject.transform.localRotation = Quaternion.Euler(0, -temp.transform.localRotation, 0);

            int n = temp.GetVertices().Count;
            Vector3[] vertices = new Vector3[2*n]; 

            for(int j=0; j< 2*n; j++) // 공간 object에 대한 vertex 좌표값 지정
            {
                if(j >= n)
                    vertices[j] = Utility.Cast2Dto3D(temp.GetVertex(j % n), 2);
                else
                    vertices[j] = Utility.Cast2Dto3D(temp.GetVertex(j));
            }

            int[] triangles = new int[] // triangle index 지정
            {
                1,2,3,3,0,1, // bottom front-face TODO: n각형에 대해서도 생성이 가능하도록 만듦
                0,4,5,5,1,0, 
                1,5,6,6,2,1,
                2,6,7,7,3,2,
                3,7,4,4,0,3,
                6,5,4,4,7,6,
            };

            //Vector2[] uv = new Vector2[] // uv 값 지정
            //{
            //    new Vector2(0,0),
            //    new Vector2(0,1),
            //    new Vector2(1,1),
            //    new Vector2(1,0)
            //};

            Mesh mesh = temp.gameObject.GetComponent<MeshFilter>().mesh; // 위의 값들을 가지고 공간 object에 대한 mesh 초기화
            //mesh.vertices = vertices;
            //mesh.triangles = triangles;
            //mesh.uv = uv;
            //mesh.RecalculateNormals();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.Optimize();
            mesh.RecalculateNormals();


            temp.gameObject.GetComponent<MeshRenderer>().material = obstacleMaterial; // 공간 object에 대한 material 초기화
        }
    }

    public bool IsInside(Vector2 samplingPosition)
    {
        return space.IsInside(samplingPosition);
    }

    public  bool IsReallyInside(Vector2 userPosition, Vector2 samplingPosition)
    {
        if(!space.IsInside(samplingPosition))
        {
            return false;
        }
        else
        {
            LineSegment2D line = new LineSegment2D(userPosition, samplingPosition, space.transform);

            for (int i = 0; i < obstacles.Count; i++)
            {
                if (obstacles[i].IsIntersect(line))
                    return false;
            }

            return true;
        }
    }
}
