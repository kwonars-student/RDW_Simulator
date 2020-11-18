using System.Collections.Generic;
using UnityEngine;
using System;

public class RedirectedUnit
{
    public static List<Vector2> debugRealPositionList;
    public static List<Vector2> debugTargetPositionList;
    public static List<Vector2> debugVirtualPositionList;
    protected Redirector redirector;
    protected Resetter resetter;
    public SimulationController controller;
    protected Object2D realUser, virtualUser;
    protected Space2D realSpace, virtualSpace;
    public ResultData resultData;
    static int totalID = 0;
    protected int id;

    //private bool isFirst = true;
    private string status;
    //private bool resetThisUnit = false;
    //private bool resetOtherUnit = false;

    public RedirectedUnit()
    {
        redirector = new Redirector();
        resetter = new Resetter();
        controller = new SimulationController();
        resultData = new ResultData();
        id = -1;
        status = "UNDEFINED"; // TODO: 이래도 되나?
    }

    public RedirectedUnit(Redirector redirector, Resetter resetter, SimulationController controller, Space2D realSpace, Space2D virtualSpace, Vector2 realStartPosition, Vector2 virtualStartPosition)
    {
        this.redirector = redirector;
        this.resetter = resetter;
        this.controller = controller;
        this.realSpace = realSpace;
        this.virtualSpace = virtualSpace;
        this.status = "IDLE";

        resultData = new ResultData();
        resultData.setUnitID(totalID++);
        id = totalID;
        resultData.setEpisodeID(controller.GetEpisodeID());

        realUser = new Circle2D(0.3f, realStartPosition, realSpace.space.transform); // TODO: 알아서 형변환 되게끔 수정
        virtualUser = new Circle2D(0.3f, virtualStartPosition, virtualSpace.space.transform);

        if (debugRealPositionList == null) debugRealPositionList = new List<Vector2>();
        if (debugTargetPositionList == null) debugTargetPositionList = new List<Vector2>();
        if (debugVirtualPositionList == null) debugVirtualPositionList = new List<Vector2>();
    }

    public List<Object2D> GetUsers(RedirectedUnit[] otherUnits)
    {
        List<Object2D> otherUsers = new List<Object2D>();

        for(int i=0; i< otherUnits.Length; i++)
        {
            if (this.id == otherUnits[i].GetID())
                continue;

            otherUsers.Add(otherUnits[i].GetRealUser());
        }

        return otherUsers;
    }

    public string CheckCurrentStatus(RedirectedUnit[] otherUnits)
    {
        List<Object2D> otherUsers = GetUsers(otherUnits);

        if (status == "WALL_RESET")
        {
            if (!resetter.NeedWallReset(realUser, realSpace))
            {
                status = "IDLE";
                resetter.NeedWallReset(realUser, realSpace);
            }
        }
        else if (status == "USER_RESET")
        {
            if (!resetter.NeedUserReset(realUser, otherUsers))
                status = "IDLE";
        }
        else if (status == "IDLE")
        {
            if (resetter.NeedWallReset(realUser, realSpace))
            {
                resultData.AddWallReset();
                status = "WALL_RESET";
            }
            else if (resetter.NeedUserReset(realUser, otherUsers))
            {
                resultData.AddUserReset();
                status = "USER_RESET";
            }
            else if (!GetEpisode().IsNotEnd())
                status = "END";
        }

        return status;
    }

    //public bool NeedUserReset(RedirectedUnit[] otherUnits)
    //{
    //    bool flag = false;

    //    for (int i = 0; i < otherUnits.Length; i++)
    //    {
    //        if (this.id == otherUnits[i].GetID())
    //            continue;

    //        Object2D otherUser = otherUnits[i].GetRealUser();

    //        if (resetter.NeedUserReset(realUser, otherUser))
    //        {
    //            flag = true;
    //        }
    //    }

    //    return flag;
    //}

    public void Simulation(RedirectedUnit[] otherUnits)
    {
        string currentStatus = CheckCurrentStatus(otherUnits);

        //if(RDWSimulationManager.remainTime >= RDWSimulationManager.limitTime)
        //{
        //    foreach(Vector2 p in debugRealPositionList) {
        //        Debug.Log(p);
        //    }

        //    throw new Exception();

        //    //Debug.Log("[Space]");
        //    //Debug.Log("RealSpace: " + realSpace.space.transform);
        //    //Debug.Log("VirtualSpace: " + virtualSpace.space.transform);
        //    //Debug.Log("[User]");
        //    //Debug.Log("RealUser: " + realUser.transform);
        //    //Debug.Log("VirtualUser: " + virtualUser.transform);
        //    //Debug.Log("[Target Position]");
        //    //Debug.Log(GetEpisode().GetTargetPosition());
        //    //Debug.Log("[Current Epsiode]");
        //    //Debug.Log(GetEpisode().GetCurrentEpisodeIndex());
        //    //Debug.Log("[Epsiode Length]");
        //    //Debug.Log(GetEpisode().GetEpisodeLength());
        //    //Debug.Log(resultData);
        //}


        switch (currentStatus)
        {
            case "IDLE":
                Move();
                break;
            case "WALL_RESET":
                ApplyWallReset();
                break;
            case "USER_RESET":
                ApplyUserReset();
                break;
            default:
                break;
        }

        //debugTargetPositionList.Add(GetEpisode().GetTargetPosition());
        //debugVirtualPositionList.Add(virtualUser.transform.localPosition);
        //debugRealPositionList.Add(realUser.transform.localPosition);
        //RDWSimulationManager.remainTime += Time.fixedDeltaTime;
    }

    public void ApplyUserReset()
    {
        resetter.ApplyReset(realUser, virtualUser, realSpace,"User"); // 필요하면 User Reset과 Wall Reset의 방법을 다르게 만들 수 있도록 이런 식으로 구현
    }

    public void ApplyWallReset()
    {
        resetter.ApplyReset(realUser, virtualUser, realSpace,"Wall");
    }

    public void Move()
    {
        (Vector2 deltaPosition, float deltaRotation) = controller.VirtualMove(realUser, virtualUser, virtualSpace); // 가상 유저를 이동 (시뮬레이션)
        (Redirector.GainType type, float degree) = redirector.ApplyRedirection(realUser, deltaPosition, deltaRotation); // 왜곡시킬 값을 계산
        controller.RealMove(realUser, type, degree); // 실제 유저를 이동

        resultData.setGains(type, redirector.GetApplidedGain(type));
        resultData.AddElapsedTime(Time.fixedDeltaTime);
    }

    public int GetID()
    {
        return id;
    }

    public Redirector GetRedirector()
    {
        return redirector;
    }

    public Resetter GetResetter()
    {
        return resetter;
    }

    public Space2D GetRealSpace()
    {
        return realSpace;
    }

    public Space2D GetVirtualSpace()
    {
        return virtualSpace;
    }

    public Object2D GetRealUser()
    {
        return (Circle2D)realUser;

    }

    public Object2D GetVirtualUser()
    {
        return virtualUser;
    }

    public Episode GetEpisode()
    {
        return controller.GetEpisode();
    }
}
