//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private Rigidbody rb;
    private Transform playerTransform;

	
	public void init(Transform playerTransform)
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        this.playerTransform = playerTransform;
    }	

	public void update()
    {
        rb.velocity = playerTransform.position - transform.position;
	}

    public void switchEnemyVisible(bool isVisible)
    {
        rb.isKinematic = !isVisible;
        rb.detectCollisions = isVisible;

        if (isVisible == true)
            transform.position = new Vector3(0, 5, 0);
        else
            transform.position = new Vector3(1000, 1000, 1000);
    }


}
