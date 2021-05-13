using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainSetting
{
    public int n_agent;
    public TrainSpaceSetting realSpaceSetting;
    public TrainSpaceSetting virtualSpaceSetting;
    public UnitSetting unitSetting;
}
