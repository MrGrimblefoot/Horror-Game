using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySleepState : State
{
    public EnemyChaseState chaseState;

    public override State Tick(EnemyStateManager stateManager, EnemySensor sensor, EnemyHealthManager healthManager)
    {
        if (!healthManager.isDead)
        {
            if (sensor.canSeePlayer) { stateManager.target = sensor.playerRef; }

            if (stateManager.target != null) { return chaseState; }

            else { return this; }
        }
        return this;
    }
}
