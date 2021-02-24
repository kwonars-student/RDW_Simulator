using System.Collections.Generic;
using UnityEngine;
using System;

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
    private ArrangementAgent arrangementAgent;
    private int currentTimeStep = 0;

    private bool showResetLocator = false;
    private GameObject resetLocPrefab = null;
    private List<GameObject> resetLocObjects;

    private bool showRealWall = false;
    private GameObject realWallPrefab = null;
    private List<GameObject> realWallObjects;

    private string status, previousStatus;
    private Object2D intersectedUser;

    private bool initialStep;
    private int step;
    private int nextStep;

    private int idleComboCnt;

    public RedirectedUnit() // 기본 생성자
    {
        redirector = new Redirector();
        resetter = new Resetter();
        controller = new SimulationController();
        resultData = new ResultData();
        resetLocObjects = new List<GameObject>();
        realWallObjects = new List<GameObject>();
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

        resetLocObjects = new List<GameObject>();
        realWallObjects = new List<GameObject>();
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
        // if (virtualSpace != null) this.virtualSpace.Destroy();
        // if (realSpace != null)  this.realSpace.Destroy();
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

        if( 
                ( (status == "WALL_RESET" && previousStatus == "WALL_RESET_DONE") ||
                  (status == "USER_RESET" && previousStatus == "USER_RESET_DONE")    )
          )
        {
            if(showResetLocator)
            {
                resetLocObjects.Add(GameObject.Instantiate(resetLocPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Virtual Space").transform));
                resetLocObjects[resetLocObjects.Count - 1].transform.localPosition = virtualUser.gameObject.transform.localPosition + new Vector3(0, 0, 0);
            }
            
            if(showRealWall)
            {
                realWallObjects.Add(GameObject.Instantiate(realWallPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Virtual Space").transform));

                Vector3 realCenterPosition =  Utility.CastVector2Dto3D( Utility.RotateVector2(-realUser.transform2D.localPosition, virtualUser.transform2D.localRotation - realUser.transform2D.localRotation));
                realWallObjects[realWallObjects.Count - 1].transform.localPosition =
                    virtualUser.gameObject.transform.localPosition
                    + realCenterPosition + Utility.CastVector2Dto3D(Utility.CastVector3Dto2D(realCenterPosition).normalized * -0.6f) // Translation Speed 4 -> -0.7f
                     //+ virtualUser.gameObject.transform.forward * realUser.gameObject.transform.localPosition.magnitude
                     //+ virtualUser.gameObject.transform.forward * - 0.7f
                    + new Vector3(0, 2, 0);

                realWallObjects[realWallObjects.Count - 1].transform.localRotation =
                    Utility.CastRotation2Dto3D(
                        virtualUser.transform2D.localRotation - realUser.transform2D.localRotation + 90f
                        );
                // if(realUser.transform2D.localPosition.y >= 0)
                // {
                //     realWallObjects[realWallObjects.Count - 1].transform.localRotation =
                //         Utility.CastRotation2Dto3D(
                //             Utility.CastRotation3Dto2D(virtualUser.gameObject.transform.localRotation)
                //             - Vector2.Angle(Utility.CastVector3Dto2D(realUser.gameObject.transform.localPosition), Vector2.right) + 90f
                //             );

                // }
                // else
                // {
                //     realWallObjects[realWallObjects.Count - 1].transform.localRotation =
                //         Utility.CastRotation2Dto3D(
                //             Utility.CastRotation3Dto2D(virtualUser.gameObject.transform.localRotation)
                //             + Vector2.Angle(Utility.CastVector3Dto2D(realUser.gameObject.transform.localPosition), Vector2.right) - 90f
                //             );
                // }

            }
            
        }

        if (status == "WALL_RESET")
        {
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

                if (spaceAgent != null) spaceAgent.AddReward(-1.0f);
                if(redirector is ArrangementRedirector)
                {
                    if (arrangementAgent != null)
                    {
                        // arrangementAgent.AddReward(-0.01f);
                        // arrangementAgent.AddTotalReward(-0.01f);
                        idleComboCnt = 0;
                        // Debug.Log("Give Reset Reward!");
                    }
                }

            }
            else if (resetter.NeedUserReset(realUser, otherUsers, out intersectedUser))
            {
                resultData.AddUserReset();
                status = "USER_RESET";
            }
            else if (!GetEpisode().IsNotEnd())
            {
                status = "END";
                Debug.Log("EndEpisode!");
                if (spaceAgent != null) spaceAgent.EndEpisode();

                if(redirector is ArrangementRedirector)
                {
                    if (arrangementAgent != null)
                    {
                        // Do Nothing. 이미 다른 곳에서 End처리 함. // arrangementAgent.EndEpisode();
                    }
                }
                
            }
            else
            {
                status = "IDLE";
                if(spaceAgent != null) spaceAgent.AddReward(+0.001f);

                if(redirector is ArrangementRedirector)
                {

                    step = controller.GetEpisode().GetCurrentEpisodeIndex();
                    if(initialStep)
                    {
                        idleComboCnt++;
                        initialStep = false;
                        nextStep = step + 1;
                        if(arrangementAgent != null)
                        {
                            // if(realUser.transform2D.localPosition.x>1.5 || realUser.transform2D.localPosition.x<-1.5 || realUser.transform2D.localPosition.y>1.5 || realUser.transform2D.localPosition.y<-1.5)
                            // {
                            //     arrangementAgent.AddReward(-0.001f);
                            //     arrangementAgent.AddTotalReward(-0.001f);
                            // }
                            // else
                            // {
                            //     arrangementAgent.AddReward(+0.001f);
                            //     arrangementAgent.AddTotalReward(+0.001f);
                            // }
                            if(idleComboCnt > 17)
                            {
                                arrangementAgent.AddReward(+0.01f);
                                arrangementAgent.AddTotalReward(0.01f);
                            }
                            else
                            {
                                arrangementAgent.AddReward(-0.001f);
                                arrangementAgent.AddTotalReward(-0.001f);
                            }


                            // Debug.Log(realUser.transform2D.localPosition);
                        }
                        // Debug.Log("Give Step Reward!");
                    }
                    else if(!initialStep && (nextStep == controller.GetEpisode().GetCurrentEpisodeIndex()))
                    {
                        initialStep = true;
                    }
                }

                
            }
        }

        return status;
    }

    public void Simulation(RedirectedUnit[] otherUnits)
    {
        currentTimeStep += 1;
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

    private int i = 0;
    public void Move()
    {
        //if (i % 10 == 0) spaceAgent.RequestDecision();
        //Debug.Log(spaceAgent.StepCount);
        //Debug.Log(i++);
        //Debug.Log(spaceAgent.CompletedEpisodes);
        //Debug.Log(spaceAgent.GetCumulativeReward());
        //Debug.Log("");
        //if (i == 100) spaceAgent.EndEpisode();
        
        // (GainType type, float degree) = redirector.ApplyRedirection(this, Vector2.zero, 0f); // 왜곡시킬 값을 계산
        // (Vector2 deltaPosition, float deltaRotation) = controller.VirtualMove(virtualUser, virtualSpace); // 가상 유저를 이동 (시뮬레이션)
        // (type, degree) = redirector.ApplyRedirection(this, deltaPosition, deltaRotation); // 왜곡시킬 값을 계산

        if(redirector is ArrangementRedirector)
        {
            ArrangementRedirector arrangementRedirector = (ArrangementRedirector) redirector;
            arrangementRedirector.ObstacleArrangement(this); // VirtualMove 전에 호출하여 virtualSpace의 배치 변형
        }

        Vector2 deltaPosition = new Vector2(0,0);
        float deltaRotation = 0f;
        if(virtualSpace.tileMode)
        {
            (deltaPosition, deltaRotation) = controller.VirtualMove(virtualUser, virtualSpace, realUser, realSpace); // 가상 유저를 이동 (시뮬레이션)
        }
        else
        {
            (deltaPosition, deltaRotation) = controller.VirtualMove(virtualUser, virtualSpace); // 가상 유저를 이동 (시뮬레이션)
        }

        (GainType type, float degree) = redirector.ApplyRedirection(this, deltaPosition, deltaRotation); // 왜곡시킬 값을 계산
        controller.RealMove(realUser, type, degree); // 실제 유저를 이동

        if(redirector is GainRedirector)
        {
            resultData.setGains(type, ((GainRedirector) redirector).GetApplidedGain(type));
        }

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

    public void SetRLArrangementAgent(ArrangementAgent arrangementAgent)
    {
        this.arrangementAgent = arrangementAgent;
    }
    
    public SpaceAgent GetRLAgent()
    {
        return spaceAgent;
    }

    public ArrangementAgent GetRLArrangementAgent()
    {
        return arrangementAgent;
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

    public void SetShowResetLocator(bool showResetLocator)
    {
        this.showResetLocator = showResetLocator;
    }

    public void SetResetLocPrefab(GameObject resetLocPrefab)
    {
        this.resetLocPrefab = resetLocPrefab;
    }

    public void DeleteResetLocObjects()
    {
        for(int i = 0; i < resetLocObjects.Count ; i++)
        {
            GameObject.Destroy(resetLocObjects[i]);
        }
        resetLocObjects = null;
    }

    public void GenerateResetLocObjects()
    {
        resetLocObjects = new List<GameObject>();
    }

    public int GetNumOfResetLocObjects()
    {
        return resetLocObjects.Count;
    }

    public void SetInitialStep(bool initialStep)
    {
        this.initialStep = initialStep;
    }

    public void SetShowRealWall(bool showRealWall)
    {
        this.showRealWall = showRealWall;
    }

    public void SetRealWallPrefab(GameObject realWallPrefab)
    {
        this.realWallPrefab = realWallPrefab;
    }

    public void DeleteRealWallObjects()
    {
        for(int i = 0; i < realWallObjects.Count ; i++)
        {
            GameObject.Destroy(realWallObjects[i]);
        }
        realWallObjects = null;
    }

    public void GenerateRealWallObjects()
    {
        realWallObjects = new List<GameObject>();
    }
}
