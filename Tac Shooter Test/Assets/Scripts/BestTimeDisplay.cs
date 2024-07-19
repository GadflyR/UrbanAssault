using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BestTimeDisplay : MonoBehaviour
{
    public TMP_Text[] bestTimes;

    private void Update()
    {
        foreach (TMP_Text bestTime in bestTimes)
        {
            float time = PlayerPrefs.GetFloat($"Level {Array.IndexOf(bestTimes, bestTime)} Best Time");
            if (time <= 0)
                bestTime.text = "Best Time: N/A";
            else
                bestTime.text = "Best Time: " + time.ToString("#.00") + "s";
        }
    }
}
