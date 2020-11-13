using System.Collections;
using UnityEngine;

public class RedirectedUnit
{
    protected Redirector redirector;
    protected Resetter resetter;
    protected SimulationController controller;
    protected Object2D realUser, virtualUser;
    protected Space2D realSpace, virtualSpace;
    public ResultData resultData;
    static int totalID = 0;
    protected int id;
    private bool isFirst = true, isUserResetFirst = true;


    public RedirectedUnit()
    {
        redirector = new Redirector();
        resetter = new Resetter();
        controller = new SimulationController();
        resultData = new ResultData();
        id = -1;
    }

    public RedirectedUnit(Redirector redirector, Resetter resetter, SimulationController controller, Space2D realSpace, Space2D virtualSpace, Vector2 realStartPosition, Vector2 virtualStartPosition)
    {
        this.redirector = redirector;
        this.resetter = resetter;
        this.controller = controller;
        this.realSpace = realSpace;
        this.virtualSpace = virtualSpace;

        resultData = new ResultData();
        resultData.setUnitID(totalID++);
        id = totalID;
        resultData.setEpisodeID(controller.GetEpisodeID());

        //realUser = new Object2D(realStartPosition, null);
        //virtualUser = new Object2D(virtualStartPosition, null);
        realUser = new Circle2D(0.3f, realStartPosition, null); // TODO: 알아서 형변환 되게끔 수정
        virtualUser = new Circle2D(0.3f, virtualStartPosition, null);

        //this.redirector.SetReferences(this);
        //this.resetter.SetReferences(this);
        //this.controller.SetReferences(this);
    }


    public bool NeedWallReset()
    {
        return resetter.NeedWallReset(realUser, realSpace);
    }

    public bool NeedUserReset(Object2D otherUser)
    {
        return resetter.NeedUserReset(realUser, otherUser);
    }

    public void ApplyWallReset()
    {
        if (isFirst)
        {
            resultData.AddWallReset();
            isFirst = false;
        }

        resetter.ApplyReset(realUser, virtualUser);
    }

    public void ApplyUserReset()
    {
        if (isUserResetFirst)
        {
            resultData.AddWallReset();
            isUserResetFirst = false;
        }

        resetter.ApplyUserReset(realUser, virtualUser);
    }

    private int moveCount = 0;
    private float moveTime = 0;

    public void Move()
    {
        isFirst = true;
        isUserResetFirst = true;

        (Vector2 deltaPosition, float deltaRotation) = controller.VirtualMove(virtualUser, virtualSpace); // 가상 유저를 이동 (시뮬레이션)
        (Redirector.GainType type, float degree) = redirector.ApplyRedirection(realUser, deltaPosition, deltaRotation); // 왜곡시킬 값을 계산
        controller.RealMove(realUser, type, degree); // 실제 유저를 이동

        resultData.setGains(type, redirector.GetApplidedGain(type));
        resultData.AddElapsedTime(Time.fixedDeltaTime);
    }

    //public void Simulation()
    //{
    //    if (resetter.NeedWallReset(realUser, realSpace))
    //    {
    //        if (isFirst)
    //        {
    //            resultData.AddWallReset();
    //            isFirst = false;
    //        }

    //        resetter.ApplyReset(realUser, virtualUser);
    //    }
    //    else if (resetter.NeedUserReset())
    //    {

    //    }
    //    else
    //    {
    //        isFirst = true;

    //        (Vector2 deltaPosition, float deltaRotation) = controller.VirtualMove(virtualUser, virtualSpace);
    //        (Redirector.GainType type, float degree) = redirector.ApplyRedirection(realUser, deltaPosition, deltaRotation);

    //        controller.RealMove(realUser, type, degree);

    //        resultData.setGains(type, redirector.GetApplidedGain(type));
    //        resultData.AddElapsedTime(Time.deltaTime);
    //    }
    //}

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
