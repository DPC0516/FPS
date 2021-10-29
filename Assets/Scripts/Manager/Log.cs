using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    public LogManager logManager;
    public int id;

    private void Start()
    {
        StartCoroutine(after5sec());
    }
    
    private IEnumerator after5sec()
    {
        yield return new WaitForSeconds(5.9f);
        logManager.RemoveLog(id);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
