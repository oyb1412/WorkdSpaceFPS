using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public Transform target;
    public float enemyHP;
    NavMeshAgent nav;
    bool isLive;
    public bool isPatrol;
    bool isHit;
    bool isTrace;
    Animator anim;
    Collider col;
    EnemyFOV fov;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();    
        nav = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();
        fov = GetComponent<EnemyFOV>();
        nav.speed = moveSpeed * 0.5f;
        isLive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLive)
            return;

        
        var dir = target.position - transform.position;
        if ((fov.isTracePlayer()) || isHit)
        {
            isPatrol = false;
            isHit = true;
            anim.SetBool("Walk", true);
            nav.SetDestination(target.position);
        }
        if (dir.magnitude > 20f && !isPatrol && !isHit)
        {
            StartCoroutine(PatrolCorutine());
        }
        if(!isPatrol && dir.magnitude < 2f) 
        {
            anim.SetBool("Walk", false);
            anim.SetTrigger("Attack");
        }
    }
    IEnumerator PatrolCorutine()
    {
        isPatrol = true;
        float count = 0;
        float ran = Random.Range(-1f, 1f);
        while (isPatrol)
        {
            count+= Time.deltaTime;
            if (count < 2)
            {
                transform.position += new Vector3(ran, 0f, ran) * Time.deltaTime * moveSpeed;
                transform.rotation = Quaternion.LookRotation(new Vector3(ran, 0f, ran));
            }
            if (count >=2 && count < 3)
                anim.SetBool("Walk", false);

            if (count >= 3)
            {
                anim.SetBool("Walk", true);

                count = 0;
                ran = Random.Range(-1f, 1f);
            }

            if (isHit)
                break;
            yield return null;

        }
    }

    public void SetHP(float hp)
    {
        enemyHP += hp;
        isHit = true;
        isPatrol = false;
        if (enemyHP <= 0)
        {
            anim.SetTrigger("Die");
            col.enabled = false;
            isLive = false;
            Destroy(gameObject, 1f);
        }
        else
        {
            anim.SetTrigger("Hit");
        }
    }

  
}
