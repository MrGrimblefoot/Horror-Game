using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : State
{
    public override State Tick(EnemyStateManager stateManager, EnemySensor sensor, EnemyAnimatorManager enemyAnimManager)
    {
        //Chase target
        //If within attack range, go to the combat stance state 
        //If the target gets away, go to the last known position
        //If there is no target, go back to sleep position, or just go back to the sleep state
        //if there is a target, start chasing again
        ChaseCurrentTarget(stateManager);
        RotateTowardsCurrentTarget(stateManager);
        return this;
    }

    private void ChaseCurrentTarget(EnemyStateManager stateManager)
    {
        stateManager.anim.SetFloat("Speed", 1);
    }

    private void RotateTowardsCurrentTarget(EnemyStateManager stateManager)
    {
        stateManager.navmeshAgent.enabled = true;
        stateManager.navmeshAgent.SetDestination(stateManager.target.transform.position);
        stateManager.transform.rotation = Quaternion.Slerp(stateManager.transform.rotation, stateManager.navmeshAgent.transform.rotation,
            stateManager.rotationSpeed / Time.deltaTime);
    }
}
