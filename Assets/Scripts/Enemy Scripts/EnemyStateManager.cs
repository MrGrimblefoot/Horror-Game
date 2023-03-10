using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateManager : MonoBehaviour
{
    [SerializeField] private EnemySensor sensor;
    public GameObject target;

    [Header("Animation")]
    [SerializeField] private EnemyHealthManager healthManager;
    public Animator anim;

    [Header("Movement")]
    public NavMeshAgent navmeshAgent;
    public float rotationSpeed = 5;

    public State currentState;

    void Start()
    {
        navmeshAgent = GetComponentInChildren<NavMeshAgent>();
        navmeshAgent.enabled = true;
        healthManager = GetComponent<EnemyHealthManager>();
        sensor = GetComponentInChildren<EnemySensor>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        navmeshAgent.transform.localPosition = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        HandleStateMachine();
    }

    private void HandleStateMachine()
    {
        if(currentState != null)
        {
            State nextState = currentState.Tick(this, sensor, healthManager);
            
            if(nextState != null) { SwitchToNextState(nextState); }
        }
    }

    private void SwitchToNextState(State state) { currentState = state; }
}
