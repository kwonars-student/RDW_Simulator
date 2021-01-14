using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RDWSimulationManager : MonoBehaviour
{
    public SimulationSetting simulationSetting; // 시뮬레이션 환경 설정을 담은 변수
    private RedirectedUnit[] redirectedUnits; //  각 unit들을 통제하는 변수
    private GameObject[] unitObjects;
    Space2D realSpace, virtualSpace; // 실제 공간과 가상 공간에 대한 정보를 담은 변수

    private bool SpaceRLMode = false;
    private bool ArrangementRLMode = false;

    private bool initializeRLAgent = false;
    private int cntForInitialize = 0;

    private bool initializedForObstaclePosition = false;
    private List<Vector2> initialObstaclePositions;

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
                    SpaceRLMode = true;
                }
                else if(redirectedUnits[i].GetRedirector() is ArrangementRedirector && simulationSetting.useVisualization)
                {
                    unitObjects[i] = GameObject.Instantiate(simulationSetting.prefabSetting.arrangementRLPrefab);
                    unitObjects[i].name = "ArrangementRLUnit_" + i;
                    ArrangementRLMode = true;
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
            if (redirectedUnits[i].GetRedirector() is SpaceRedirector && simulationSetting.useVisualization)
            {
                unitObjects[i].GetComponent<SpaceAgent>().OnEpisodeBegin();
            }
            else if (redirectedUnits[i].GetRedirector() is ArrangementRedirector && simulationSetting.useVisualization)
            {
                // unitObjects[i].GetComponent<ArrangementAgent>().OnEpisodeBegin();
                unitObjects[i].GetComponent<ArrangementAgent>();
                redirectedUnits[i].GetRedirector().SetRLArrangementAgent(unitObjects[i].GetComponent<ArrangementAgent>());
            }
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
        GenerateRealSpace();
        GenerateVirtualSpace();
    }

    public void GenerateRealSpace()
    {
        realSpace = simulationSetting.realSpaceSetting.GetSpace();
        realSpace.spaceObject.transform2D.parent = this.transform;

        if (!simulationSetting.realSpaceSetting.usePredefinedSpace)
            realSpace.GenerateSpace(simulationSetting.prefabSetting.realMaterial, simulationSetting.prefabSetting.obstacleMaterial, 3, 2);
    }

    public void GenerateVirtualSpace()
    {
        virtualSpace = simulationSetting.virtualSpaceSetting.GetSpace();
        virtualSpace.spaceObject.transform2D.parent = this.transform;

        if (!simulationSetting.virtualSpaceSetting.usePredefinedSpace)
        {
            virtualSpace.GenerateSpace(simulationSetting.prefabSetting.virtualMaterial, simulationSetting.prefabSetting.obstacleMaterial, 3, 2);
        }
            
        if(!initializedForObstaclePosition)
        {
            initializedForObstaclePosition = true;
            this.initialObstaclePositions = new List<Vector2>();
            for (int i=0; i<virtualSpace.obstacles.Count; i++)
            {
                initialObstaclePositions.Add(virtualSpace.GetObstaclePositionByIndex(i));
            }

            virtualSpace.SetInitialObstaclePositions(initialObstaclePositions);
        }
    }

    public void GenerateUnits()
    {
        redirectedUnits = new RedirectedUnit[simulationSetting.unitSettings.Length];
        
        for (int i = 0; i < simulationSetting.unitSettings.Length; i++)
        {
            redirectedUnits[i] = simulationSetting.unitSettings[i].GetUnit(realSpace, virtualSpace); // 실질적으로 가져옴.
            redirectedUnits[i].GetEpisode().targetPrefab = simulationSetting.prefabSetting.targetPrefab;
            redirectedUnits[i].GetEpisode().setShowTarget(simulationSetting.showTarget);
            redirectedUnits[i].SetShowResetLocator(simulationSetting.showResetLocator);
            redirectedUnits[i].SetResetLocPrefab(simulationSetting.prefabSetting.resetLocPrefab);
        }
        GenerateUnitObjects();
    }

    public bool IsAllEpisodeEnd()
    {
        
        for (int i = 0; i < redirectedUnits.Length; i++)
        {
            // Debug.Log(redirectedUnits[i].GetCurrentTimeStep());
            // Debug.Log(redirectedUnits[i].GetEpisode().GetCurrentEpisodeIndex());
            // Debug.Log(redirectedUnits.Length); // 1만 출력됨
            // Debug.Log(redirectedUnits[i].GetCurrentTimeStep()); State 판단할때마다 계속 1씩 증가함. 엄청 빨리 올라감. 1000되면 끝남
            // Debug.Log(redirectedUnits[i].GetArrangementRLAgent().MaxStep); 1000이 출력됨
            // Debug.Log(redirectedUnits[i].GetEpisode().GetCurrentEpisodeIndex());
            if(SpaceRLMode)
            {
                if (redirectedUnits[i].GetCurrentTimeStep() >= redirectedUnits[i].GetRLAgent().MaxStep)
                return true;
            }
            else if(ArrangementRLMode)
            {
                if (redirectedUnits[i].GetEpisode().GetCurrentEpisodeIndex() >= redirectedUnits[i].GetEpisode().GetEpisodeLength()) // 2,000 Length에 MaxStep은 100,000 정도까지 해야함
                {

                    DestroyUnits();
                    GenerateUnits();

                    unitObjects[i].GetComponent<ArrangementAgent>().EndEpisode(); // EndEpisode -> Observation -> Begin ...
                    redirectedUnits[i].GetEpisode().SetCurrentEpisodeIndex(0);
                    unitObjects[i].GetComponent<ArrangementAgent>().SetActionReady(true);
                    unitObjects[i].GetComponent<ArrangementAgent>().RequestDecision();


                    return false;
                }
            }
            
            if (redirectedUnits[i].GetEpisode().IsNotEnd())
                return false;
        }

        return true;
    }

    public void PrintResult()
    {
        for (int i = 0; i < redirectedUnits.Length; i++)
        {
            // Debug.Log("[Space]");
            // Debug.Log("RealSpace: " + redirectedUnits[i].GetRealSpace().spaceObject.transform2D);
            // Debug.Log("VirtualSpace: " + redirectedUnits[i].GetVirtualSpace().spaceObject.transform2D);
            // Debug.Log("[User]");
            // Debug.Log("RealUser: " + redirectedUnits[i].GetRealUser().transform2D);
            // Debug.Log("VirtualUser: " + redirectedUnits[i].GetVirtualUser().transform2D);
            // Debug.Log("[Current Target]");
            // Debug.Log(redirectedUnits[i].GetEpisode().GetCurrentEpisodeIndex());
            // Debug.Log("[Target Length]");
            // Debug.Log(redirectedUnits[i].GetEpisode().GetEpisodeLength());
            // Debug.Log("[Result Data]");
            // Debug.Log(redirectedUnits[i].resultData);
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
            {
                redirectedUnits[i].Simulation(redirectedUnits);
            }


            if (simulationSetting.useDebugMode) DebugDraws();

        }
        PrintResult();
    }

    public IEnumerator SlowSimulationRoutine()
    {
        Time.timeScale = 1f;
        do
        {
            DestroyAll();
            GenerateSpaces();
            GenerateUnits();

            for (int i = 0; i < redirectedUnits.Length; i++)
            {
                unitObjects[i].GetComponent<ArrangementAgent>().RequestDecision();
            }
            
            //if(!initializeRLAgent)
            //{
            //    initializeRLAgent = true;
            //    yield return new WaitForFixedUpdate();
            //}

            while (!IsAllEpisodeEnd())
            {
                for (int i = 0; i < redirectedUnits.Length; i++)
                {
                    redirectedUnits[i].Simulation(redirectedUnits);
                }
                    

                if (simulationSetting.useDebugMode) DebugDraws();

                // yield return new WaitForEndOfFrame();
                yield return new WaitForFixedUpdate();
            }

            PrintResult();
            //initializeRLAgent = false;

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
