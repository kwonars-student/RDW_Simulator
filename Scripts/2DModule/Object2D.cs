using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object2D
{
    public Transform2D transform;
    public GameObject gameObject;

    public Object2D()
    {
        transform = new Transform2D();
    }

    public Object2D(Object2D geometry)
    {
        transform = new Transform2D(geometry.transform); 
    }

    public Object2D(Vector2 localPosition, Transform2D parent)
    {
        transform = new Transform2D(localPosition, parent);
    }

    public Object2D(Vector2 localPosition, float localRotation, Transform2D parent = null)
    {
        transform = new Transform2D(localPosition, localRotation, parent);
    }

    public Object2D(Vector2 localPosition, float localRotation, Vector2 localScale, Transform2D parent = null)
    {
        transform = new Transform2D(localPosition, localRotation, localScale, parent);
    }

    public override string ToString()
    {
        return string.Format("transform: {0}\n" +
            "GameObject: {1}\n",
            transform, gameObject);
    }

    public void Translate(Vector2 translation, Space relativeTo = Space.Self)
    {
        transform.Translate(translation, relativeTo);
        if (gameObject) gameObject.transform.Translate(Utility.Cast2Dto3D(translation), relativeTo);
    }

    public void Rotate(float degree, Space relativeTo = Space.Self)
    {
        transform.Rotate(degree, relativeTo);
        if (gameObject) gameObject.transform.Rotate(new Vector3(0, -degree, 0), relativeTo);
    }

    public void Instantiate(string name, GameObject prefab, Object2D parentObject)
    {

        Vector3 position = Utility.Cast2Dto3D(transform.localPosition);
        Quaternion rotation = Quaternion.Euler(0, -transform.localRotation, 0);
        gameObject = Object.Instantiate(prefab); // TODO: 잘 복사한건가?
        gameObject.name = name;
        if(parentObject == null)
        {
            gameObject.transform.parent = null;
        }
        else if (parentObject.gameObject != null)
        {
            gameObject.transform.parent = parentObject.gameObject.transform;
        }
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
    }

    public virtual bool IsIntersect(Object2D geometry, float epsilon=0)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool IsInside(Vector2 point, float bound=0)
    {
        return false;
    }

}
