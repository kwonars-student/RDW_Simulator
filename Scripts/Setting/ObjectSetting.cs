using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectSetting
{
    public Vector2 position;
    public float rotation;
    public bool useRegularPolygon;
    public int count, size;
    public List<Vector2> vertices;
    //public bool usePredefinedSpace;

    public Object2D GetObject()
    {
        //return new Polygon2D(count, size, position, rotation);

        // TODO: useRegularPolygon 이 OFF일 때도 작동이 되게 
        if (useRegularPolygon)
            return new Polygon2D(count, size, position, rotation);
        else
            return new Polygon2D(vertices, position, rotation);
    }
}
