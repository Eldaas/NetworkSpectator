using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar]
    public float currentSpeed = 2;

    private void Start()
    {
        if (!isLocalPlayer) 
        {
            transform.GetChild(0).gameObject.SetActive(false); 
            RpcTellServer();
        }
    }

    [ClientRpc]
    public void RpcTellServer()
    {
        CmdTellClients($"{gameObject.name} has joined");
    }

    [Command]
    public void CmdTellClients(string message)
    {
        Debug.Log(message);
    }
}
