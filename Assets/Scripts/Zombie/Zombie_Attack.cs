using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Zombie_Attack : NetworkBehaviour
{
    private float attackRate = 2;
    private float nextAttack;
    private int damage = 20;
    private float minDistance = 2;
    private float currentDistance;
    private Transform myTransform;
    private Zombie_Target targetScript;
    private Vector3 mouthPosition;

    [SerializeField]
    private Material zombieGreen;
    [SerializeField]
    private Material zombieRed;

    private AudioSync audioSync;


    void Start()
    {
        myTransform = transform;
        targetScript = GetComponent<Zombie_Target>();
        audioSync = GetComponent<AudioSync>();
        if(isServer)
        {
            StartCoroutine(Attack());
            StartCoroutine(Bellow());
        }
    }

    void CheckIfTargetRange()
    {
        if(targetScript.targetTransform != null)
        {
            mouthPosition = myTransform.position + new Vector3(0, 1, 0);
            currentDistance = Vector3.Distance(targetScript.targetTransform.position, mouthPosition);
            if(currentDistance < minDistance && Time.time > nextAttack)
            {
                audioSync.PlaySound(-1);
                nextAttack = Time.time + attackRate;
                targetScript.targetTransform.GetComponent<Player_Health>().DeductHealth(damage);
                StartCoroutine(ChangeZombieMat()); //为主机玩家
                RpcChangeZombieApperarance();
            }
        }
    }

    IEnumerator ChangeZombieMat()
    {
        GetComponent<Renderer>().material = zombieRed;
        yield return new WaitForSeconds(attackRate - 0.5f);
        GetComponent<Renderer>().material = zombieGreen;
    }

    [ClientRpc]
    void RpcChangeZombieApperarance()
    {
        StartCoroutine(ChangeZombieMat());
    }

    IEnumerator Attack()
    {
        for(;;)
        {
            yield return new WaitForSeconds(0.2f);
            CheckIfTargetRange();
        }
    }

    IEnumerator Bellow()
    {
        for(;;)
        {
            if(Random.Range(0, 5) == 0)
            {
                audioSync.PlaySound(-1);
            }
            yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
        }
    }
}
