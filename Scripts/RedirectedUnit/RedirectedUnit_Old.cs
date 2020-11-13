using System.Collections;
using UnityEngine;

public class RedirectedUnit_Old : MonoBehaviour
{
    //protected Redirector redirector;
    //protected Resetter resetter;
    //protected Transform realTransform;
    //protected Transform virtualTransform;
    //public ResultData resultData;
    //protected int id;

    //public virtual void Initialzing(SimulationSetting simulationSetting, int id)
    //{
    //    UnitSetting unitSetting = simulationSetting.unitSettings[id];
    //    this.redirector = unitSetting.GetRedirector();
    //    this.resetter = unitSetting.GetRestter();
    //    this.id = id;

    //    this.redirector.SetReferences(this);
    //    this.resetter.SetReferences(this, simulationSetting.realSpaceSetting.GetSpace2D());

    //    GameObject realObject = Instantiate(simulationSetting.objectSetting.userPrefabs[id], GameObject.Find("Real Space").transform);
    //    GameObject virtualObject = Instantiate(simulationSetting.objectSetting.userPrefabs[id], GameObject.Find("Virtual Space").transform);

    //    this.realTransform = realObject.transform;
    //    this.virtualTransform = virtualObject.transform;

    //    realObject.name = string.Format("Real Unit {0}", id);
    //    virtualObject.name = string.Format("Virtual Unit {0}", id);
    //    this.gameObject.name = string.Format("RDWUnit {0}", id);

    //    if (unitSetting.useRandomStart)
    //    {
    //        float boundX = simulationSetting.realSpaceSetting.size.x / 2 - 1.5f;
    //        float boundY = simulationSetting.realSpaceSetting.size.y / 2 - 1.5f;
    //        float x = Random.Range(-boundX, boundX);
    //        float y = Random.Range(-boundY, boundY);
    //        realTransform.localPosition = new Vector3(x, 0, y);
    //    }
    //    else
    //    {
    //        realTransform.localPosition = Utility.Cast2Dto3D(unitSetting.realStartPosition);
    //    }

    //    virtualTransform.localPosition = Utility.Cast2Dto3D(unitSetting.virtualStartPosition);
    //}




    //public virtual IEnumerator SimulationCoroutine() { yield return new WaitForFixedUpdate(); }


    //public Redirector GetRedirector()
    //{
    //    return redirector;
    //}

    //public Resetter GetResetter()
    //{
    //    return resetter;
    //}

    //public Transform GetRealTransform()
    //{
    //    return realTransform;
    //}

    //public Transform GetVirtualTransform()
    //{
    //    return virtualTransform;
    //}
}
