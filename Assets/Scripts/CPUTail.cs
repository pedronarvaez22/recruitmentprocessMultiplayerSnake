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
            this.transform.position = Vector3.Lerp(this.transform.position, objRef.position, 3 * Time.deltaTime);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, objRef.rotation, 3 * Time.deltaTime);
        }

    }
}
