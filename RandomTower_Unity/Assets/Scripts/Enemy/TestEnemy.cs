using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private Transform routeGroup;
    private Transform[] routes;
    private NavMeshAgent agent;

    private Vector3 destination;
    private int targetIndex = 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        Transform[] tree = routeGroup.GetComponentsInChildren<Transform>();
        routes = tree.Where(route => route != routeGroup).ToArray();
    }


    private void Start()
    {
        transform.position = routes[targetIndex].position;
        NextDestination();
    }

    private void Update()
    {
        if (IsArrived())
        {
            NextDestination();
        }
    }

    private bool IsArrived()
    {
        return transform.position.x == destination.x &&
            transform.position.z == destination.z; 
    }

    private void NextDestination()
    {
        targetIndex++;
        if(targetIndex >= routes.Length)
        {
            targetIndex = 0;
        }

        destination = routes[targetIndex].position;
        agent.SetDestination(destination);
    }
}
