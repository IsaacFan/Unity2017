using System;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Rigidbody rb;
    private Action<GameObject> deleteEnemyCallback;
    private Action addOnBulletCallback;
    private Action decreaseOneHeartCallback;
    private Action useSkillCallback;
    private bool isActive;


	public void init(Action<GameObject> deleteEnemyCallback, Action addOnBulletCallback, Action decreaseOneHeartCallback,
                     Action useSkillCallback)
    {
        this.deleteEnemyCallback = deleteEnemyCallback;
        this.addOnBulletCallback = addOnBulletCallback;
        this.decreaseOneHeartCallback = decreaseOneHeartCallback;
        this.useSkillCallback = useSkillCallback;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();


        InputListener.Instance.registerMouseDownCallback(InputListener.MouseButtonType.MouseButtonType_Left, onMouseDownCallback);
        InputListener.Instance.registerMouseMoveCallback(onMouseMoveCallback);

        InputListener.Instance.registerKeyPressCallback(onForwardKeyPressCallback, KeyCode.W);
        InputListener.Instance.registerKeyPressCallback(onForwardKeyPressCallback, KeyCode.UpArrow);
        InputListener.Instance.registerKeyPressCallback(onBackwardKeyPressCallback, KeyCode.S);
        InputListener.Instance.registerKeyPressCallback(onBackwardKeyPressCallback, KeyCode.DownArrow);
        InputListener.Instance.registerKeyPressCallback(onLeftwardKeyPressCallback, KeyCode.A);
        InputListener.Instance.registerKeyPressCallback(onLeftwardKeyPressCallback, KeyCode.LeftArrow);
        InputListener.Instance.registerKeyPressCallback(onRightwardKeyPressCallback, KeyCode.D);
        InputListener.Instance.registerKeyPressCallback(onRightwardKeyPressCallback, KeyCode.RightArrow);

        InputListener.Instance.registerKeyPressCallback(onSpaceKeyDownCallback, KeyCode.Space);

    }
    public void destroy()
    {
        /*
        InputListener.Instance.unregisterKeyDownCallback(onForwardKeyDownCallback, KeyCode.UpArrow);
        InputListener.Instance.unregisterKeyDownCallback(onBackwardKeyDownCallback, KeyCode.DownArrow);
        InputListener.Instance.unregisterKeyDownCallback(onLeftwardKeyDownCallback, KeyCode.LeftArrow);
        InputListener.Instance.unregisterKeyDownCallback(onRightwardKeyDownCallback, KeyCode.RightArrow);
        */
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == (int)UnityLayerOrder.LayerOrder_Enemy)
        {
            if (deleteEnemyCallback != null)
                deleteEnemyCallback(collision.gameObject);

            if (decreaseOneHeartCallback != null)
                decreaseOneHeartCallback();
        }
    }


    public void setActive(bool isActive)
    {
        rb.isKinematic = !isActive;
        rb.detectCollisions = isActive;

        this.isActive = isActive;
    }

    private void onMouseDownCallback()
    {
        if (isActive == false)
            return;

        if (addOnBulletCallback != null)
            addOnBulletCallback();
    }

    private const float ROTATE_SPEED_RATE = 0.1f;
    private void onMouseMoveCallback(float x, float y)
    {
        if (isActive == false)
            return;

        //transform.Rotate(new Vector3(0, x * ROTATE_SPEED_RATE, 0));

        // TODO : 如果轉的角度過大, 限定一次只能轉某個限定的角度

        Vector3 position = UtilityFunction.getMouseRaycastPosition();
        if (Vector3.Distance(transform.position, position) > 5)
        {
            position.y = 5;
            transform.LookAt(position);
        }
        else
        {
            Vector3 relativePos = position - transform.position;
            relativePos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = rotation;
        }
    }

    private const float MOVE_SPEED_RATE = 20f;
    private void onForwardKeyPressCallback()
    {
        if (isActive == false)
            return;

        rb.velocity = new Vector3(-1 * MOVE_SPEED_RATE, 0, 0);
    }
    private void onBackwardKeyPressCallback()
    {
        if (isActive == false)
            return;

        rb.velocity = new Vector3(1 * MOVE_SPEED_RATE, 0, 0);
    }
    private void onLeftwardKeyPressCallback()
    {
        if (isActive == false)
            return;

        rb.velocity = new Vector3(0, 0, -1 * MOVE_SPEED_RATE);
    }
    private void onRightwardKeyPressCallback()
    {
        if (isActive == false)
            return;

        rb.velocity = new Vector3(0, 0, 1 * MOVE_SPEED_RATE);
    }

    private void onSpaceKeyDownCallback()
    {
        if (isActive == false)
            return;

        if (useSkillCallback != null)
            useSkillCallback();
    }




}
