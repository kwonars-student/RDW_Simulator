using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RDWSimulationManager : MonoBehaviour
{
    public SimulationSetting simulationSetting; // 시뮬레이션 환경 설정을 담은 변수

    private RedirectedUnit[] redirectedUnit; //  각 unit들을 통제하는 변수
    Space2D realSpace, virtualSpace; // 실제 공간과 가상 공간에 대한 정보를 담은 변수

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

        realSpace.Instantiate("Real Space", simulationSetting.prefabSetting.realMaterial, realSize, realOrigin);
        virtualSpace.Instantiate("Virtual Space", simulationSetting.prefabSetting.virtualMaterial, virtualSize, virtualOrigin);

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
        for (int i = 0; i < redirectedUnit.Length; i++)
            Debug.Log(redirectedUnit[i].resultData);
    }

    //public void FastSimulationRoutine()
    //{
    //    while (!IsAllEpisodeEnd())
    //    {
    //        for (int i = 0; i < redirectedUnit.Length; i++)
    //            redirectedUnit[i].Simulation();
    //    }

    //    PrintResult();
    //}

    //public IEnumerator SlowSimulationRoutine()
    //{
    //    while (!IsAllEpisodeEnd())
    //    {
    //        for (int i = 0; i < redirectedUnit.Length; i++)
    //            redirectedUnit[i].Simulation();

    //        yield return new FixedUpdate();
    //    }

    //    PrintResult();
    //}

    public bool NeedUserReset(int i, out int otherIndex)
    {
        for (int j = 0; j < redirectedUnit.Length; j++)
        {
            if (j == i)
                continue;

            Object2D otherUser = redirectedUnit[j].GetRealUser();

            if (redirectedUnit[i].NeedUserReset(otherUser))
            {
                otherIndex = j;
                return true;
            }
        }

        otherIndex = -1;
        return false;
    }

    public void Simulation()
    {
        for (int i = 0; i < redirectedUnit.Length; i++)
        {
            if (redirectedUnit[i].GetEpisode().IsNotEnd())
            {
                int j = -1;

                if (redirectedUnit[i].NeedWallReset())
                {
                    redirectedUnit[i].ApplyWallReset();
                }
                else if (NeedUserReset(i, out j))
                {
                    redirectedUnit[i].ApplyUserReset();
                    redirectedUnit[j].ApplyUserReset();
                }
                else
                {
                    redirectedUnit[i].Move();
                }
            }
        }
    }

    public void FastSimulationRoutine()
    {
        while (!IsAllEpisodeEnd())
            Simulation();

        PrintResult();
    }

    public IEnumerator SlowSimulationRoutine()
    {
        while (!IsAllEpisodeEnd())
        {
            Simulation();
            yield return new FixedUpdate();
        }

        PrintResult();
    }

    public void Start()
    {
        GenerateSpaces();
        GenerateUnits();

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
