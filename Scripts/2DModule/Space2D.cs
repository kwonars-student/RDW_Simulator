using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space2D {
    public Shape2D space;
    public List<Shape2D> obstacles;

    public Space2D() {
        space = null;
        obstacles = null;
    }

    public Space2D(List<Vector2> corners, Vector2 originPosition, float originRotation)
    {
        space = new Plane2D(corners, originPosition, originRotation);
        obstacles = null;
    }

    public Space2D(Vector2 size, Vector2 originPosition, float originRotation, string type = "plane") {
        if (type == "plane")
            space = new Plane2D(size, originPosition, originRotation); // 사각형 생성
        else
            space = new Circle2D(); // 원 생성

        obstacles = null;
    }


}
