using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PrefabSetting : ScriptableObject
{
    public List<GameObject> userPrefabs;
    public GameObject targetPrefabs;
    //public GameObject realSpacePrefab;
    //public GameObject virtualSpacePrefab;
    public Material realMaterial;
    public Material virtualMaterial;
}
