using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Targetable
{
    public const short ZBUCK_COLLECTION_RADIUS = 50;
    private const float MOVE_SPEED = 30.0f;
    private const ushort TOWER_PRICE = 10;

    public uint ZBucks { get; private set; }

    private GameObject placeObj;
    private Vector3 moveDirection = Vector3.zero;
    private float leftBound;
    private float topBound;
    private float rightBound;
    private float bottomBound;
    bool isPlacing;

    private void Awake()
    {
        IsMoveable = true;
        isPlacing = false;
        placeObj = transform.Find("TowerPlacement").gameObject;
    }

    private void Start()
    {
        float spriteWidth = transform.Find("Sprite").GetComponent<SpriteRenderer>().size.x;

        Transform groundTransform = GameObject.Find("GameManager/Ground").transform;
        MeshRenderer groundRenderer = groundTransform.GetComponent<MeshRenderer>();

        float spriteHalfWidth = spriteWidth / 2;

        leftBound = groundTransform.position.x - groundRenderer.bounds.extents.x + spriteHalfWidth;
        rightBound = groundTransform.position.x + groundRenderer.bounds.extents.x - spriteHalfWidth;
        bottomBound = groundTransform.position.z - groundRenderer.bounds.extents.z + spriteHalfWidth;
        topBound = groundTransform.position.z + groundRenderer.bounds.extents.z - spriteHalfWidth;

        placeObj.SetActive(false);

        ZBucks = TOWER_PRICE;
    }

    // Update is called once per frame
    void Update()
    {
        //Moving
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.z = Input.GetAxisRaw("Vertical");
        moveDirection.Normalize();

        //Placing towers
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Show shadow of tower before placing
            if(!isPlacing)
            {
                isPlacing = true;
                placeObj.SetActive(true);
            }
            //Place the tower
            else
            {
                GameManager.Instance.SpawnTower(placeObj.transform.position, placeObj.transform.rotation);
                isPlacing = false;
                placeObj.SetActive(false);
            }
        }
        //Cancel placement
        else if(isPlacing && Input.GetKeyDown(KeyCode.F))
        {
            isPlacing = false;
            placeObj.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + (moveDirection * MOVE_SPEED * Time.fixedDeltaTime);
        
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

    /// <summary>
    /// Add an amount of zbucks to the player
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void AddZBucks(ushort amount)
    {
        ZBucks += amount;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(ZBUCK_COLLECTION_RADIUS));
    }
#endif
}

