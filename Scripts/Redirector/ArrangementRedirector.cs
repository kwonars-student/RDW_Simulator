using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ArrangementRedirector : Redirector
{
    public ArrangementAgent arrangementAgent;
    private bool executed = false;
    private bool ready = true;
    private int cnt = 0;
    private List<Vector2> initialObstaclePositions;


    public class ObstacleAction
    {
        public Vector2 translation;
        public float rotation;
        public Vector2 scale;

        public ObstacleAction()
        {
            translation = Vector2.zero;
            rotation = 0;
            scale = Vector2.zero;
        }

        public void setObstacleAction(Vector2 translation, float rotation, Vector2 scale)
        {
            this.translation = translation;
            this.rotation = rotation;
            this.scale = scale;
        }
    }

    public List<ObstacleAction> obstacleActions;

    public override (GainType, float) ApplyRedirection(RedirectedUnit unit, Vector2 deltaPosition, float deltaRotation)
    {
        (GainType type, float degree) = base.ApplyRedirection(unit, deltaPosition, deltaRotation); // redirector
        return (type, degree);
    }
    
    public void SetRLArrangementAgent(ArrangementAgent arrangementAgent)
    {
        this.arrangementAgent = arrangementAgent;
    }

    public void SetRedirectorReady()
    {
        executed = false;
        ready = true;
        cnt = 0;
    }

    public void ObstacleArrangement(RedirectedUnit unit)
    {
                // space manipulation 
        Space2D virtualSpace = unit.GetVirtualSpace();
        Object2D virtualUser = unit.virtualUser;
        initialObstaclePositions = virtualSpace.GetInitialObstaclePositions();

        if (obstacleActions == null)
        {
            obstacleActions = new List<ObstacleAction>();

            foreach (var obstacle in virtualSpace.obstacles)
                obstacleActions.Add(new ObstacleAction());

        }

        if(!executed && ready && cnt < 2)
        {

            if(cnt == 1)
            {

                for (int i=0; i<virtualSpace.obstacles.Count; i++)
                {
                    //Transform2D virtualUserTransform = virtualUser.transform2D;
                    //Episode episode = unit.GetEpisode();
                    //Vector2 targetPosition = episode.GetTarget(virtualUserTransform, virtualSpace);

                    // Debug.Log("Virtual User Position : "+ virtualUser.transform2D.localPosition);
                    // Debug.Log("Target Position : "+ targetPosition);

                    Vector2 samplingTranslation = obstacleActions[i].translation;
                    float samplingRotation = obstacleActions[i].rotation;
                    Vector2 samplingScale = obstacleActions[i].scale;

                    Vector2 displacement = initialObstaclePositions[i] + samplingTranslation;

                    // Obstacle 움직이기
                    virtualSpace.JumpObstacleByIndex(i, displacement);
                    virtualSpace.RotateObstacleByIndex(i, samplingRotation);
                    
                    // if (virtualSpace.obstacles[i].IsInside(virtualUser, 0.0f))
                    // // if (virtualSpace.IsInside(virtualUser, 0.0f) && !virtualSpace.IsPossiblePath(virtualUser.transform2D.localPosition, targetPosition, Space.Self))
                    // // if (virtualSpace.obstacles[i].IsInside(virtualUser, 0.0f) && virtualSpace.IsInside(virtualUser, 0.0f) && !virtualSpace.IsPossiblePath(virtualUser.transform2D.localPosition, targetPosition, Space.Self))
                    // {
                    //     unit.GetEpisode().SetWrongEpisode(true);
                    //     //virtualSpace.JumpObstacleByIndex(i, initialObstaclePositions[i]);
                    //     // arrangementAgent.AddReward(0.32f); // 바로 끝나는 경우이므로 평균 Reward값을 주고 끝냄.
                    //     // Debug.Log("Give Collision Reward!");
                    //     unit.GetEpisode().SetCurrentEpisodeIndex(unit.GetEpisode().GetEpisodeLength());
                    // }

                    // if (!virtualSpace.IsInside(virtualSpace.obstacles[i], 0.0f))
                    // {
                    //     virtualSpace.JumpObstacleByIndex(i, initialObstaclePositions[i]);
                    //     arrangementAgent.AddReward(-1f);
                    //     unit.GetEpisode().SetCurrentEpisodeIndex(unit.GetEpisode().GetEpisodeLength());
                    //     // Debug.Log("Give Outside Reward!");
                    // }


                }
            }
            cnt++;

            if (cnt >= 2)
            {
                executed = true;
                ready = false;
            }

        }
        else
        {
            if(!ready && unit.GetEpisode().GetCurrentEpisodeIndex()+1 == unit.GetEpisode().GetEpisodeLength())
            {
                ready = true;
            }
            else if(ready && unit.GetEpisode().GetCurrentEpisodeIndex() == 0)
            {
                executed = false;
                cnt = 0;
            }
        }
    }
}
