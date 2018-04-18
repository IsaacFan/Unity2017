using System;
using System.Collections.Generic;
using UnityEngine;

public class InputListener {

    private static InputListener instance;
    public static InputListener Instance
    {
        get
        {
            if (instance == null)
                instance = new InputListener();

            return instance;
        }
    }

    public enum MouseButtonType
    {
        MouseButtonType_Left = 0,
        MouseButtonType_Right = 1,
        MouseButtonType_Middle = 2,

        MouseButtonType_Max,
    }

    public class HotKeyKeyDownSetting
    {
        public KeyCode keyCode;
        public bool isCtrl;
        public bool isShift;
        public bool isAlt;

        public Action callbackAction;
    }
    public class HotKeyKeyPressSetting
    {
        public KeyCode keyCode;
        public Action callback;
    }

    // TODO
    private Action<float, float> dragCallback;                // float: the percentage of screen size. touch drag and mouse darg share this callback
    private bool isLeftMousePress;

    private Action touchCallback;
   
    private List<Action> mouseDownCallbackList = new List<Action>();        // left mouse button, right mouse button, middle mouse button
    private List<Action> mousePressCallbackList = new List<Action>();       // left mouse button, right mouse button, middle mouse button
    private Action<float, float> mouseMoveCallback;
    private Vector3 previousMousePosition;

    private List<HotKeyKeyDownSetting> keyDownListeningList = new List<HotKeyKeyDownSetting>();
    private List<HotKeyKeyPressSetting> keyPressListeningList = new List<HotKeyKeyPressSetting>();  


    public void init()
    {
        for (int i = 0; i < (int)MouseButtonType.MouseButtonType_Max; i++)
        {
            mouseDownCallbackList.Add(null);
            mousePressCallbackList.Add(null);
        }

        GameCore.Instace.registerUpdateAction(update);
    }
    public void destroy()
    {
        GameCore.Instace.unregisterUpdateAction(update);

        keyDownListeningList.Clear();
        keyPressListeningList.Clear();
    }


    



  
    public void update()
    {

        // touch
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (touchCallback != null)
                    touchCallback();
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (dragCallback != null)
                    dragCallback(Input.GetTouch(0).deltaPosition.x, Input.GetTouch(0).deltaPosition.y);
            }
        }


#if UNITY_STANDALONE || DEBUG_MODE
        // mouse
        for (int i = (int)MouseButtonType.MouseButtonType_Left; i < (int)MouseButtonType.MouseButtonType_Max; i++)
        {
            if (Input.GetMouseButtonDown(i) == true)
            {
                if (mouseDownCallbackList[i] != null)
                    mouseDownCallbackList[i]();

                if (i == (int)MouseButtonType.MouseButtonType_Left)
                    isLeftMousePress = true;
            }
            if (Input.GetMouseButton(i) == true)
            {
                if (mousePressCallbackList[i] != null)
                    mousePressCallbackList[i]();
            }
            if (i == (int)MouseButtonType.MouseButtonType_Left)
            {
                if (Input.GetMouseButtonUp(i) == true)
                    isLeftMousePress = false;
            }
        }
        if (Input.mousePosition != previousMousePosition)
        {
            float deltaX = Input.mousePosition.x - previousMousePosition.x;
            float deltaY = Input.mousePosition.y - previousMousePosition.y;

            if (mouseMoveCallback != null)
                mouseMoveCallback(deltaX, deltaY);


            if (isLeftMousePress == true)
            if (dragCallback != null)
                dragCallback(deltaX, deltaY);

            previousMousePosition = Input.mousePosition;
        }


        // keyboard
        if (Input.anyKeyDown == true)
        {
            for (int i = 0; i < keyDownListeningList.Count; i++)
            {
                if (Input.GetKeyDown(keyDownListeningList[i].keyCode) == false)
                    continue;
                if (keyDownListeningList[i].isAlt != Input.GetKey(KeyCode.LeftAlt))
                    continue;
                if (keyDownListeningList[i].isCtrl != Input.GetKey(KeyCode.LeftControl))
                    continue;
                if (keyDownListeningList[i].isCtrl != Input.GetKey(KeyCode.LeftShift))
                    continue;

                keyDownListeningList[i].callbackAction();
            }
        }
        if (Input.anyKey == true)
        {
            for (int i = 0; i < keyPressListeningList.Count; i++)
            {
                if (Input.GetKey(keyPressListeningList[i].keyCode) == false)
                    continue;

                keyPressListeningList[i].callback();
            }
        }


