using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManagement : NetworkManager
{
    public override void OnStopClient()
    {
        Stats.instance.playerRating = 0;
        Rating.instance.ratingText.text = "Rating: " + Stats.instance.playerRating.ToString();
        Stats.instance.player = null;
        base.OnStopClient();
    }

    public override void OnStopHost()
    {
        Stats.instance.playerRating = 0;
        Rating.instance.ratingText.text = "Rating: " + Stats.instance.playerRating.ToString();
        base.OnStopHost();
    }
}
