using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUTail : MonoBehaviour
{
    public Transform objRef;

    private void Update()
    {
        if(objRef != null)
        {
            transform.position = Vector3.Lerp(transform.position, objRef.position, 3 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, objRef.rotation, 3 * Time.deltaTime);
        }

    }
}
