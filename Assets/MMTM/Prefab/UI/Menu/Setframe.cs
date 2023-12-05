using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setframe : MonoBehaviour
{
    public int targetFrame = 30;
    private void Awake()
    {
        
        Application.targetFrameRate = targetFrame;
    }
    
}
