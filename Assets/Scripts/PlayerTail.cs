using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTail : MonoBehaviour
{
    public GameObject playerRef;

    public void TailID(GameObject playerRefExt)
    {
        playerRef = playerRefExt;
    }
}
