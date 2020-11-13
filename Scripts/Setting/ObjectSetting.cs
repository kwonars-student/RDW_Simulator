using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectSetting : ScriptableObject {
    public Material realMaterial;
    public Material virtualMaterial;
    public GameObject targetPrefab;
    public GameObject virtualSpacePrefab;
    public GameObject[] userPrefabs;
}
