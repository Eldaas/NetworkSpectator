using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spectator : NetworkBehaviour
{
    NetworkIdentity identity;
    Transform target;

    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
    }

    private void Start()
    {
        CmdSetPlayerRating();
        ResetPlayerInstance();
    }

    private void Update()
    {   
        if (target != null)
        {
            transform.position = target.position;
            if(Input.GetKeyDown(KeyCode.O))
            {
                Debug.Log("Registered O key.");
                RatePlayer(5);
            }
            else if(Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("Registered P key.");
                RatePlayer(-5);
            }
        }
    }

    /// <summary>
    /// Sets necessary references and changes how movement is tracked by the NetworkTransform
    /// Finds and sets the spectator target
    /// </summary>
    /// <param name="identityInstance"></param>
    public void Initialize(NetworkIdentity identityInstance)
    {
        identity = identityInstance;
        GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
        StartCoroutine(SetSpectatorTarget());
    }

    /// <summary>
    /// Rates the player on the local client, setting the local playerRating value. Then updates the UI element before sending a command to the server to update the connected clients.
    /// </summary>
    /// <param name="rating">The value to increase or decrease the local playerRating value by. This is then pushed to the server.</param>
    void RatePlayer(int rating)
    {
        Debug.Log("Rating the player with " + rating);
        Stats.instance.playerRating += rating;
        Rating.instance.UpdateRating(Stats.instance.playerRating);
        CmdRatePlayer(Stats.instance.playerRating);
    }

    /// <summary>
    /// A client side method which tells the server to update every client's tracked player instance.
    /// </summary>
    public void ResetPlayerInstance()
    {
        Debug.Log("Resetting player instance");
        CmdResetPlayerInstance();
    }

    /// <summary>
    /// This command sets the local playerRating value to the passed newRating value, before telling the server to call the RpcRatePlayer method, passing in this new rating value as has been locally set.
    /// </summary>
    /// <param name="newRating">The new rating value to set all clients' versions of the playerRating variable to.</param>
    [Command]
    public void CmdRatePlayer(int newRating)
    {
        Stats.instance.playerRating = newRating;
        RpcRatePlayer(Stats.instance.playerRating);
    }

    /// <summary>
    /// This is the RPC method which gets called by the server to update all clients' playerRating value with the passed argument value.
    /// </summary>
    /// <param name="newRating">The new rating value to set all clients' versions of the playerRating variable to.</param>
    [ClientRpc]
    private void RpcRatePlayer(int newRating)
    {
        if (identity.isLocalPlayer == false)
        {
            Stats.instance.playerRating = newRating;
            Rating.instance.UpdateRating(Stats.instance.playerRating);
        }
    }

    /// <summary>
    /// Tells the server to tell clients to update their rating to the rating referenced on the server.
    /// </summary>
    [Command]
    public void CmdSetPlayerRating()
    {
        RpcSetPlayerRating(Stats.instance.playerRating);
    }

    /// <summary>
    /// Tells connected clients to update the rating being tracked across the network to the passed value.
    /// Updates the UI display to display the new value
    /// </summary>
    /// <param name="rating">The new rating value to set all clients' versions of the playerRating variable to.</param>
    [ClientRpc]
    private void RpcSetPlayerRating(int rating)
    {
        Stats.instance.playerRating = rating;
        Rating.instance.UpdateRating(Stats.instance.playerRating);
    }

    /// <summary>
    /// This function sets the local Stats instance's player value to the passed game object argument. This then tells the server to update all of the other clients via the RpcSetPlayer function.
    /// </summary>
    /// <param name="playerInstance">The GameObject representing the host player instance.</param>
    [Command]
    public void CmdSetPlayer(GameObject playerInstance)
    {
        Stats.instance.player = playerInstance;
        RpcSetPlayer(Stats.instance.player);
    }

    /// <summary>
    /// This is the command which tells the server to call the RpcSetPlayer method, passing in the player instance as has been set in the local Stats instance.
    /// </summary>
    [Command]
    private void CmdResetPlayerInstance()
    {
        RpcSetPlayer(Stats.instance.player);
    }

    /// <summary>
    /// This is the RPC function called by the server to set the Stats instance's player variable to the passed GameObject value.
    /// </summary>
    /// <param name="playerInstance">The GameObject representing the host player instance.</param>
    [ClientRpc]
    private void RpcSetPlayer(GameObject playerInstance)
    {
        Stats.instance.player = playerInstance;
    }

    /// <summary>
    /// Delays the setting of the spectator target to ensure that the network has had an opportunity to sync the required data.
    /// </summary>
    private IEnumerator SetSpectatorTarget()
    {
        while (Stats.instance.player == null)
        {
            yield return new WaitForEndOfFrame();
        }

        target = Stats.instance.player.transform;
        Debug.Log("Target has been set.");
    }

}
