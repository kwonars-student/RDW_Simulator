using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoOneTurnResetter : RotationResetter
{
    public TwoOneTurnResetter() : base() {
        targetAngle = 180;
        ratio = 2;
    }

    //public TwoOneTurnResetter(Space2D space) : base(space) {
    //    targetAngle = 180;
    //    ratio = 2;
    //}
}
