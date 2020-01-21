using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    /// <summary>
    /// The current target this zombie is chasing
    /// </summary>
    public Transform Target { get; set; }
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pnt = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
        agent.destination = pnt;
        pnt.z = -1;
    }

    private void OnDestroy()
    {
        RobotManager.DecrementRobotCount();
    }
}
