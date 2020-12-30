using System.Collections;
using UnityEngine;
using System;

public class RDWSimulationManager : MonoBehaviour
{
    public SimulationSetting simulationSetting; // 시뮬레이션 환경 설정을 담은 변수
    private RedirectedUnit[] redirectedUnits; //  각 unit들을 통제하는 변수
    private GameObject[] unitObjects;
    Space2D realSpace, virtualSpace; // 실제 공간과 가상 공간에 대한 정보를 담은 변수

    public void GenerateUnitObjects()
    {
        if(unitObjects == null)
        {
            unitObjects = new GameObject[redirectedUnits.Length];
            for (int i=0; i< redirectedUnits.Length; i++)
            {
                if (redirectedUnits[i].GetRedirector() is SpaceRedirector && simulationSetting.useVisualization)
                {
                    unitObjects[i] = GameObject.Instantiate(simulationSetting.prefabSetting.RLPrefab);
                    unitObjects[i].name = "SpaceRLUnit_" + i;
                }
                else
                {
                    unitObjects[i] = new GameObject();
                    unitObjects[i].name = "Unit_" + i;
                }

                unitObjects[i].AddComponent<RedirectedUnitObject>();
                unitObjects[i].transform.parent = this.transform;
            }
        }

        for (int i = 0; i < redirectedUnits.Length; i++)
        {
            unitObjects[i].GetComponent<RedirectedUnitObject>().unit = redirectedUnits[i];
            if (redirectedUnits[i].GetRedirector() is SpaceRedirector && simulationSetting.useVisualization) unitObjects[i].GetComponent<SpaceAgent>().OnEpisodeBegin();
        }
    }

    public void DestroySpace()
    {
        if (realSpace != null) realSpace.Destroy();
        if (virtualSpace != null) virtualSpace.Destroy();
    }

    public void DestroyUnits()
    {
        if (redirectedUnits != null)
        {
            for (int i = 0; i < simulationSetting.unitSettings.Length; i++)
                if (redirectedUnits[i] != null) redirectedUnits[i].Destroy();
        }
    }

    public void DestroyAll()
    {
        DestroySpace();
        DestroyUnits();
    }

    public void GenerateSpaces()
    {
        realSpace = simulationSetting.realSpaceSetting.GetSpace();
        virtualSpace = simulationSetting.virtualSpaceSetting.GetSpace();

        realSpace.spaceObject.transform2D.parent = this.transform;
        virtualSpace.spaceObject.transform2D.parent = this.transform;
        
        if (!simulationSetting.realSpaceSetting.usePredefinedSpace)
            realSpace.GenerateSpace(simulationSetting.prefabSetting.realMaterial, simulationSetting.prefabSetting.obstacleMaterial, 3, 2);
        if (!simulationSetting.virtualSpaceSetting.usePredefinedSpace)
            virtualSpace.GenerateSpace(simulationSetting.prefabSetting.virtualMaterial, simulationSetting.prefabSetting.obstacleMaterial, 3, 2);
    }

    public void GenerateUnits()
    {
        redirectedUnits = new RedirectedUnit[simulationSetting.unitSettings.Length];
        
        for (int i = 0; i < simulationSetting.unitSettings.Length; i++)
        {
            redirectedUnits[i] = simulationSetting.unitSettings[i].GetUnit(realSpace, virtualSpace);
            redirectedUnits[i].GetEpisode().targetPrefab = simulationSetting.prefabSetting.targetPrefab;
        }

        GenerateUnitObjects();
    }

    public bool IsAllEpisodeEnd()
    {
        for (int i = 0; i < redirectedUnits.Length; i++)
        {
            if (redirectedUnits[i].GetCurrentTimeStep() >= redirectedUnits[i].GetRLAgent().MaxStep)
                return true;
            if (redirectedUnits[i].GetEpisode().IsNotEnd())
                return false;
        }

        return true;
    }

    public void PrintResult()
    {
        for (int i = 0; i < redirectedUnits.Length; i++)
        {
            Debug.Log("[Space]");
            Debug.Log("RealSpace: " + redirectedUnits[i].GetRealSpace().spaceObject.transform2D);
            Debug.Log("VirtualSpace: " + redirectedUnits[i].GetVirtualSpace().spaceObject.transform2D);
            Debug.Log("[User]");
            Debug.Log("RealUser: " + redirectedUnits[i].GetRealUser().transform2D);
            Debug.Log("VirtualUser: " + redirectedUnits[i].GetVirtualUser().transform2D);
            Debug.Log("[Current Target]");
            Debug.Log(redirectedUnits[i].GetEpisode().GetCurrentEpisodeIndex());
            Debug.Log("[Target Length]");
            Debug.Log(redirectedUnits[i].GetEpisode().GetEpisodeLength());
            Debug.Log("[Result Data]");
            Debug.Log(redirectedUnits[i].resultData);
        }
    }

    //long overTime = 10 * 1000;
    //Stopwatch sw = new Stopwatch();
    //bool checkTime = true;
    //public static float remainTime = 0;
    //public static float limitTime = 30;

    public void FastSimulationRoutine()
    {
        //int j = 0;
        //do
        //{
        //    DestroyAll();
        //    GenerateSpaces();
        //    GenerateUnits();

        //    while (!IsAllEpisodeEnd())
        //    {
        //        for (int i = 0; i < redirectedUnits.Length; i++)
        //            redirectedUnits[i].Simulation(redirectedUnits);

        //        if (simulationSetting.useDebugMode) DebugDraws();
        //    }

        //    PrintResult();
        //    j++;

        //} while (j < 3);

        DestroyAll();
        GenerateSpaces();
        GenerateUnits();

        while (!IsAllEpisodeEnd())
        {
            for (int i = 0; i < redirectedUnits.Length; i++)
                redirectedUnits[i].Simulation(redirectedUnits);

            if (simulationSetting.useDebugMode) DebugDraws();

        }
        PrintResult();
    }

    public IEnumerator SlowSimulationRoutine()
    {
        do
        {
            DestroyAll();
            GenerateSpaces();
            GenerateUnits();

            while (!IsAllEpisodeEnd())
            {
                for (int i = 0; i < redirectedUnits.Length; i++)
                    redirectedUnits[i].Simulation(redirectedUnits);

                if (simulationSetting.useDebugMode) DebugDraws();

                yield return new WaitForFixedUpdate();
            }

            PrintResult();

        } while (simulationSetting.useContinousSimulation);
    }

    public void DebugDraws()
    {
        realSpace.DebugDraws(Color.red, Color.blue);
        virtualSpace.DebugDraws(Color.red, Color.blue);

        foreach (RedirectedUnit unit in redirectedUnits)
            unit.DebugDraws(Color.green);
    }

    public void Start()
    {
        if (simulationSetting.useVisualization)
            StartCoroutine(SlowSimulationRoutine());
        else
            FastSimulationRoutine();
    }
}
