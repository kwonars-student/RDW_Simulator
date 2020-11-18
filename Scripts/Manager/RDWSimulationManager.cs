using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class RDWSimulationManager : MonoBehaviour
{
    public SimulationSetting simulationSetting; // 시뮬레이션 환경 설정을 담은 변수

    private RedirectedUnit[] redirectedUnit; //  각 unit들을 통제하는 변수
    Space2D realSpace, virtualSpace; // 실제 공간과 가상 공간에 대한 정보를 담은 변수

    private StreamWriter writer;

    public void GenerateSpaces() 
    {
        realSpace = simulationSetting.realSpaceSetting.GetSpace2D();
        virtualSpace = simulationSetting.virtualSpaceSetting.GetSpace2D();
    }

    public void GenerateUnits()
    {
        redirectedUnit = new RedirectedUnit[simulationSetting.unitSettings.Length];
        for (int i = 0; i < simulationSetting.unitSettings.Length; i++)
        {
            redirectedUnit[i] = simulationSetting.unitSettings[i].GetUnit(realSpace, virtualSpace);
        }
    }

    public void Visualize()
    {
        Vector2 realSize = new Vector2(simulationSetting.realSpaceSetting.spaceObject.size, simulationSetting.realSpaceSetting.spaceObject.size);
        Vector2 realOrigin = realSize / 2;
     
        Vector2 virtualSize = new Vector2(simulationSetting.virtualSpaceSetting.spaceObject.size, simulationSetting.virtualSpaceSetting.spaceObject.size);
        Vector2 virtualOrigin = new Vector2(virtualSize.x, 0) + virtualSize / 2;

        realSpace.Instantiate("Real Space", simulationSetting.prefabSetting.realMaterial, simulationSetting.prefabSetting.obstacleMaterial, realSize, realOrigin);
        virtualSpace.Instantiate("Virtual Space", simulationSetting.prefabSetting.virtualMaterial, simulationSetting.prefabSetting.obstacleMaterial, virtualSize, virtualOrigin);

        for (int i = 0; i < simulationSetting.unitSettings.Length; i++)
        {
            string realUserName = "realuser_" + redirectedUnit[i].GetID();
            string virtualUserName = "virtualuser_" + redirectedUnit[i].GetID();

            redirectedUnit[i].GetRealUser().Instantiate(realUserName, simulationSetting.prefabSetting.userPrefabs[i], realSpace.space);
            redirectedUnit[i].GetVirtualUser().Instantiate(virtualUserName, simulationSetting.prefabSetting.userPrefabs[i], virtualSpace.space);
        }
    }

    public bool IsAllEpisodeEnd()
    {
        for (int i = 0; i < redirectedUnit.Length; i++)
        {
            if (redirectedUnit[i].GetEpisode().IsNotEnd())
                return false;
        }

        return true;
    }

    public void PrintResult()
    {
        Debug.Log("---Result---");

        for (int i = 0; i < redirectedUnit.Length; i++)
        {
            Debug.Log(redirectedUnit[i].GetVirtualUser().transform);
            Debug.Log(redirectedUnit[i].GetRealUser().transform);
            Debug.Log(redirectedUnit[i].resultData);
        }
    }

    public void FastSimulationRoutine()
    {
        while (!IsAllEpisodeEnd())
        {
            for (int i = 0; i < redirectedUnit.Length; i++)
            {
                redirectedUnit[i].Simulation(redirectedUnit);
                writer.WriteLine(redirectedUnit[i].GetRealUser().transform.localPosition.x.ToString() + ", " + redirectedUnit[i].GetRealUser().transform.localPosition.y.ToString());
            }
                
        }
        PrintResult();
    }

    public IEnumerator SlowSimulationRoutine()
    {
        while (!IsAllEpisodeEnd())
        {
            for (int i = 0; i < redirectedUnit.Length; i++)
                redirectedUnit[i].Simulation(redirectedUnit);

            yield return new WaitForFixedUpdate();
        }

        PrintResult();
    }

    //public GameObject testPrefab;
    //private Object2D testCube;
    //private float testRotationSpeed = 60.0f;
    //private float myRotation = 0.0f, previousRotation = 0.0f;

    public void Start()
    {
        GenerateSpaces();
        GenerateUnits();
        string path = "Assets/tmp.txt";
        writer = new StreamWriter(path);
        
        if (simulationSetting.useVisualization)
        {
            Visualize();
            StartCoroutine(SlowSimulationRoutine());
        }
        else
        {
            FastSimulationRoutine();
        }
    }

}