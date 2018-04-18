using System;
//using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    private Rigidbody rb;
    private Action<GameObject> deleteEnemyCallback;
    private Action<GameObject> deleteBulletCallback;

    public void init(Action<GameObject> deleteEnemyCallback, Action<GameObject> deleteBulletCallback)
    {
        this.deleteEnemyCallback = deleteEnemyCallback;
        this.deleteBulletCallback = deleteBulletCallback;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)UnityLayerOrder.LayerOrder_Enemy)
        {
            if (deleteEnemyCallback != null)
                deleteEnemyCallback(other.gameObject);

            if (deleteBulletCallback != null)
                deleteBulletCallback(gameObject);
        }
        else if (other.gameObject.layer == (int)UnityLayerOrder.LayerOrder_Wall)
        {
            if (deleteBulletCallback != null)
                deleteBulletCallback(gameObject);
        }
    }

    private const float BULLET_SPEED_RATE = 20f;
    public void shootBullet(Vector3 position, Vector3 shootingDirection)
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;

        transform.position = position;
        rb.velocity = shootingDirection * BULLET_SPEED_RATE;
    }
    public void invisibleBullet()
    {
        rb.isKinematic = true;
        rb.detectCollisions = false;

        transform.position = new Vector3(1000, 1000, 1000);
    }

}
