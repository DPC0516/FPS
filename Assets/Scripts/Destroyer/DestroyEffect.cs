using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject toDestroy;

    private void OnDestroy()
    {
        Destroy(toDestroy);
    }
}
