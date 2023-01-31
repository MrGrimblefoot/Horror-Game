using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : State
{
    public override State Tick(EnemyStateManager stateManager, EnemySensor sensor, EnemyAnimatorManager enemyAnimManager)
    {
        //Select one of many attacks based on attack scores
        //If selected attack is invalid, select a new attack
        //If selected attack is valid, stop movement and perform attack
        //Set recovery timer to the attack's recovery time
        //Return to the combat stance state
        return this;
    }
}
