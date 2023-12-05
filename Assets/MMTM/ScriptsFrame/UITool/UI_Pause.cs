using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Pause : MonoBehaviour
{
    [SerializeField]
    private bool isShowPause = true;
    private void Update()
    {
        if (isShowPause)
        {
            if (gameObject.activeSelf) Time.timeScale = 0;
        }
        else
        {
            if (gameObject.activeSelf) Time.timeScale = 1;
        }
    }


    private void OnDisable()
    {
        if (isShowPause)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        
    }
}
