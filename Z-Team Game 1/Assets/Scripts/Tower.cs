using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Targetable
{
    private const int SEARCH_RADIUS = 25;
    public Targetable Target { get; set; }
    Collider[] overlapSphereCols;
    public float newTargetTimer;
    public bool trackingTarget;
    public bool notTracking;
    private short DAMAGE_AMOUNT = 1;
    [SerializeField]
    private float timeSinceLastShot;
    private const float SHOOT_LIMIT = 1.5f;
    private GameObject shootSprite;

    //Initialize vars
    private void Awake()
    {
        IsMoveable = false;
        newTargetTimer = 0.0f;
        timeSinceLastShot = 0.0f;
        overlapSphereCols = new Collider[RobotManager.MAX_ROBOTS];
    }

    // Start is called before the first frame update
    void Start()
    {
        shootSprite = transform.Find("bigBullet").gameObject;
        shootSprite.SetActive(false);
    } 

    // Update is called once per frame
    void Update()
    {
        //Search for a nearby tower
        if (newTargetTimer > 10)
        {
            Targetable newTarget = FindTarget();
            if (newTarget != null)
            {
                Target = newTarget;
                newTargetTimer = 0.0f;
                trackingTarget = true;
            }
        }


        if (Target == null)
        {
            if ((Target = FindTarget()) == null)
                trackingTarget = false;
        }
        else if (Target.IsMoveable)
        {
            Aim();
            if (timeSinceLastShot > SHOOT_LIMIT)
            {
                Shoot();
                timeSinceLastShot = 0.0f;
            }
        }


        if (timeSinceLastShot > .25f)
        {
            shootSprite.SetActive(false);
        }

        //Update Timer
        newTargetTimer += Time.deltaTime;
        timeSinceLastShot += Time.deltaTime;

    }


    private Targetable FindTarget()
    {
        //Perform overlap sphere
        int result = Physics.OverlapSphereNonAlloc(transform.position, SEARCH_RADIUS, overlapSphereCols, LayerMask.GetMask("Robot"), QueryTriggerInteraction.Ignore);

        //Find closest robot
        Collider closest = null;
        float shortestDist = float.MaxValue;
        float sqrDist = 0;
        for (int i = 0; i < result; i++)
        {
            sqrDist = Vector3.SqrMagnitude(transform.position - overlapSphereCols[i].transform.position);
            if (sqrDist < shortestDist)
            {
                shortestDist = sqrDist;
                closest = overlapSphereCols[i];
            }
        }
        return closest?.GetComponent<Targetable>();
    }


    public void Aim()
    {
        transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));
        transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        
    }

    public void Shoot()
    {
        //Make sure that the target has not been destroyed by another tower
        if (Target != null)
        {
            //Cast the object into a Robot
            Robot currentRobot = (Robot)Target;
            //Give Damage
            currentRobot.TakeDamage(DAMAGE_AMOUNT);
        }
        timeSinceLastShot = 0.0f;

        //Display Shot

        shootSprite.SetActive(true);

    }




}
