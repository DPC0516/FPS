using System.Collections;
using UnityEngine;

public class EmptyShell : MonoBehaviour
{
    //물리이동을 위한 RigidBody
    [SerializeField]
    private Rigidbody emptyShellRigidbody;

    void Start()
    {
        if (!name.Contains("NR"))
        {
            emptyShellRigidbody.AddForce(transform.rotation * Vector3.right * 2, ForceMode.Impulse);
        }
        StartCoroutine(destroyEmptyShell());
    }

    private IEnumerator destroyEmptyShell()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
