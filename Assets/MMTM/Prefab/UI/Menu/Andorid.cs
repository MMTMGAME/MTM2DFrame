using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Andorid : MonoBehaviour
{
    private void Start()
    {
#if UNITY_ANDROID
                gameObject.SetActive(true);
                Application.targetFrameRate = 30;
#elif UNITY_STANDALONE_WIN
        gameObject.SetActive(false);
        Application.targetFrameRate = 60;
#endif
    }
}
