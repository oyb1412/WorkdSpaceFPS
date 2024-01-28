using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletDamage;
    public float bulletSpeed;
    Rigidbody rigid;
    public ParticleSystem sparkParticle;
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();  
        rigid.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("WALL"))
        {
            ShowSparkEffect(collision);
            Destroy(gameObject);
        }
        else if(collision.collider.CompareTag("ENEMY"))
        {
            collision.collider.GetComponent<EnemyController>().SetHP(-10);
            Destroy(gameObject);

        }
    }

    void ShowSparkEffect(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);

        var spark = Instantiate(sparkParticle, collision.transform);
        spark.transform.position = contact.point;
        spark.transform.rotation = rot;
        spark.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Destroy(spark, 0.5f);
    }
}
