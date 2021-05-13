using System.Collections.Generic;
using UnityEngine;
using System;

public enum FLAG { IDLE, RESET_OCCUR, RESETTING, END }

public class RedirectedUnit
{
    protected Redirector redirector;
    protected Resetter resetter;
    public SimulationController controller;
    public Object2D realUser, virtualUser;
    protected Space2D realSpace, virtualSpace;
    public ResultData resultData;
    static int totalID = 0;
    protected int id;

    private SpaceAgent spaceAgent;
    private int currentTimeStep = 0;

    private string status, previousStatus;
    private Object2D intersectedUser;

    public FLAG flag;

    public RedirectedUnit() // 기본 생성자
    {
        redirector = new Redirector();
        resetter = new Resetter();
        controller = new SimulationController();
        resultData = new ResultData();
        id = -1;
        status = "UNDEFINED"; // TODO: 이래도 되나?
    }

    public RedirectedUnit(Redirector redirector, Resetter resetter, SimulationController controller, Space2D realSpace, Space2D virtualSpace, Object2D realUser, Object2D virtualUser) // 생성자
    {
        this.redirector = redirector;
        this.resetter = resetter;
        this.controller = controller;
        this.realSpace = realSpace;
        this.virtualSpace = virtualSpace;
        this.realUser = realUser;
        this.virtualUser = virtualUser;
        this.status = "IDLE";

        resultData = new ResultData();
        resultData.setUnitID(totalID++);
        id = totalID;
        resultData.setEpisodeID(controller.GetEpisodeID());
    }

    public void Destroy()
    {
        this.redirector = null;
        this.resetter = null;
        this.controller = null;
        this.resultData = null;
        if (virtualSpace != null) this.virtualSpace.Destroy();
        if (realSpace != null)  this.realSpace.Destroy();
        if (realUser != null)  this.realUser.Destroy();
        if (virtualUser != null) this.virtualUser.Destroy();
    }

    public List<Object2D> GetUsers(RedirectedUnit[] otherUnits)
    {
        List<Object2D> otherUsers = new List<Object2D>();

        for (int i = 0; i < otherUnits.Length; i++)
        {
            if (this.id == otherUnits[i].GetID())
                continue;

            otherUsers.Add(otherUnits[i].GetRealUser());
        }

        return otherUsers;
    }

    public string CheckCurrentStatus(RedirectedUnit[] otherUnits, string previousStatus)
    {
        List<Object2D> otherUsers = GetUsers(otherUnits);

        if (status == "WALL_RESET")
        {
            flag = FLAG.RESETTING;

            if (previousStatus == "WALL_RESET_DONE")
            {
                status = "IDLE";
            }
        }
        else if (status == "USER_RESET")
        {
            if (previousStatus == "USER_RESET_DONE")
            {
                status = "IDLE";
            }
        }
        else if (status == "IDLE")
        {
            if (resetter.NeedWallReset(realUser, realSpace))
            {
                resultData.AddWallReset();
                status = "WALL_RESET";
                flag = FLAG.RESET_OCCUR;
                //if (spaceAgent != null) spaceAgent.SetReward(-1.0f);
            }
            else if (resetter.NeedUserReset(realUser, otherUsers, out intersectedUser))
            {
                resultData.AddUserReset();
                status = "USER_RESET";
            }
            else if (!GetEpisode().IsNotEnd())
            {
                status = "END";
                flag = FLAG.END;
                //if (spaceAgent != null) spaceAgent.EndEpisode();
            }
            else
            {
                status = "IDLE";
                flag = FLAG.IDLE;
                //if (spaceAgent != null) spaceAgent.AddReward(+0.005f);
            }
        }

        return status;
    }

    public int i = 1;

    public void Simulation(RedirectedUnit[] otherUnits)
    {
        currentTimeStep += 1;
        //Debug.Log("frame: " + i++);
        string currentStatus = CheckCurrentStatus(otherUnits, previousStatus);
        switch (currentStatus)
        {
            case "IDLE":
                Move();
                break;
            case "WALL_RESET":
                previousStatus = ApplyWallReset();
                break;
            case "USER_RESET":
                previousStatus = ApplyUserReset(intersectedUser);
                break;
            default:
                break;
        }
    }

    public string ApplyUserReset(Object2D otherUser)
    {
        Vector2 resetDirection = (realUser.transform2D.localPosition - otherUser.transform2D.localPosition).normalized;
        return resetter.ApplyUserReset(realUser, resetDirection); // 필요하면 User Reset과 Wall Reset의 방법을 다르게 만들 수 있도록 이런 식으로 구현
    }

    public string ApplyWallReset()
    {
        return resetter.ApplyWallReset(realUser, virtualUser, realSpace);
    }

    public void Move()
    {
        (Vector2 deltaPosition, float deltaRotation) = controller.VirtualMove(virtualUser, virtualSpace); // 가상 유저를 이동 (시뮬레이션)
        (GainType type, float degree) = redirector.ApplyRedirection(this, deltaPosition, deltaRotation); // 왜곡시킬 값을 계산
        controller.RealMove(realUser, type, degree); // 실제 유저를 이동

        resultData.setData(redirector.GetResult(), true);
        resultData.AddElapsedTime(Time.fixedDeltaTime);
    }

    public void DebugDraws(Color userColor)
    {
        realUser.DebugDraw(userColor);
        virtualUser.DebugDraw(userColor);
    }

    public void SetRLAgent(SpaceAgent spaceAgent)
    {
        this.spaceAgent = spaceAgent;
    }

    public SpaceAgent GetRLAgent()
    {
        return spaceAgent;
    }
    
    public int GetCurrentTimeStep()
    {
        return currentTimeStep;
    }

    public int GetID()
    {
        return id;
    }

    public string GetStatus()
    {
        return status;
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
        return realUser;

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
