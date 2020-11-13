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

    public UnitType unitType;
    public RedirectType redirectType;
    public ResetType resetType;
    public EpisodeType episodeType;
    public int episodeLength;
    public bool useRandomStart;
    public Vector2 realStartPosition;
    public Vector2 virtualStartPosition;

    //public RedirectedUnit GetUnit()
    //{
    //    if (useRandomStart)
    //    {
    //        float boundX = realSpaceSetting.size.x / 2 - 1.5f;
    //        float boundY = realSpaceSetting.size.y / 2 - 1.5f;
    //        float x = Random.Range(-boundX, boundX);
    //        float y = Random.Range(-boundY, boundY);
    //        realStartPosition = new Vector3(x, 0, y);
    //    }

    //    if (unitType == UnitType.User)
    //        return new RedirectedUser(GetEpisode(), GetRedirector(), GetRestter(), realStartPosition, virtualStartPosition);
    //    else
    //        return new RedirectedUnit(GetRedirector(), GetRestter(), realStartPosition, virtualStartPosition);
    //}

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
                redirector = new NullRedirector();
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
                resetter = new Resetter();
                break;
            case ResetType.TwoOneTurn:
                resetter = new Resetter();
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
                episode = new PreDefinedEpisode();
                break;
            default:
                episode = new Episode(episodeLength);
                break;
        }

        return episode;
    }
}
