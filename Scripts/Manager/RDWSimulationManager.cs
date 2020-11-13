using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RDWSimulationManager : MonoBehaviour
{
    public SimulationSetting simulationSetting;

    private RedirectedUnit[] redirectedUnit; // 실제 실험할때 각 unit들을 통제하는 클래스
    Space2D realSpace, virtualSpace;

    // object의 이름, material, 크기 그리고 위치를 입력 받아서 직사각형의 공간 object를 생성하는 함수
    public void GenerateSpace(string name, Material material, Vector2 size, Vector2 origin) {
        GameObject gameObject = new GameObject(); // GameObject 생성
        gameObject.name = name; // 이름 바꾸기
        gameObject.AddComponent<MeshFilter>(); // MeshFilter 컴포넌트 부착
        gameObject.AddComponent<MeshRenderer>(); // MeshRenderer 컴포넌트 부착
        gameObject.transform.SetParent(this.transform); // RDWSimulationManager 밑에 두기
        gameObject.transform.localPosition = new Vector3(origin.x, 0, origin.y);

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

        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh; // 위의 값들을 가지고 공간 object에 대한 mesh 초기화
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        gameObject.GetComponent<MeshRenderer>().material = material; // 공간 object에 대한 material 초기화
    }

    // 입력된 unit 정보를 가지고 실제 RDWUnit을 생성해내는 함수
    public void GenerateUnits() {
        redirectedUnit = new RedirectedUnit[simulationSetting.unitSettings.Length]; // units 수 만큼 생성

        for (int i=0; i< simulationSetting.unitSettings.Length; i++) {
            GameObject rdwUnitObject = new GameObject();

            switch (simulationSetting.unitSettings[i].unitType) {
                case UnitSetting.UnitType.User:
                    redirectedUnit[i] = rdwUnitObject.AddComponent<RedirectedUser>();
                    break;
                default:
                    redirectedUnit[i] = rdwUnitObject.AddComponent<RedirectedUnit>();
                    break;
            }

            redirectedUnit[i].Initialzing(simulationSetting, i);
        }
    }

    public void GenerateSpaces()
    {
        // TODO: usePredefinedSpace 라는 조건에서는 어떻게?
        Vector2 realSize = simulationSetting.realSpaceSetting.size;
        Vector2 realOrigin = simulationSetting.realSpaceSetting.originPosition;
        if (simulationSetting.realSpaceSetting.usePredefinedOrigin)
            realOrigin = realSize / 2;

        Vector2 virtualSize = simulationSetting.virtualSpaceSetting.size;
        Vector2 virtualOrigin = simulationSetting.virtualSpaceSetting.originPosition;
        if (simulationSetting.virtualSpaceSetting.usePredefinedOrigin)
            virtualOrigin = new Vector2(virtualSize.x, 0) + virtualSize / 2;

        GenerateSpace("Real Space", simulationSetting.objectSetting.realMaterial, realSize, realOrigin);
        GenerateSpace("Virtual Space", simulationSetting.objectSetting.virtualMaterial, virtualSize, virtualOrigin);
    }

    public void Start()
    {
        //realSpace = GenerateSpaceV2(simulationSetting.realSpaceSetting);
        //virtualSpace = GenerateSpaceV2(simulationSetting.virtualSpaceSetting);

        //redirectedUnit = new RedirectedUnit[simulationSetting.unitSettings.Length];
        //for (int i = 0; i < simulationSetting.unitSettings.Length; i++)
        //{
        //    redirectedUnit[i] = GenerateUnitV2(simulationSetting.unitSettings[i]);
        //}

        GenerateSpaces();
        GenerateUnits();

        for (int i = 0; i < redirectedUnit.Length; i++)
        {
            StartCoroutine(redirectedUnit[i].SimulationCoroutine());
        }
    }

    //public Space2D GenerateSpaceV2(SpaceSetting spaceSetting)
    //{
    //    Space2D result = spaceSetting.GetSpace2D();
    //    if (simulationSetting.useVisualization) VisualizeSpace();
    //    return result;
    //}

    //public RedirectedUnit GenerateUnitV2(UnitSetting unitSetting)
    //{
    //    return unitSetting.GetUnit();
    //}

    //public void VisualizeSpace(Space2D space, string name, Material material)
    //{
    //}

    //public void VisualizeUnit(RedirectedUnit redirectedUnit, string name, Object prefab)
    //{

    //}
}
