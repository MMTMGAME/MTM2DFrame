using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 让摇杆可以自动吸附手指
/// </summary>
public class Press : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public UnityEvent<int> pressDown;
    public UnityEvent<int> pressHold;
    public UnityEvent<int> pressUp;
    private bool pressed = false;
    private int nowfinger;
    public InputCenter _inputCenter;
    private void Awake()
    {
        _inputCenter = new InputCenter();
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        MyDebug.Log("FingerTest_OnPointerDown_1");
        pressed = true;
        nowfinger = _inputCenter.GetFreeFinger();
         pressDown?.Invoke(nowfinger);
    }
    private void Update()
    {
        if(pressed)
            pressHold?.Invoke(nowfinger);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //MyDebug.Log($"Pressfinger:{nowfinger},_inputCenter.GetFreeFinger():{_inputCenter.GetFreeFinger()}");
        pressed = false;
        pressUp?.Invoke(nowfinger);
    }

    private void OnDisable()
    {
        _inputCenter.OnDestroy();
        _inputCenter = null;
        gameObject.SetActive(false);
    }
}
