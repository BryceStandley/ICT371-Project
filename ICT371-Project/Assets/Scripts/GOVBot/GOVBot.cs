using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GOVBot : MonoBehaviour
{
    public bool hasMeetGovBot = false;

    public Transform[] walkLocations;
    private int currentIndex = 0;
    private Transform currentTargetLocation;
    public Transform startLocation;
    public Transform playerTarget;
    public NavMeshAgent agent;
    public Animator anim;
    private float distanceToWalkLocation,distanceToPlayer;
    public float distanceToAmount = 0.5f;
    public float distanceToPlayerAmount = 4f;

    private void Start()
    {
        currentTargetLocation = walkLocations[0];
        agent.SetDestination(startLocation.position);
    }
    private void Update()
    {
        
        if(hasMeetGovBot)
        {
            agent.SetDestination(currentTargetLocation.position);
            
            distanceToWalkLocation = Vector3.Distance(currentTargetLocation.position, transform.position);
            distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            if(distanceToWalkLocation <= distanceToAmount)
            {
                anim.SetBool("Walk_Anim", false);
                agent.SetDestination(NextLocation());
            }

            if(distanceToPlayer <= distanceToPlayerAmount)
            {
                agent.SetDestination(transform.position);
                anim.SetBool("Walk_Anim", false);
                transform.LookAt(playerTarget);//rotates to look at the player in 1 frame, would work better over time
            }
            else
            {
                anim.SetBool("Walk_Anim", true);
            }
        }
        
    }

    private Vector3 NextLocation()
    {
        if(currentIndex == walkLocations.Length - 1)
        {
            currentIndex--;
        }
        else
        {
            currentIndex++;
        }
        currentTargetLocation = walkLocations[currentIndex];
        anim.SetBool("Walk_Anim", true);
        return currentTargetLocation.position;
    }

}
