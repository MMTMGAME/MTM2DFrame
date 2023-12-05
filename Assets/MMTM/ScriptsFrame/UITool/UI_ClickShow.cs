using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ClickShow : MonoBehaviour
{
    private bool isShow = false;

    [SerializeField]
    private GameObject activeAim;

    private void Awake()
    {
        if (activeAim == null) activeAim = gameObject;
    }

    public void OnShow()
    {
        isShow = !isShow;
        
        if(isShow)activeAim.SetActive(true);
        else
        {
            activeAim.SetActive(false);
        }
    }
}