#endif



    }



    public void registerDragCallback(Action<float, float> callback)
    {
        dragCallback += callback;
    }
    public void unregisterDragCallback(Action<float, float> callback)
    {
        dragCallback -= callback;
    }

    public void registerTouchCallback(Action callback)
    {
        touchCallback += callback;
    }
    public void unregisterTouchCallback(Action callback)
    {
        touchCallback -= callback;
    }

    public void registerMouseDownCallback(MouseButtonType mouseButtonType, Action callback)
    {
        mouseDownCallbackList[(int)mouseButtonType] += callback;
    }
    public void unregisterMouseDownCallback(MouseButtonType mouseButtonType, Action callback)
    {
        mouseDownCallbackList[(int)mouseButtonType] -= callback;
    }

    public void registerMousePressCallback(MouseButtonType mouseButtonType, Action callback)
    {
        mousePressCallbackList[(int)mouseButtonType] += callback;
    }
    public void unregisterMousePressCallback(MouseButtonType mouseButtonType, Action callback)
    {
        mousePressCallbackList[(int)mouseButtonType] -= callback;
    }

    public void registerMouseMoveCallback(Action<float, float> callback)
    {
        mouseMoveCallback += callback;
    }
    public void unregisterMouseMoveCallback(Action<float, float> callback)
    {
        mouseMoveCallback -= callback;
    }


    public void registerKeyDownCallback(Action callbackAction, KeyCode keyCode,
                                        bool isCtrl = false, bool isShift = false, bool isAlt = false)
    {
        for (int i = 0; i < keyDownListeningList.Count; ++i)
        {
            if (keyDownListeningList[i].keyCode == keyCode)
                if (keyDownListeningList[i].isAlt == isAlt)
                    if (keyDownListeningList[i].isCtrl == isCtrl)
                        if (keyDownListeningList[i].isShift == isShift)
                        {
                            keyDownListeningList[i].callbackAction += callbackAction;
                            return;
                        }
        }

        HotKeyKeyDownSetting hotKeyKeyDownSetting = new HotKeyKeyDownSetting();
        hotKeyKeyDownSetting.keyCode = keyCode;
        hotKeyKeyDownSetting.isAlt = isAlt;
        hotKeyKeyDownSetting.isCtrl = isCtrl;
        hotKeyKeyDownSetting.isShift = isShift;
        hotKeyKeyDownSetting.callbackAction += callbackAction;
        keyDownListeningList.Add(hotKeyKeyDownSetting);
    }
    public void unregisterKeyDownCallback(Action callbackAction, KeyCode keyCode,
                                          bool isCtrl = false, bool isShift = false, bool isAlt = false)
    {
        for (int i = 0; i < keyDownListeningList.Count; ++i)
        {
            if (keyDownListeningList[i].keyCode == keyCode)
                if (keyDownListeningList[i].isAlt == isAlt)
                    if (keyDownListeningList[i].isCtrl == isCtrl)
                        if (keyDownListeningList[i].isShift == isShift)
                        {
                            keyDownListeningList[i].callbackAction -= callbackAction;
                            if (keyDownListeningList[i].callbackAction == null)
                                keyDownListeningList.RemoveAt(i);

                            return;
                        }
        }

        Debug.LogError("Unregister key down callback error!");
    }

    public void registerKeyPressCallback(Action callback, KeyCode keyCode)
    {
        for (int i = 0; i < keyPressListeningList.Count; ++i)
        {
            if (keyPressListeningList[i].keyCode == keyCode)
            {
                keyPressListeningList[i].callback += callback;
                return;
            }
        }

        HotKeyKeyPressSetting hotKeyKeyPressSetting = new HotKeyKeyPressSetting();
        hotKeyKeyPressSetting.keyCode = keyCode;
        hotKeyKeyPressSetting.callback += callback;
        keyPressListeningList.Add(hotKeyKeyPressSetting);
    }
    public void unregisterKeyPressCallback(Action callbackAction, KeyCode keyCode)
    {
        for (int i = 0; i < keyPressListeningList.Count; ++i)
        {
            if (keyPressListeningList[i].keyCode == keyCode)
            {
                keyPressListeningList[i].callback -= callbackAction;
                if (keyPressListeningList[i].callback == null)
                    keyPressListeningList.RemoveAt(i);

                return;
            }
        }

        Debug.LogError("Unregister key press callback error!");
    }

    


}
