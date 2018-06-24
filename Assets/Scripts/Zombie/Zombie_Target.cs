using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Zombie_Target : NetworkBehaviour
{
    [SerializeField]
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform myTransform;
    public Transform targetTransform;
    private LayerMask raycastLayer;
    private float radius = 30;

    private Vector3 ranomPos;
    private UnityEngine.AI.NavMeshHit navMeshHit;
    public float walkSpeed = 2;
    public float runSpeed = 8;


    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        myTransform = transform;
        raycastLayer = 1 << LayerMask.NameToLayer("Player");
        if(isServer)
        {
            StartCoroutine(DoCheck());
            StartCoroutine(RandomWalk());
            agent.enabled = true;
            agent.speed = walkSpeed;
        }
    }

    void SearchForTarget()
    {
        if(!isServer)
        {
            return;
        }

        if(targetTransform == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius, raycastLayer);

            if(hitColliders.Length > 0)
            {
                int randomint = Random.Range(0, hitColliders.Length);
                targetTransform = hitColliders[randomint].transform;
            }
        }

        if(targetTransform != null && targetTransform.GetComponent<BoxCollider>().enabled == false)
        {
            targetTransform = null;
            StartCoroutine(RandomWalk());
        }
    }

    IEnumerator DoCheck()
    {
        for(;;)
        {
            SearchForTarget();
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator MoveToTarget()
    {
        for(;;)
        {
            if(targetTransform != null)
            {
                agent.speed = runSpeed;
                agent.SetDestination(targetTransform.position);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator RandomWalk()
    {
        agent.speed = walkSpeed;
        for(;;)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3.2f));
            if(targetTransform != null)
            {
                StopCoroutine(RandomWalk());
                StartCoroutine(MoveToTarget());
                break;
            }
            Vector3 randomPoint = myTransform.position + new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
            if(UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out navMeshHit, 50f, UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.SetDestination(navMeshHit.position);
            }
        }
    }
}
