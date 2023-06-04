using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointInTime
{
    public Vector3 Position;
    public Quaternion Rotation;

    public PointInTime(Vector3 _position, Quaternion _rotation)
    {
        Position = _position;
        Rotation = _rotation;
    }
}
