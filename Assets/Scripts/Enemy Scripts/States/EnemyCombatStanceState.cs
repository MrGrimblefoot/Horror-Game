using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatStanceState : State
{
    public override State Tick(EnemyStateManager stateManager, EnemySensor sensor, EnemyHealthManager healthManager)
    {
        //Check to see if in attack range
        //If ready to attack, go to attack state, otherwise wait
        //If the player runs away, go back to chase state
        return this;
    }
}
