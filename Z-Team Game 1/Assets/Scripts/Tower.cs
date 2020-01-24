using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Targetable
{
    private const int SEARCH_RADIUS = 20;
    public Targetable Target { get; set; }
    Collider[] overlapSphereCols;
    public float newTargetTimer;
    public bool trackingTarget;
    public bool notTracking;

    //Initialize vars
    private void Awake()
    {
        IsMoveable = false;
        newTargetTimer = 0.0f;
        overlapSphereCols = new Collider[RobotManager.MAX_ROBOTS];
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        else if (Target.IsMoveable) //update aiming
            Aim();

        //Update Timer
        newTargetTimer += Time.deltaTime;

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
        ////Find the X and Z of the enemy being tracked
        Vector2 enemyPosition = new Vector2(Target.transform.position.x, Target.transform.position.z);
        float towerYRot = transform.rotation.y;




    }



}
