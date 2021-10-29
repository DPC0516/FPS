using UnityEngine;

public class ArmController : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    private Transform armUp;
    [SerializeField]
    private Transform armDown;

    public void setTarget(Transform _target)
    {
        target = _target;
    }
 
    void Update()
    {
        transform.LookAt(target);
        transform.localScale = new Vector3(1f, 1f, Vector3.Distance(transform.position, target.position));
    }
}
