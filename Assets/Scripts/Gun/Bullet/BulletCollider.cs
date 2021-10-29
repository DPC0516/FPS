using UnityEngine;
using Varriables;
using Photon.Pun;

public class BulletCollider : MonoBehaviour
{
    public Gun gunScript;
    [SerializeField]
    private Rigidbody bulletRigidbody;

    public void fire()
    {
        Public.DebugLog("fire", "fired", null);
        bulletRigidbody.AddForce(transform.forward.normalized * gunScript.bulletSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Public.DebugLog("bullet onHit", collision.collider.name, null);
        onHit(collision);
    }

    private void Update()
    {
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }

    //히트판정 처리
    private void onHit(Collision collision)
    {
        if (!collision.collider.CompareTag("EmptyShell") && !collision.collider.CompareTag("Bullet"))
        {
            string material;
            try
            {
                material = collision.collider.GetComponent<HitMaterial>().material;
            }
            catch
            {
                material = null;
            }
            if (collision.collider.name.Contains("Player"))
            {
                try
                {
                    if (collision.collider.name.Contains("Head"))
                    {
                        collision.collider.gameObject.GetComponent<HitEvent>().hit(gunScript.damage * gunScript.headShotMultipler, Public.id);
                    }
                    else
                    {
                        collision.collider.gameObject.GetComponent<HitEvent>().hit(gunScript.damage, Public.id);
                    }
                }
                catch
                {

                }
            }
            if (material != null)
            {
                //총알 자국 표시
                PhotonNetwork.Instantiate(Path.bulletHole + material + "_BulletHole",
                    collision.contacts[collision.contacts.Length / 2].point,
                    Quaternion.FromToRotation(Vector3.up, collision.contacts[collision.contacts.Length / 2].normal));
            }
            Destroy(gameObject);
        }
    }
}
