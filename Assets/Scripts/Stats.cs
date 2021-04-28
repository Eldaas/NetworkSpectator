using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Stats : NetworkBehaviour
{
    [SyncVar]
    public int playerRating = 0;

    [SyncVar]
    public GameObject player;

    public static Stats instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}
