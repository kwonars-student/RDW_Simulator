using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimulationSetting
{
    //public ObjectSetting_v2 objectSetting;
    public bool useVisualization;
    public bool useDebugMode;
    public bool useContinousSimulation;
    public bool arrangementRLMode;
    public PrefabSetting prefabSetting;
    public SpaceSetting realSpaceSetting;
    public SpaceSetting virtualSpaceSetting; // 기존 세팅과 동일하고자 할 때 적용.
    public SpaceTilingSetting virtualSpaceTilingSetting; // 타일링 활용 시 설정 적용.
    public UnitSetting[] unitSettings;
    public bool showTarget;
    public bool showResetLocator;
    public bool showRealWall;
    
}