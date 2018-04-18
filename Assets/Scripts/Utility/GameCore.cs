using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour {

    private static GameCore instance;
    public static GameCore Instace
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = GameObject.Find("GameCore");
                if (gameObject == null)
                {
                    gameObject = new GameObject("GameCore");
                    instance = gameObject.AddComponent<GameCore>();
                }
                else
                {
                    instance = gameObject.GetComponent<GameCore>();
                    if (instance == null)
                        instance = gameObject.AddComponent<GameCore>();
                }
            }

            return instance;
        }
    }

    private Action updateAction;


	// Use this for initialization
	void Awake () {

        TimerManager.Instace.init();
        InputListener.Instance.init();
        AudioManager.Instance.init();

	}
    void OnDestroy() {
        TimerManager.Instace.destroy();
        InputListener.Instance.destroy();
        AudioManager.Instance.destroy();

    }

    // Update is called once per frame
    void Update () {

        updateAction();

    }

    public void registerUpdateAction(Action updateAction)
    {
        this.updateAction += updateAction;
    }
    public void unregisterUpdateAction(Action updateAction)
    {
        this.updateAction -= updateAction;
    }


}
