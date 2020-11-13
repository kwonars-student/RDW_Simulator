using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resetter
{
    protected RedirectedUnit redirectedUnit;
    protected Space2D realSpace;
    public bool isComplete, isFirst;

    public Resetter() {
        redirectedUnit = null;
        realSpace = null;
        isComplete = true;
        isFirst = true;
    }

    public Resetter(RedirectedUser redirectedUser, Space2D space) {
        this.redirectedUnit = redirectedUser;
        realSpace = space;
        isComplete = true;
        isFirst = true;
    }

    public void SetReferences(RedirectedUnit redirectedUnit, Space2D space) {
        this.redirectedUnit = redirectedUnit;
        realSpace = space;
    }

    public virtual void ApplyReset() {

    }

    public bool NeedReset() {
        return !(realSpace.space.IsInside(Utility.Cast3Dto2D(redirectedUnit.GetRealTransform().localPosition)));
    }
}
