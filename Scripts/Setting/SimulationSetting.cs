using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimulationSetting
{
    //public ObjectSetting_v2 objectSetting;
    public bool useVisualization;
    public bool useDebugMode;
    public PrefabSetting prefabSetting;
    public SpaceSetting realSpaceSetting;
    public SpaceSetting virtualSpaceSetting;
    public UnitSetting[] unitSettings;
    public bool showTarget;
    
}