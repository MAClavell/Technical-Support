﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Targetable
{
    public enum PlayerState
    {
        Alive,
        Dying,
        Dead
    }

    private const float MOVE_SPEED = 30.0f;
    private const float ROTATION_SPEED = 10.0f;
    private const int MAX_HEALTH = 10;
    public const short ZBUCK_COLLECTION_RADIUS = 50;
    private const ushort TOWER_PRICE = 1;

    private readonly Color green = new Color(125f/255f, 1f, 100f/255f, 110f/255f);
    private readonly Color red = new Color(1, 100f/255f, 115f/255f, 110f/255f);
    private const ushort TOWER_UPGRADE_PRICE = 2;

    public uint ZBucks { get; private set; }
    public float TowerSize { get; set; }
    public PlayerState currentState = PlayerState.Dead;
    public PlayerHealthBar healthBar;
    public TextMeshProUGUI zBucksCounter;

    private GameObject placeObj;
    private BoxCollider boxCollider;
    private SpriteRenderer towerGhost;
    private Material towerRadiusMatInst;
    private Collider[] towerResults;

    private Vector3 moveDirection = Vector3.zero;

    private float leftBound;
    private float topBound;
    private float rightBound;
    private float bottomBound;

    private int health = 0;
    bool isBuilding;
    private bool canPlace;

    private void Awake()
    {
        IsMoveable = true;
        isBuilding = false;
        boxCollider = GetComponent<BoxCollider>();
        towerGhost = transform.Find("TowerGhost").GetComponent<SpriteRenderer>();
        towerGhost.transform.GetChild(0).localScale = new Vector3(Tower.SEARCH_RADIUS_SQRT, Tower.SEARCH_RADIUS_SQRT, Tower.SEARCH_RADIUS_SQRT);
        towerRadiusMatInst = towerGhost.GetComponentInChildren<MeshRenderer>().material;
        towerResults = new Collider[10];

        float spriteHalfWidth = transform.Find("Sprite").GetComponent<SpriteRenderer>().size.x / 2;

        leftBound = -GameManager.Instance.BoundsX + spriteHalfWidth;
        rightBound = GameManager.Instance.BoundsX - spriteHalfWidth;
        bottomBound = -GameManager.Instance.BoundsY + spriteHalfWidth;
        topBound = GameManager.Instance.BoundsY - spriteHalfWidth;
    }

    /// <summary>
    /// Initialize the player at the start of each round.
    /// </summary>
    public void Init()
    {
        gameObject.SetActive(true);

        currentState = PlayerState.Alive;

        SetHealth(MAX_HEALTH);

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + 180.0f, 0);

        towerGhost.gameObject.SetActive(false);

        canPlace = true;
        ZBucks = TOWER_PRICE;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Alive:
                //Moving
                moveDirection.x = Input.GetAxisRaw("Horizontal");
                moveDirection.y = 0.0f;
                moveDirection.z = Input.GetAxisRaw("Vertical");
                moveDirection.Normalize();

                //Update tower ghost
                if(isBuilding)
                    UpdateTowerGhost();

                //Placing towers
		        if(Input.GetKeyDown(KeyCode.Space))
		        {
		            //Show shadow of tower before placing
		            if(!isBuilding)
		                SetBuildMode(true);
		            //Place the tower
		            else if (canPlace)
		            {
                        RemoveZBucks(TOWER_PRICE);
		                GameManager.Instance.SpawnTower(towerGhost.transform.position, towerGhost.transform.rotation);
		                SetBuildMode(false);
		            }
		        }
		        //Cancel placement
		        else if(isBuilding && Input.GetKeyDown(KeyCode.F))
		            SetBuildMode(false);
                break;

            case PlayerState.Dying:
                gameObject.SetActive(false);
                currentState = PlayerState.Dead;
                GameManager.Instance.SetGamestate(GameState.Ended);
                break;

            case PlayerState.Dead:
                break;

            default:
                Debug.LogError("Player state unknown");
                break;
        }
    }

    /// <summary>
    /// Have the player take damange
    /// </summary>
    /// <param name="damageAmount">The amount of damage to apply</param>
    public void TakeDamage(ushort damageAmount)
    {
        int newHealth = health - damageAmount;

        SetHealth(newHealth);
    }

    /// <summary>
    /// Update the players health value and update the health display
    /// </summary>
    /// <param name="value">The new health value</param>
    public void SetHealth(int value)
    {
        health = value;

        healthBar.UpdateDisplay(health, MAX_HEALTH);

        if (health < 1)
        {
            health = 0;
            currentState = PlayerState.Dying;
        }
    }

    /// <summary>
    /// Move the character
    /// </summary>
    private void FixedUpdate()
    {
        if (currentState == PlayerState.Alive)
        {
            Move();
        }
        // Commented all this out because all of this logic is in update where it is also hooked up to input
        ////Show shadow of tower before placing
        //if(!isBuilding) {
        //    SetBuildMode(true);
        //}
        ////Place the tower
        //else if(ZBucks >= TOWER_PRICE)
        //{
        //    ZBucks -= TOWER_PRICE;
        //    GameManager.Instance.SpawnTower(towerGhost.transform.position, towerGhost.transform.rotation);
        //    SetBuildMode(false);
        //}
        ////Cancel placement
        //else if(isBuilding && Input.GetKeyDown(KeyCode.F))
        //    SetBuildMode(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RobotHitbox")
        {
            Debug.Log("Player hit");
            TakeDamage(GameManager.ROBOT_ATTACK_DAMAGE);
        }
    }

    /// <summary>
    /// Move the player, check for collisions and update sprite
    /// </summary>
    private void Move()
    {
        if (moveDirection != Vector3.zero)
        {
            transform.position = transform.position + moveDirection * MOVE_SPEED * Time.fixedDeltaTime;

            LerpSpriteRotation();
            // RotateSprite();

            CheckBounds();
        }
    }

    /// <summary>
    /// Immediately update the sprite's rotation based on the move direction
    /// </summary>
    /// <returns></returns>
    private void RotateSprite()
    {
        Quaternion moveAngle = Quaternion.Euler(0, Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + 180.0f, 0);
        transform.rotation = moveAngle;
    }

    /// <summary>
    /// Update the sprite's rotation based on the looking direction and move direction
    /// </summary>
    /// <returns></returns>
    private void LerpSpriteRotation()
    {
        Quaternion moveAngle = Quaternion.Euler(0, Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + 180.0f, 0);

        float angleOffset = moveAngle.eulerAngles.y - transform.rotation.eulerAngles.y;

        if (angleOffset == 0)
        {
            return;
        }

        if (angleOffset > 180.0f) angleOffset -= 360;
        else if (angleOffset < -180.0f) angleOffset += 360;

        if (angleOffset > ROTATION_SPEED) angleOffset = ROTATION_SPEED;
        else if (angleOffset < -ROTATION_SPEED) angleOffset = -ROTATION_SPEED;

        transform.Rotate(Vector3.up, angleOffset);
    }

    /// Turn on or off build mode for the player
    /// </summary>
    /// <param name="buildOn">What mode to set</param>
    private void SetBuildMode(bool buildOn)
    {
        isBuilding = buildOn;
        towerGhost.gameObject.SetActive(buildOn);
        GameManager.Instance.SetBuildMode(buildOn);

        if (isBuilding)
            UpdateTowerGhost();
    }

    /// <summary>
    /// Update the tower ghost to represent whether the player can build a tower
    /// </summary>
    private void UpdateTowerGhost()
    {
        //Find a tower
        var numTowers = Physics.OverlapSphereNonAlloc(towerGhost.transform.position, 
            TowerSize, towerResults, 
            LayerMask.GetMask("Tower"));

        //Update ghost
        if (numTowers > 0 || ZBucks < TOWER_PRICE)
        {
            canPlace = false;
            towerGhost.color = red;
            towerRadiusMatInst.SetColor("_Color", red);
        }
        else
        {
            canPlace = true;
            towerGhost.color = green;
            towerRadiusMatInst.SetColor("_Color", green);
        }
    }

    /// <summary>
    /// Check the player's relationship to the bounds of the background and lock the player within it
    /// </summary>
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
    /// Remove an amount of zbucks from the player
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void RemoveZBucks(ushort amount)
    {
        ZBucks -= amount;

        UpdateZBucksDisplay();
    }

    /// <summary>
    /// Add an amount of zbucks to the player
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void AddZBucks(ushort amount)
    {
        ZBucks += amount;

        UpdateZBucksDisplay();

        if (isBuilding)
            UpdateTowerGhost();
    }

    /// <summary>
    /// Update the zBucks display
    /// </summary>
    public void UpdateZBucksDisplay()
    {
        zBucksCounter.text = ZBucks.ToString();
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(ZBUCK_COLLECTION_RADIUS));
    }
#endif
}
