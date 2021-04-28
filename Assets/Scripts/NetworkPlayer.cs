using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar]
    public float currentSpeed = 2;

    private Spectator spectator;
    private NetworkIdentity identity;
    private CharacterController controller;

    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
        controller = GetComponent<CharacterController>();
        spectator = GetComponent<Spectator>();
    }

    private void Start()
    {
        NetworkSetup();
    }

    private void Update()
    {
        if(Input.GetAxis("Vertical") > 0)
        {
            controller.Move(Vector3.forward * currentSpeed * Time.deltaTime);
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            controller.Move(-Vector3.forward * currentSpeed * Time.deltaTime);
        }
    }

    private void NetworkSetup()
    {
        if (identity.isServer) 
        {
            if (identity.isLocalPlayer)
            {
                name = "Local - Host";
                if (Stats.instance.player == null)
                {
                    spectator.CmdSetPlayer(gameObject);
                }
            }
            else
            {
                spectator.ResetPlayerInstance();
                StartCoroutine(SetupRemotePlayer());
            }
            name += " (Server Side)";
        }
        else
        {
            spectator.ResetPlayerInstance();
            if (identity.isLocalPlayer == true)
            {
                SetupSpectator(true);
            }
            else
            {
                StartCoroutine(SetupRemotePlayer());
            }
            name += " (Client Side)";
        }
    }

    private IEnumerator SetupRemotePlayer()
    {
        while (Stats.instance.player == null)
        {
            yield return new WaitForEndOfFrame();
        }

        if (gameObject == Stats.instance.player)
        {
            name = "Remote - Host";
            transform.GetChild(0).gameObject.SetActive(false);
            enabled = false;
        }
        else
        {
            SetupSpectator(false);
        }
    }

    private void SetupSpectator(bool local)
    {
        Transform child = transform.GetChild(0);
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (local)
        {
            name = "Local - Spectator";
            child.transform.position = new Vector3(0, 3, -5);
            child.transform.eulerAngles = new Vector3(45, 0, 0);
            renderer.enabled = true;
            spectator.enabled = true;
            tag = "Spectator";
            spectator.Initialize(identity);
            enabled = false;
            controller.enabled = false;
            

        }
        else
        {
            name = "Remote - Spectator";
            renderer.enabled = false;
            child.gameObject.SetActive(false);
            GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
            enabled = false;
            controller.enabled = false;
            tag = "Spectator";

        }
    }

    [Command]
    public void CmdTellServer()
    {
        RpcTellClients($"{gameObject.name} has joined");
    }

    [ClientRpc]
    public void RpcTellClients(string message)
    {
        Debug.Log(message);
    }
}
