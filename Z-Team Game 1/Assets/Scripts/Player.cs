using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Targetable
{
    private readonly Color green = new Color(125f/255f, 1f, 100f/255f, 110f/255f);
    private readonly Color red = new Color(1, 100f/255f, 115f/255f, 110f/255f);
    private const float MOVE_SPEED = 30.0f;
    public const short ZBUCK_COLLECTION_RADIUS = 50;
    private const ushort TOWER_PRICE = 3;
    private const ushort TOWER_UPGRADE_PRICE = 2;

    public uint ZBucks { get; private set; }

    private SpriteRenderer towerGhost;
    private Material towerRadiusMatInst;
    private Vector3 moveDirection = Vector3.zero;
    private float leftBound;
    private float topBound;
    private float rightBound;
    private float bottomBound;
    bool isBuilding;

    private void Awake()
    {
        IsMoveable = true;
        isBuilding = false;
        towerGhost = transform.Find("TowerGhost").GetComponent<SpriteRenderer>();
        towerGhost.transform.GetChild(0).localScale = new Vector3(Tower.SEARCH_RADIUS_SQRT, Tower.SEARCH_RADIUS_SQRT, Tower.SEARCH_RADIUS_SQRT);
        towerRadiusMatInst = towerGhost.GetComponentInChildren<MeshRenderer>().material;
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

        towerGhost.gameObject.SetActive(false);

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
            if(!isBuilding)
                SetBuildMode(true);
            //Place the tower
            else if(ZBucks >= TOWER_PRICE)
            {
                ZBucks -= TOWER_PRICE;
                GameManager.Instance.SpawnTower(towerGhost.transform.position, towerGhost.transform.rotation);
                SetBuildMode(false);
            }
        }
        //Cancel placement
        else if(isBuilding && Input.GetKeyDown(KeyCode.F))
            SetBuildMode(false);
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

    /// <summary>
    /// Turn on or off build mode for the player
    /// </summary>
    /// <param name="buildOn">What mode to set</param>
    private void SetBuildMode(bool buildOn)
    {
        isBuilding = buildOn;
        towerGhost.gameObject.SetActive(buildOn);
        GameManager.Instance.SetBuildMode(buildOn);

        if (buildOn)
            UpdateTowerGhost();
    }

    /// <summary>
    /// Update the tower ghost to represent whether the player can build a tower
    /// </summary>
    private void UpdateTowerGhost()
    {
        if (ZBucks >= TOWER_PRICE)
        {
            towerGhost.color = green;
            towerRadiusMatInst.SetColor("_Color", green);
        }
        else
        {
            towerGhost.color = red;
            towerRadiusMatInst.SetColor("_Color", red);
        }
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
        if (isBuilding)
            UpdateTowerGhost();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(ZBUCK_COLLECTION_RADIUS));
    }
#endif
}

