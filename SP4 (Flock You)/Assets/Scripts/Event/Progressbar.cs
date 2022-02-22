using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progressbar : MonoBehaviour
{
    [SerializeField] protected Image image;
    private float currentProgress = 100;
    [SerializeField] protected Color high;
    [SerializeField] protected Color mid;
    [SerializeField] protected Color low;


    private void Update()
    {
        image.fillAmount = currentProgress;
        if (currentProgress > 66.6f)
        {
            image.color = high;
        }
        else if (currentProgress >33.3f && currentProgress < 66.6f)
        {
            image.color = mid;
        }
        else
        {
            image.color = low;
        }
    }

     public  void addProgress(float progress)
    {
        currentProgress += progress;
    }
}
