using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class InputCenter
{
    private MultiTouches multiTouches;
    private List<int> activetouchid = new List<int>();
    private int fingerid;

    public InputCenter()
    {
        multiTouches = new MultiTouches();
        multiTouches.Enable();

        multiTouches.touches.Finger_1.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_1_Press");
            activetouchid.Add(1);
        };

        multiTouches.touches.Finger_1.canceled += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_1_Cancel");
            activetouchid.Remove(1);
        };
        
        multiTouches.touches.Finger_2.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_2_Press");
            activetouchid.Add(2);
        };
        
        multiTouches.touches.Finger_2.canceled += (a) =>
        {
            
            activetouchid.Remove(2);
        };
        
        multiTouches.touches.Finger_3.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_3_Press");
            activetouchid.Add(3);
        };
        
        multiTouches.touches.Finger_3.canceled += (a) =>
        {
            activetouchid.Remove(3);
        };
        
        multiTouches.touches.Finger_4.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_4_Press");
            activetouchid.Add(4);
        };
        
        multiTouches.touches.Finger_4.canceled += (a) =>
        {
            activetouchid.Remove(4);
        };
        
        multiTouches.touches.Finger_5.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_5_Press");
            activetouchid.Add(5);
        };
        
        multiTouches.touches.Finger_5.canceled += (a) =>
        {
            activetouchid.Remove(5);
        };
        
        multiTouches.touches.Finger_6.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_6_Press");
            activetouchid.Add(6);
        };
        
        multiTouches.touches.Finger_6.canceled += (a) =>
        {
            activetouchid.Remove(6);
        };
        
        multiTouches.touches.Finger_7.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_7_Press");
            activetouchid.Add(7);
        };
        
        multiTouches.touches.Finger_7.canceled += (a) =>
        {
            activetouchid.Remove(7);
        };
        
        multiTouches.touches.Finger_8.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_8_Press");
            activetouchid.Add(8);
        };
        
        multiTouches.touches.Finger_8.canceled += (a) =>
        {
            activetouchid.Remove(8);
        };
        
        multiTouches.touches.Finger_9.started += (a) =>
        {
            MyDebug.Log("FingerTest_Finger_9_Press");
            activetouchid.Add(9);
        };
        
        multiTouches.touches.Finger_9.canceled += (a) =>
        {
            activetouchid.Remove(9);
        };
    }
    public InputAction GetfingerAction(int fingerid)
    {
        switch (fingerid)
        {
            case 1: return multiTouches.touches.Finger_pos_1;
            case 2: return multiTouches.touches.Finger_pos_2;
            case 3: return multiTouches.touches.Finger_pos_3;
            case 4: return multiTouches.touches.Finger_pos_4;
            case 5: return multiTouches.touches.Finger_pos_5;
            case 6: return multiTouches.touches.Finger_pos_6;
            case 7: return multiTouches.touches.Finger_pos_7;
            case 8: return multiTouches.touches.Finger_pos_8;
            case 9: return multiTouches.touches.Finger_pos_9;
        }
        return null;
    }

    public int GetFreeFinger()
    {
        MyDebug.Log($"activetouchid:{activetouchid==null},count:{activetouchid.Count}");
       return activetouchid[activetouchid.Count-1];
    }
    /// <summary>
    /// 获取射击点
    /// </summary>
    /// <returns></returns>
    public Vector2 GetTouchMousePoint()
    {
        

//如果是电脑
        //return (Vector2)Camera.main.ScreenToWorldPoint(multiTouches.Daifu.MousePos.ReadValue<Vector2>());
//如果是安卓
        // var shootdirection = multiTouches.instance.M_PlayerInput.Daifu.ShootDirection.ReadValue<Vector2>().normalized;
        // if (shootdirection == Vector2.zero) //如果什么都没操作默认打右边
        // {
        //     shootdirection = Vector2.right;
        // }
        // return GameCenter.instance.Player.M_BodyOffset.body_centerbone.position + (Vector3)shootdirection * 40;

        return Vector2.zero;
    }
    /// <summary>
    /// 获取射击方向
    /// </summary>
    /// <returns></returns>
    public Vector2 GetTouchMouseDirection()
    {
        // var shootdirection = GameCenter.instance.M_PlayerInput.Daifu.ShootDirection.ReadValue<Vector2>().normalized;
        // if (shootdirection == Vector2.zero) //如果什么都没操作默认打右边
        // {
        //     shootdirection = Vector2.right;
        // }
        // //MyDebug.Log($"获取射击方向:{shootdirection}");
        // return shootdirection;
        return Vector2.zero;
        //如果是电脑

        // var mousepos = (Vector2)Camera.main.ScreenToWorldPoint(GameCenter.instance.M_PlayerInput.Daifu.MousePos.ReadValue<Vector2>());
        // var direction = (mousepos - (Vector2)GameCenter.instance.Player.M_BodyOffset.body_centerbone.position).normalized;
        // MyDebug.Log($"获取射击方向:{direction}");
        // return  direction;

        //----------------------
        //如果是安卓
        // var shootdirection = GameCenter.instance.M_PlayerInput.Daifu.ShootDirection.ReadValue<Vector2>().normalized;
        // if (shootdirection == Vector2.zero) //如果什么都没操作默认打右边
        // {
        //     shootdirection = Vector2.right;
        // }
        // MyDebug.Log($"获取射击方向:{shootdirection}");
        // return shootdirection;

    }

    public void OnDestroy()
    {
        multiTouches.Disable();
        multiTouches.Dispose();
    }
}

