using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpaceRedirector : Redirector
{
    public class ObstacleAction
    {
        public Vector2 translation;
        public float rotation;
        public Vector2 scale;

        public ObstacleAction()
        {
            translation = Vector2.left;
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

    public SpaceRedirector(Space2D virtualSpace)
    {
        obstacleActions = new List<ObstacleAction>();

        foreach (var obstacle in virtualSpace.obstacles)
            obstacleActions.Add(new ObstacleAction());
    }

    public override (GainType, float) ApplyRedirection(RedirectedUnit unit, Vector2 deltaPosition, float deltaRotation)
    {

        (GainType type, float degree) = base.ApplyRedirection(unit, deltaPosition, deltaRotation); // redirector

        Space2D virtualSpace = unit.GetVirtualSpace();
        Object2D virtualUser = unit.virtualUser;

        float fov = 85.0f; // user's field of view
        for(int i=0; i<virtualSpace.obstacles.Count; i++)
        {
            Vector2 obstaclePosition = virtualSpace.obstacles[i].transform2D.localPosition;
            Vector2 userToObstacle = obstaclePosition - virtualUser.transform2D.localPosition;

            if (Vector2.Angle(virtualUser.transform2D.forward, userToObstacle) > fov && !virtualUser.IsIntersect(virtualSpace.obstacles[i])) // obstacle이 user 시야 밖에 있고 user와 충분히 멀다고 판단되는 경우
            {
                Vector2 samplingTranslation = obstacleActions[i].translation * Time.fixedDeltaTime;
                float samplingRotation = obstacleActions[i].rotation * Time.fixedDeltaTime;
                Vector2 samplingScale = obstacleActions[i].scale * Time.fixedDeltaTime;

                virtualSpace.TranslateObstacleByIndex(i, samplingTranslation);
                if (!virtualSpace.IsInside(virtualSpace.obstacles[i], 0.0f, true)) virtualSpace.TranslateObstacleByIndex(i, -samplingTranslation); // space를 벗어나는 action(translation)을 취했을 경우 실제로 Translatrion을 적용하지 않음
                virtualSpace.RotateObstacleByIndex(i, samplingRotation);
                virtualSpace.ScaleObstacleByIndex(i, samplingScale);
            }
        }

        return (type, degree);
    }
}
