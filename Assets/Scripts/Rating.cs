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

    public void UpdateRating(int currentRating)
    {
        ratingText.text = "Rating : " + currentRating.ToString();
    }
}
