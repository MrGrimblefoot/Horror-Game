using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthManager : MonoBehaviour
{
    [Header("Health")]
    public bool isDead;
    public float deathDelay;
    public float enemyCurrentHealth;
    public float enemyMaxHealth;
    public float bodyPartHealth = 100;
    [SerializeField] private float highHealthPercentage, halfHealthPercentage, lowHealthPercentage;
    [SerializeField] private Color shieldHealthColor, highHealthColor, halfHealthColor, lowHealthColor, deadHealthColor;
    [SerializeField] private float shieldHealthIntensity, highHealthIntensity, halfHealthIntensity, lowHealthIntensity, deadHealthIntensity;

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
        enemyCurrentHealth = enemyMaxHealth;
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

        foreach (Collider c in colliders)
        {
            if (c.gameObject != this.gameObject && c.gameObject.layer != 2)
            {
                RagdollParts.Add(c);
                c.attachedRigidbody.isKinematic = true;
            }
        }
    }

    public void DamageEnemy(float damage)
    {
        if (!isDead)
        {
            enemyCurrentHealth -= damage;
            anim.SetTrigger("TakeHit");
            //print("I took " + damage + " damage!");
            HandleColorChange();

            if (enemyCurrentHealth <= 0)
            {
                enemyCurrentHealth = 0;
                Die();
            }
            else { anim.SetTrigger("TakeHit"); }
        }
    }

    private void HandleColorChange()
    {
        mat1.EnableKeyword("_EMISSION");
        mat2.EnableKeyword("_EMISSION");

        if(enemyCurrentHealth == enemyMaxHealth)
        {
            //print("Shield");
            mat1.SetColor("_EmissionColor", shieldHealthColor * shieldHealthIntensity);
            mat2.SetColor("_EmissionColor", shieldHealthColor * shieldHealthIntensity);
        }
        else if (enemyCurrentHealth <= (enemyMaxHealth * highHealthPercentage) && enemyCurrentHealth > (enemyMaxHealth * halfHealthPercentage))
        {
            //print("High Health");
            mat1.SetColor("_EmissionColor", highHealthColor * highHealthIntensity);
            mat2.SetColor("_EmissionColor", highHealthColor * highHealthIntensity);
        }
        else if(enemyCurrentHealth <= (enemyMaxHealth * halfHealthPercentage) && enemyCurrentHealth > (enemyMaxHealth * lowHealthPercentage))
        {
            //print("Mid Health");
            mat1.SetColor("_EmissionColor", halfHealthColor * halfHealthIntensity);
            mat2.SetColor("_EmissionColor", halfHealthColor * halfHealthIntensity);
        }
        else if(enemyCurrentHealth <= (enemyMaxHealth * lowHealthPercentage) && enemyCurrentHealth > 0)
        {
            //print("Low Health");
            mat1.SetColor("_EmissionColor", lowHealthColor * lowHealthIntensity);
            mat2.SetColor("_EmissionColor", lowHealthColor * lowHealthIntensity);
        }
        else if(enemyCurrentHealth <= 0)
        {
            //print("Dead");
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
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.velocity = Vector3.zero;
            c.GetComponent<EnemyBodyPartHealthManager>().canDie = false;
        }

        rb.velocity = Vector3.zero;

        isDead = true;
        Destroy(gameObject, deathDelay);
    }

    //private void ReadyToDissolve() { readyToDissolve = true; }
}
