using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultData
{
    private int totalReset, wallReset, userReset;
    private int episodeID, unitID;
    private float sumOfAppliedTranslationGain, sumOfAppliedRotationGain, sumOfAppliedCurvatureGain;
    private float elapsedTime;

    public ResultData() {
        totalReset = wallReset = userReset = 0;
        sumOfAppliedTranslationGain = sumOfAppliedRotationGain = sumOfAppliedCurvatureGain = 0.0f;
        episodeID = unitID = -1;
        elapsedTime = 0;
    }

    public void setEpisodeID(int episodeID)
    {
        this.episodeID = episodeID;
    }

    public void setUnitID(int unitID)
    {
        this.unitID = unitID;
    }

    public void setGains(Redirector.GainType gaintype, float appliedGain)
    {
        switch (gaintype)
        {
            case Redirector.GainType.Translation:
                sumOfAppliedTranslationGain += appliedGain;
                break;
            case Redirector.GainType.Rotation:
                sumOfAppliedRotationGain += appliedGain;
                break;
            case Redirector.GainType.Curvature:
                sumOfAppliedCurvatureGain += appliedGain;
                break;
            default:
                break;
        }
    }

    public void AddElapsedTime(float deltaTime)
    {
        elapsedTime += deltaTime;
    }

    public void AddWallReset()
    {
        wallReset += 1;
        totalReset += 1;
    }

    public override string ToString()
    {
        string result = "";
        result += "----- Result ----\n";
        result += string.Format("UnitID: {0}, EpisodeID: {1}\n", unitID, episodeID);
        result += string.Format("totalReset: {0}, wallReset: {1}, userReset: {2}\n", totalReset, wallReset, userReset);
        result += string.Format("totalTranslationGain: {0}, totalRotationGain: {1}, totalCurvatureGain: {2}\n", sumOfAppliedTranslationGain, sumOfAppliedRotationGain, sumOfAppliedCurvatureGain);
        result += string.Format("elapsedTime: {0}", elapsedTime);

        return result;
    }
}
