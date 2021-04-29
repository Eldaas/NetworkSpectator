using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManagement : NetworkManager
{
    /// <summary>
    /// Called when a client disconnects from the server. This resets all data back to default values to ensure no data carries over beyond the current game session.
    /// </summary>
    public override void OnStopClient()
    {
        Stats.instance.playerRating = 0;
        Rating.instance.ratingText.text = "Rating: " + Stats.instance.playerRating.ToString();
        Stats.instance.player = null;
        base.OnStopClient();
    }

    /// <summary>
    /// Called when the host stops/disconnects the server. This resets all data back to default values to ensure no data carries over beyond the current game session.
    /// </summary>
    public override void OnStopHost()
    {
        Stats.instance.playerRating = 0;
        Rating.instance.ratingText.text = "Rating: " + Stats.instance.playerRating.ToString();
        base.OnStopHost();
    }
}
