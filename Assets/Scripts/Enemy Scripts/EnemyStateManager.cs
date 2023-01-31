using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateManager : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navmeshAgent;
    [SerializeField] private EnemyAnimatorManager animManager;
    [SerializeField] private EnemySensor sensor;
    public GameObject target;

    public State currentState;

    void Start()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        animManager = GetComponent<EnemyAnimatorManager>();
        sensor = GetComponentInChildren<EnemySensor>();
    }

    void FixedUpdate()
    {
        HandleStateMachine();
    }

    private void HandleStateMachine()
    {
        if(currentState != null)
        {
            State nextState = currentState.Tick(this, sensor, animManager);
            
            if(nextState != null) { SwitchToNextState(nextState); }
        }
    }

    private void SwitchToNextState(State state) { currentState = state; }
}
