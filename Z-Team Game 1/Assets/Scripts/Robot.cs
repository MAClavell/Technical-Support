using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    NavMeshAgent agent;

    Transform playerTransform;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        playerTransform = GameObject.Find("Player").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pnt = Vector3.zero;

        if (playerTransform)
        {
            pnt = playerTransform.position;
        }
        
        //Vector3 pnt = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
        agent.destination = pnt;
        pnt.z = -1;
    }
}
