using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : State
{
    bool hasAssignedAnim;
    public override State Tick(EnemyStateManager stateManager, EnemySensor sensor, EnemyHealthManager healthManager)
    {
        //Chase target
        //If within attack range, go to the combat stance state 
        //If the target gets away, go to the last known position
        //If there is no target, go back to sleep position, or just go back to the sleep state
        //if there is a target, start chasing again
        if (!healthManager.isDead)
        {
            if(hasAssignedAnim == false) { stateManager.anim.SetFloat("Speed", 1); hasAssignedAnim = true; }
            NavigateTowardsCurrentTarget(stateManager);
        }
        return this;
    }

    private void NavigateTowardsCurrentTarget(EnemyStateManager stateManager)
    {
        //Move towards the player
        stateManager.navmeshAgent.SetDestination(stateManager.target.transform.position);

        //Rotat towards the player
        var turnTowardNavSteeringTarget = stateManager.navmeshAgent.steeringTarget;

        Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        stateManager.transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * stateManager.rotationSpeed);

        //Thank you InsaneDuane!!! https://forum.unity.com/threads/how-do-i-update-the-rotation-of-a-navmeshagent.707579/
    }
}
