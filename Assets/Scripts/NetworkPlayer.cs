using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        // Player objects are spawned on both the host client and on the spectator client. With two players connected, you have two objects on each client. 
        // This method enables the client executing this code to figure out what to do with these game objects.
        if (identity.isServer)
        {
            Debug.LogError("This is the server.");
            if(identity.isLocalPlayer) // If this is the game object representing the actual player on this particular client.
            {
                if (Stats.instance.player == null)
                {
                    spectator.CmdSetPlayer(gameObject);
                }

                name = "Local - Host";
            }
            else // If this is a game object being spawned on this particular client but which represents another player.
            {
                spectator.ResetPlayerInstance();
                StartCoroutine(SetupRemotePlayer());
            }
            name += " (Server Side)";
        }
        else // If this is not the server
        {
            spectator.ResetPlayerInstance();
            if (identity.isLocalPlayer) // If this is the game object representing the actual player on this particular client.
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

            GetComponent<FreeCamera>().enabled = false;
            //GetComponent<CharacterController>().enabled = false;
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
        Transform camera = transform.GetChild(0);
        Transform redCube = transform.GetChild(1);
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        renderer.enabled = false;
        redCube.gameObject.SetActive(false);
        controller.enabled = false;
        freeCamera.enabled = false;
        tag = "Spectator";

        if (local)
        {
            name = "Local - Spectator";
            camera.transform.position = new Vector3(0, 3, -5);
            camera.transform.eulerAngles = new Vector3(45, 0, 0);
            spectator.enabled = true;
            spectator.Initialize(identity);
        }
        else
        {
            name = "Remote - Spectator";
            camera.gameObject.SetActive(false);
            spectator.enabled = false;
            GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
        }

        enabled = false;
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
