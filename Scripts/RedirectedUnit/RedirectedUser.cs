using System.Collections;
using UnityEngine;

public class RedirectedUser : RedirectedUnit
{
    protected SimulationController controller;

    public override void Initialzing(SimulationSetting simulationSetting, int id) {
        base.Initialzing(simulationSetting, id);
        controller = new SimulationController(this, simulationSetting.unitSettings[id].GetEpisode(),  simulationSetting.virtualSpaceSetting.GetSpace2D(), simulationSetting.objectSetting.targetPrefab);
    }


    IEnumerator ResetCoroutine() {
        resetter.isComplete = false;

        while (!resetter.isComplete) {
            resetter.ApplyReset();

            yield return new WaitForFixedUpdate();
        }
    }

    public override IEnumerator SimulationCoroutine() {
        resultData = new ResultData();
        resultData.setUnitID(id);
        resultData.setEpisodeID(controller.getEpisodeID());

        while (controller.isNotEnd()) {
            if (resetter.NeedReset())
            {
                resultData.AddWallReset();
                yield return StartCoroutine(ResetCoroutine());
            }

            controller.VirtualMove();
            (Redirector.GainType type, float appliedGain) = controller.RealMove();

            resultData.setGains(type, appliedGain);
            resultData.AddElapsedTime(Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        Debug.Log(resultData);
    }
}
