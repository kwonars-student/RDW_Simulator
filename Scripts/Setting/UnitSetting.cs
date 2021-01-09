using System;
using UnityEngine;

public enum RedirectType { Null, Default, S2C, Space, Arrangement };
public enum ResetType { Default, TwoOneTurn , FreezeTurn, CenterTurn };
public enum EpisodeType { LongWalk, Random, PreDefined, Wandering };

[System.Serializable]
public class UnitSetting
{
    public RedirectType redirectType;
    public ResetType resetType;
    public EpisodeType episodeType;
    public int episodeLength;
    public string episodeFileName;

    public bool useRandomStartReal;
    public bool useRandomStartVirtual;
    public GameObject userPrefab;
    public float userStartRotation;
    public float realStartRotation;
    public float virtualStartRotation;
    public Vector2 realStartPosition;
    public Vector2 virtualStartPosition;
    public float translationSpeed;
    public float rotationSpeed;

    public RedirectedUnit GetUnit(Space2D realSpace, Space2D virtualSpace)
    {
        Object2D realUser, virtualUser;

        if (useRandomStartReal)
        {
            realStartPosition = realSpace.GetRandomPoint(0.2f);
            realStartRotation = Utility.sampleUniform(0f, 360f);
        }

        if (useRandomStartVirtual)
        {
            virtualStartPosition = virtualSpace.GetRandomPoint(0.2f);
            virtualStartRotation = Utility.sampleUniform(0f, 360f);
        }

        if (userPrefab != null)
        {
            switch (userPrefab.tag) // 좀더 깔끔한 코드 있을 꺼 같은데 (추상화 가능성)
            {
                default:
                    realUser = new Polygon2DBuilder().SetPrefab(userPrefab).SetLocalPosition(realStartPosition).SetLocalRotation(realStartRotation).SetParent(realSpace.spaceObject).Build();
                    virtualUser = new Polygon2DBuilder().SetPrefab(userPrefab).SetLocalPosition(virtualStartPosition).SetLocalRotation(virtualStartRotation).SetParent(virtualSpace.spaceObject).Build();
                    break;
                case "Circle":
                    realUser = new Circle2DBuilder().SetPrefab(userPrefab).SetLocalPosition(realStartPosition).SetLocalRotation(realStartRotation).SetParent(realSpace.spaceObject).Build();
                    virtualUser = new Circle2DBuilder().SetPrefab(userPrefab).SetLocalPosition(virtualStartPosition).SetLocalRotation(virtualStartRotation).SetParent(virtualSpace.spaceObject).Build();
                    break;
            }
        }
        else
        {
            realUser = new Circle2DBuilder().SetLocalPosition(realStartPosition).SetLocalRotation(realStartRotation).SetRadius(0.5f).SetParent(realSpace.spaceObject).Build();
            virtualUser = new Circle2DBuilder().SetLocalPosition(virtualStartPosition).SetLocalRotation(virtualStartRotation).SetRadius(0.5f).SetParent(virtualSpace.spaceObject).Build();
        }

        return new RedirectedUnitBuilder()
            .SetController(GetController())
            .SetRedirector(GetRedirector())
            .SetResetter(GetRestter())
            .SetRealSpace(realSpace)
            .SetVirtualSpace(virtualSpace)
            .SetRealUser(realUser)
            .SetVirtualUser(virtualUser)
            .Build();
    }

    public SimulationController GetController()
    {
        return new SimulationController(GetEpisode(), translationSpeed, rotationSpeed);
    }

    public Redirector GetRedirector()
    {
        Redirector redirector;

        switch (redirectType)
        {
            case RedirectType.S2C:
                redirector = new S2CRedirector();
                break;
            case RedirectType.Space:
                redirector = new SpaceRedirector();
                break;
            case RedirectType.Arrangement:
                redirector = new ArrangementRedirector();
                break;
            case RedirectType.Null:
                redirector = new NullRedirector();
                break;
            default:
                redirector = new Redirector();
                break;
        }

        return redirector;
    }

    public Resetter GetRestter()
    {
        Resetter resetter;

        switch (resetType)
        {
            case ResetType.TwoOneTurn:
                resetter = new TwoOneTurnResetter(translationSpeed, rotationSpeed);
                break;
            case ResetType.FreezeTurn:
                resetter = new FreezeTurnResetter(translationSpeed, rotationSpeed);
                break;
            case ResetType.CenterTurn:
                resetter = new CenterTurnResetter(translationSpeed, rotationSpeed);
                break;
            default:
                resetter = new Resetter(translationSpeed, rotationSpeed);
                break;
        }

        return resetter;
    }

    public Episode GetEpisode()
    {
        Episode episode;

        switch (episodeType)
        {
            case EpisodeType.LongWalk:
                episode = new LongWalkEpisode(episodeLength);
                break;
            case EpisodeType.Random:
                episode = new RandomEpisode(episodeLength);
                break;
            case EpisodeType.PreDefined:
                episode = new PreDefinedEpisode(episodeFileName);
                break;
            case EpisodeType.Wandering:
                episode = new WanderingEpisode(episodeLength);
                break;
            default:
                episode = new Episode(episodeLength);
                break;
        }

        return episode;
    }
}
