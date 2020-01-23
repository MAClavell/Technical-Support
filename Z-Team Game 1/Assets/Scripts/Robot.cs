using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : Targetable
{
    /// <summary>
    /// The current target this zombie is chasing
    /// </summary>
    public Targetable Target { get; set; }

    //Consts
    private const int MAX_HEALTH = 3;
    private const int SEARCH_RADIUS = 20;
    private const float SEARCH_TIMER_MAX = 0.2f;

    private NavMeshAgent agent;
    private float searchTimer;
    private short health;
    Collider[] overlapSphereCols;

    //Initialize vars
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        IsMoveable = true;
        overlapSphereCols = new Collider[GameManager.MAX_TOWERS];
    }

    // Start is called before the first frame update
    void Start()
    {
        health = MAX_HEALTH;
        Target = GameManager.Instance.Player;
    }

    // Update is called once per frame
    void Update()
    {
        //Search for a nearby tower
        searchTimer += Time.deltaTime;
        if(searchTimer > SEARCH_TIMER_MAX)
        {
            searchTimer -= SEARCH_TIMER_MAX;
            Targetable newTarget = FindTarget();
            if (newTarget != null)
            {
                Target = newTarget;
                agent.destination = Target.transform.position;
            }
        }

        //Find a new target because this one died
        if(Target == null)
        {
            //If still  null, assign to player
            if ((Target = FindTarget()) == null)
                Target = GameManager.Instance.Player;
            agent.destination = Target.transform.position;
        }
        //The target is moveable, so continuously update the position
        else if(Target.IsMoveable)
            agent.destination = Target.transform.position;

    }

    /// <summary>
    /// Use Physics.OverlapSphere to find any towers in range
    /// </summary>
    /// <returns>The closest tower in range</returns>
    private Targetable FindTarget()
    {
        //Perform overlap sphere
        int result = Physics.OverlapSphereNonAlloc(transform.position, SEARCH_RADIUS, overlapSphereCols, LayerMask.GetMask("Tower"), QueryTriggerInteraction.Ignore);

        //Find closest robot
        Collider closest = null;
        float shortestDist = float.MaxValue;
        float sqrDist = 0;
        for(int i = 0; i < result; i++)
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

    /// <summary>
    /// Have the robot take damage, possibly killing it
    /// </summary>
    /// <param name="damageAmount">The amount of damage to apply</param>
    public void TakeDamage(short damageAmount)
    {
        health -= damageAmount;
        if (health < 0)
        {
            RobotManager.DecrementRobotCount();
            Destroy(gameObject); //TODO: decide if object pooling would be better than destroy/instantiate
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SEARCH_RADIUS);
    }
#endif
}
