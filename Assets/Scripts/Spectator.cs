using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spectator : NetworkBehaviour
{
    NetworkIdentity identity;
    Transform target;
    Camera cam;

    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
    }

    private void Start()
    {
        CmdSetPlayerRating();
        ResetPlayerInstance();
    }

    public void Initialize(NetworkIdentity identityInstance)
    {
        identity = identityInstance;
        GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
        //Debug.Log(Stats.instance.name);
        StartCoroutine(SetSpectatorTarget());
    }

    private void Update()
    {
        if(target != null)
        {
            transform.position = target.position;
            if(Input.GetKeyDown(KeyCode.O))
            {
                RatePlayer(5);
            }
            else if(Input.GetKeyDown(KeyCode.P))
            {
                RatePlayer(-5);
            }
        }
    }

    void RatePlayer(int rating)
    {
        Stats.instance.playerRating += rating;
        Rating.instance.UpdateRating(Stats.instance.playerRating);
        CmdRatePlayer(Stats.instance.playerRating);
    }

    void ResetPlayerInstance()
    {
        CmdResetPlayerInstance();
    }

    [Command]
    public void CmdRatePlayer(int newRating)
    {
        Stats.instance.playerRating = newRating;
        RpcRatePlayer(Stats.instance.playerRating);
    }

    [ClientRpc]
    void RpcRatePlayer(int newRating)
    {
        if (identity.isLocalPlayer == false)
        {
            Stats.instance.playerRating = newRating;
            Rating.instance.UpdateRating(Stats.instance.playerRating);
        }
    }

    [Command]
    public void CmdSetPlayerRating()
    {
        RpcSetPlayerRating(Stats.instance.playerRating);
    }

    [ClientRpc]
    void RpcSetPlayerRating(int rating)
    {
        Stats.instance.playerRating = rating;
        Rating.instance.UpdateRating(Stats.instance.playerRating);
    }

    [Command]
    public void CmdSetPlayer(GameObject playerInstance)
    {
        Stats.instance.player = playerInstance;
        RpcSetPlayer(Stats.instance.player);
    }

    [Command]
    void CmdResetPlayerInstance()
    {

    }

    [ClientRpc]
    void RpcSetPlayer(GameObject playerInstance)
    {

    }

    IEnumerator SetSpectatorTarget()
    {
        while (Stats.instance.player == null)
        {
            yield return new WaitForEndOfFrame();
        }

        target = Stats.instance.player.transform;
    }

}
