using System.Collections;
using UnityEngine;
using System.Diagnostics;
using System;

public class RDWSimulationManager : MonoBehaviour
{
    public static float remainTime = 0;
    public static float limitTime = 30;
    public SimulationSetting simulationSetting; // 시뮬레이션 환경 설정을 담은 변수
    private RedirectedUnit[] redirectedUnits; //  각 unit들을 통제하는 변수
    Space2D realSpace, virtualSpace; // 실제 공간과 가상 공간에 대한 정보를 담은 변수
    private void GenerateUnitObject(RedirectedUnit redirectedUnit, int index)
    {
        GameObject unitObject = null;
        if (redirectedUnit.GetRedirector() is SpaceRedirector && simulationSetting.useVisualization)
        {
            unitObject = Instantiate(simulationSetting.prefabSetting.RLPrefab);
            unitObject.name = "SpaceRLUnit_" + index;
        }
        else
        {
            unitObject = new GameObject();
            unitObject.name = "Unit_" + index;
        }

        unitObject.AddComponent<RedirectedUnitObject>();
        unitObject.GetComponent<RedirectedUnitObject>().unit = redirectedUnit;
        unitObject.transform.parent = this.transform;
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
            GenerateUnitObject(redirectedUnits[i], i);
            redirectedUnits[i].GetEpisode().targetPrefab = simulationSetting.prefabSetting.targetPrefab;
            redirectedUnits[i].GetEpisode().setShowTarget(simulationSetting.showTarget);
            redirectedUnits[i].SetShowResetLocator(simulationSetting.showResetLocator);
            redirectedUnits[i].SetResetLocPrefab(simulationSetting.prefabSetting.resetLocPrefab);
        }
    }

    public bool IsAllEpisodeEnd()
    {
        for (int i = 0; i < redirectedUnits.Length; i++)
        {
            if (redirectedUnits[i].GetEpisode().IsNotEnd())
                return false;
        }

        return true;
    }

    public void PrintResult()
    {
        for (int i = 0; i < redirectedUnits.Length; i++)
        {
            UnityEngine.Debug.Log("[Space]");
            UnityEngine.Debug.Log("RealSpace: " + redirectedUnits[i].GetRealSpace().spaceObject.transform2D);
            UnityEngine.Debug.Log("VirtualSpace: " + redirectedUnits[i].GetVirtualSpace().spaceObject.transform2D);
            UnityEngine.Debug.Log("[User]");
            UnityEngine.Debug.Log("RealUser: " + redirectedUnits[i].GetRealUser().transform2D);
            UnityEngine.Debug.Log("VirtualUser: " + redirectedUnits[i].GetVirtualUser().transform2D);
            UnityEngine.Debug.Log("[Current Epsiode]");
            UnityEngine.Debug.Log(redirectedUnits[i].GetEpisode().GetCurrentEpisodeIndex());
            UnityEngine.Debug.Log("[Epsiode Length]");
            UnityEngine.Debug.Log(redirectedUnits[i].GetEpisode().GetEpisodeLength());
            UnityEngine.Debug.Log("[Result Data]");
            UnityEngine.Debug.Log(redirectedUnits[i].resultData);
        }
    }

    long overTime = 10 * 1000;
    Stopwatch sw = new Stopwatch();
    bool checkTime = true;

    public void FastSimulationRoutine()
    {
        sw.Start();

        while (!IsAllEpisodeEnd())
        {

            for (int i = 0; i < redirectedUnits.Length; i++)
                redirectedUnits[i].Simulation(redirectedUnits);

            if (simulationSetting.useDebugMode) DebugDraws();

            if(checkTime && sw.ElapsedMilliseconds >= overTime)
            {
                PrintResult();
                throw new TimeoutException();
            }

        }
        PrintResult();
    }

    public IEnumerator SlowSimulationRoutine()
    {
        sw.Start();

        while (!IsAllEpisodeEnd())
        {
            for (int i = 0; i < redirectedUnits.Length; i++)
                redirectedUnits[i].Simulation(redirectedUnits);

            if (simulationSetting.useDebugMode) DebugDraws();


            if (false && sw.ElapsedMilliseconds >= overTime)
            {
                PrintResult();
                throw new TimeoutException();
            }

            yield return new WaitForFixedUpdate();
        }

        PrintResult();
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
        GenerateSpaces();
        GenerateUnits();

        if (simulationSetting.useVisualization)
        {
            StartCoroutine(SlowSimulationRoutine());
        }
        else
        {
            FastSimulationRoutine();
        }
    }
}
