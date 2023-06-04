using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public bool isReversing;
    public float recordTime = 5f;

    private List<PointInTime> _pointsInTime;

    private void Start()
    {
        _pointsInTime = new List<PointInTime>();
    }

    private void FixedUpdate()
    {
        if (!isReversing)
        {
            Record();
        }
        else
        {
            Rewind();
        }
    }

    private void Record()
    {   
        if(_pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            _pointsInTime.RemoveAt(_pointsInTime.Count - 1);
        }

        _pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
    }

    private void Rewind()
    {
        if(_pointsInTime.Count > 0)
        {
            PointInTime pointInTime = _pointsInTime[0];
            transform.position = pointInTime.Position;
            transform.rotation = pointInTime.Rotation;
            _pointsInTime.RemoveAt(0);
        }
        else
        {
            isReversing = false;
        }

    }

}
