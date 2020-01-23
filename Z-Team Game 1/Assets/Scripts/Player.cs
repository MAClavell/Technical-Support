using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Targetable
{
    private float moveSpeed = 30.0f;

    private Vector3 moveDirection = Vector3.zero;

    private float spriteWidth;

    private float leftBound;
    private float topBound;
    private float rightBound;
    private float bottomBound;

    private void Awake()
    {
        IsMoveable = true;
    }

    private void Start()
    {
        spriteWidth = transform.Find("Sprite").GetComponent<SpriteRenderer>().size.x;

        Transform groundTransform = GameObject.Find("GameManager/Ground").transform;
        MeshRenderer groundRenderer = groundTransform.GetComponent<MeshRenderer>();

        float spriteHalfWidth = spriteWidth / 2;

        leftBound = groundTransform.position.x - groundRenderer.bounds.extents.x + spriteHalfWidth;
        rightBound = groundTransform.position.x + groundRenderer.bounds.extents.x - spriteHalfWidth;
        bottomBound = groundTransform.position.z - groundRenderer.bounds.extents.z + spriteHalfWidth;
        topBound = groundTransform.position.z + groundRenderer.bounds.extents.z - spriteHalfWidth;
    }

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

        CheckBounds();
    }

    private void CheckBounds()
    {
        Vector3 newTransform = transform.position;

        if (transform.position.x < leftBound)
        {
            newTransform.x = leftBound;
        }
        else if (transform.position.x > rightBound)
        {
            newTransform.x = rightBound;
        }
        if (transform.position.z < bottomBound)
        {
            newTransform.z = bottomBound;
        }
        else if (transform.position.z > topBound)
        {
            newTransform.z = topBound;
        }

        transform.position = newTransform;
    }
}
