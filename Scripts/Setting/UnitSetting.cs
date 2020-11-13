using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitSetting
{
    public enum RedirectType { Null = 0, S2C = 1, SRL = 2, Debug = 3 };
    public enum ResetType { Null = 0, TwoOneTurn = 1, FreezeTurn = 2 };
    public enum UnitType { User = 0, Object = 1 };
    public enum EpisodeType { LongWalk = 0, Random = 1, PreDefined = 2 };

    //public UnitType unitType;
    public RedirectType redirectType;
    public ResetType resetType;
    public EpisodeType episodeType;
    public int episodeLength;
    public string episodeFileName;
    public bool useRandomStart;
    public Vector2 realStartPosition;
    public Vector2 virtualStartPosition;
    private RedirectedUnit unitInstance;


    public RedirectedUnit GetUnit(Space2D realSpace, Space2D virtualSpace)
    {
        if(unitInstance == null)
        {
            if (useRandomStart)
            {
                //float boundX = simulationSetting.realSpaceSetting.size.x / 2 - 1.5f;
                //float boundY = simulationSetting.realSpaceSetting.size.y / 2 - 1.5f;
                //float x = Random.Range(-boundX, boundX);
                //float y = Random.Range(-boundY, boundY);
                //realStartPosition = new Vector2(x, y);
            }

            unitInstance = new RedirectedUnit(GetRedirector(), GetRestter(), GetController(), realSpace, virtualSpace, realStartPosition, virtualStartPosition);
        }

        return unitInstance;
    }

    public SimulationController GetController()
    {
        return new SimulationController(GetEpisode());
    }

    public Redirector GetRedirector() {
        Redirector redirector;

        switch (redirectType) {
            case RedirectType.Debug:
                redirector = new Redirector();
                break;
            case RedirectType.Null:
                redirector = new NullRedirector();
                break;
            case RedirectType.S2C:
                redirector = new S2CRedirector();
                break;
            case RedirectType.SRL:
                redirector = new Redirector();
                break;
            default:
                redirector = new NullRedirector();
                break;
        }

        return redirector;
    }

    public Resetter GetRestter() {
        Resetter resetter;

        switch (resetType) {
            case ResetType.Null:
                resetter = new NullResetter();
                break;
            case ResetType.TwoOneTurn:
                resetter = new TwoOneTurnResetter();
                break;
            case ResetType.FreezeTurn:
                resetter = new FreezeTurnResetter();
                break;
            default:
                resetter = new NullResetter();
                break;
        }

        return resetter;
    }

    public Episode GetEpisode() {
        Episode episode;

        switch (episodeType) {
            case EpisodeType.LongWalk:
                episode = new LongWalkEpisode(episodeLength);
                break;
            case EpisodeType.Random:
                episode = new RandomEpisode(episodeLength);
                break;
            case EpisodeType.PreDefined:
                episode = new PreDefinedEpisode(episodeFileName);
                break;
            default:
                episode = new Episode(episodeLength);
                break;
        }

        return episode;
    }
}
