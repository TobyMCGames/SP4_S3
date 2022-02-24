using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progressbar : MonoBehaviour
{
    [SerializeField] protected Image image;
    private float currentProgress = 0.0f;
    [SerializeField] protected Color high;
    [SerializeField] protected Color mid;
    [SerializeField] protected Color low;
    [SerializeField] protected Text PlayerName;
    bool iscomplete = false;

    private void Update()
    {
        if (iscomplete)
            return;
        image.fillAmount =currentProgress;
        if (currentProgress < 0.3f)
            image.color = low;
        else if (currentProgress > 0.3f && currentProgress < 0.6f)
            image.color = mid;
        else if (currentProgress > 0.6f)
            image.color = high;
        if (currentProgress == 1f) // gameover
        {
            iscomplete = true;
        }
    }

     public  void addProgress(float progress)
    {
        currentProgress += progress;
        image.fillAmount += progress;   
    }

    public void SetPlayerName(string playername)
    {
        PlayerName.text = playername;
    }
    public void setProgress(float progress)
    {
        currentProgress = progress;
        image.fillAmount = progress;
    }
    public string GetPlayerName()
    {
        return PlayerName.text;
    }

    public float getProgress()
    {
        return currentProgress;
    }
}
