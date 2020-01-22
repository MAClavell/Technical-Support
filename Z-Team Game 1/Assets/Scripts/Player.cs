using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float moveSpeed = 30.0f;

    private Vector3 moveDirection = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.z = Input.GetAxisRaw("Vertical");

        moveDirection.Normalize();
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + (moveDirection * moveSpeed * Time.fixedDeltaTime);
        
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Euler(0, Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + 180.0f, 0);
        }
    }
}
