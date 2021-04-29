using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar]
    public float currentSpeed = 1000;

    private Spectator spectator;
    private NetworkIdentity identity;
    private CharacterController controller;
    private FreeCamera freeCamera;

    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
        controller = GetComponent<CharacterController>();
        spectator = GetComponent<Spectator>();
        freeCamera = GetComponent<FreeCamera>();
    }

    private void Start()
    {
        NetworkSetup();
    }

    private void Update()
    {
        if(Input.GetAxis("Vertical") > 0)
        {
            controller.Move(transform.forward * currentSpeed * Time.deltaTime);
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            controller.Move(-transform.forward * currentSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Checks if the identity is the server and calls the relevant methods to set the player up as either the host or spectator.
    /// </summary>
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
            if (identity.isLocalPlayer)
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

    /// <summary>
    /// Coroutine probes server for player reference and waits for a response before setting up appropriate remote player type.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Sets the object properties depending on if the gameobject is local or remote spectator.
    /// </summary>
    /// <param name="local"></param>
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
            freeCamera.enabled = false;
        }
        else
        {
            name = "Remote - Spectator";
            renderer.enabled = false;
            child.gameObject.SetActive(false);
            GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
            enabled = false;
            controller.enabled = false;
            freeCamera.enabled = false;
            tag = "Spectator";

        }
    }

    /// <summary>
    /// Sends a command to the server to tell the clients to print a debug message to their respective consoles when a player joins the game.
    /// </summary>
    [Command]
    public void CmdTellServer()
    {
        RpcTellClients($"{gameObject.name} has joined");
    }

    /// <summary>
    /// This is the implementation of the RPC call as a result of the CmdTellServer command.
    /// </summary>
    /// <param name="message"></param>
    [ClientRpc]
    public void RpcTellClients(string message)
    {
        Debug.Log(message);
    }
}
