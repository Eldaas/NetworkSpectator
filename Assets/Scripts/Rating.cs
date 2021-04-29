using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rating : MonoBehaviour
{
    public Text ratingText;

    public static Rating instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// Updates the player rating text on the local game client - has no effect upon game state or data on other clients.
    /// </summary>
    /// <param name="currentRating"></param>
    public void UpdateRating(int currentRating)
    {
        ratingText.text = "Rating : " + currentRating.ToString();
    }
}
