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
        return this;
    }
}
