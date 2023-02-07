using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthManager : MonoBehaviour
{
    [Header("Health")]
    public bool isDead;
    public float deathDelay;
    public float enemyHealth;
    public float bodyPartHealth = 100;
    [SerializeField] private Color fullHealthColor, halfHealthColor, lowHealthColor, deadHealthColor;
    [SerializeField] private float fullHealthIntensity, halfHealthIntensity, lowHealthIntensity, deadHealthIntensity;

    public List<Collider> RagdollParts = new List<Collider>();

    Material mat1;
    Material mat2;
    //Variables
    NavMeshAgent navMeshAgent;
    private Animator anim;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        mat1 = GetComponentInChildren<SkinnedMeshRenderer>().materials[0];
        mat2 = GetComponentInChildren<SkinnedMeshRenderer>().materials[1];
        anim.enabled = true;
        isDead = false;
        SetRagdollParts();
    }

    private void Start()
    {
        HandleColorChange();
    }

    //private void Update()
    //{

    //}

    void SetRagdollParts()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();

        foreach (Collider c in colliders) { if (c.gameObject != this.gameObject && c.gameObject.layer != 2) { RagdollParts.Add(c); } }
    }

    public void DamageEnemy(float damage)
    {
        if (!isDead)
        {
            enemyHealth -= damage;
            anim.SetTrigger("TakeHit");
            print("I took " + damage + " damage!");
            HandleColorChange();

            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                Die();
            }
            else { anim.SetTrigger("TakeHit"); }
        }
    }

    private void HandleColorChange()
    {
        mat1.EnableKeyword("_EMISSION");
        mat2.EnableKeyword("_EMISSION");

        if (enemyHealth == 100)
        {
            print("Full Health");
            mat1.SetColor("_EmissionColor", fullHealthColor * fullHealthIntensity);
            mat2.SetColor("_EmissionColor", fullHealthColor * fullHealthIntensity);
        }
        else if(enemyHealth < 100 && enemyHealth > 25)
        {
            print("Mid Health");
            mat1.SetColor("_EmissionColor", halfHealthColor * halfHealthIntensity);
            mat2.SetColor("_EmissionColor", halfHealthColor * halfHealthIntensity);
        }
        else if(enemyHealth < 26 && enemyHealth > 0)
        {
            print("Low Health");
            mat1.SetColor("_EmissionColor", lowHealthColor * lowHealthIntensity);
            mat2.SetColor("_EmissionColor", lowHealthColor * lowHealthIntensity);
        }
        else if(enemyHealth <= 0)
        {
            print("Dead");
            mat1.SetColor("_EmissionColor", deadHealthColor * deadHealthIntensity);
            mat2.SetColor("_EmissionColor", deadHealthColor * deadHealthIntensity);
        }

        DynamicGI.UpdateEnvironment();
    }

    public void Die()
    {
        rb.useGravity = false;
        //this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        anim.enabled = false;
        navMeshAgent.enabled = false;
        foreach (Collider c in RagdollParts)
        {
            c.isTrigger = false;
            c.attachedRigidbody.velocity = Vector3.zero;
            c.GetComponent<EnemyBodyPartHealthManager>().canDie = false;
        }

        rb.velocity = Vector3.zero;

        isDead = true;
        Destroy(gameObject, deathDelay);
    }

    //private void ReadyToDissolve() { readyToDissolve = true; }
}
