using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Varriables;


public class BulletTrajectory : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private PhotonView PV;

    public Gun gunScript;

    private Vector3 lastPos;
    private Vector3 currentPos;

    public Vector3 fireAngleForward;
    public Quaternion fireAngle;
    public Vector3 firePosition;
    public Vector3 firePositionGun;

    private float currentTime = 0f;

    [PunRPC]
    private void setLineRenderer(int index, float[] pos)
    {
        lineRenderer.SetPosition(index, new Vector3(pos[0], pos[1], pos[2]));
    }

    void Start()
    {
        lineRenderer.endWidth = 0.02f;
        lineRenderer.startWidth = 0.02f;
        currentPos = firePositionGun;
        PV.RPC("setLineRenderer", RpcTarget.All, 0, new float[] { currentPos.x, currentPos.y, currentPos.z});
        PV.RPC("setLineRenderer", RpcTarget.All, 1, new float[] { currentPos.x, currentPos.y, currentPos.z });
    }

    void Update()
    {
        if (PV.IsMine)
        {
            RaycastHit hit;
            currentTime += Time.deltaTime;
            UpdatePos();
            PV.RPC("setLineRenderer", RpcTarget.All, 0, new float[] { currentPos.x, currentPos.y, currentPos.z });
            PV.RPC("setLineRenderer", RpcTarget.All, 1, new float[] { lastPos.x, lastPos.y, lastPos.z });

            if (Physics.Linecast(lastPos, currentPos, out hit))
            {
                onHit(hit);
                PhotonNetwork.Destroy(gameObject);
            }
            if (currentPos.y < 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void UpdatePos()
    {
        float angle = fireAngle.eulerAngles.x;
        if(angle > 180)
        {
            angle = (360 - angle);
        }
        else
        {
            angle *= -1;
        }
        angle = angle * Mathf.PI / 180;

        Public.DebugLog("UpdatePos" , "angle:" + angle.ToString(), PV);

        float x = gunScript.bulletSpeed * Mathf.Cos(angle) * currentTime;
        Public.DebugLog("UpdatePos", "x:" + x.ToString(), PV);

        float y = gunScript.bulletSpeed * Mathf.Sin(angle) * currentTime - (-Physics.gravity.y * currentTime * currentTime)/2;
        Public.DebugLog("UpdatePos", "y:" + y.ToString(), PV);

        float world_pos_y = y + firePosition.y;
        Vector3 world_pos_z = fireAngleForward * x;
        Vector3 world_pos = new Vector3(firePosition.x + world_pos_z.x, world_pos_y, firePosition.z + world_pos_z.z);

        lastPos = currentPos;
        currentPos = world_pos;

        Public.DebugLog("UpdatePos", "time:" + currentTime.ToString(), PV);
    }

    //히트판정 처리
    private void onHit(RaycastHit hit)
    {
        if (!hit.collider.name.Contains("EmptyShell") && !hit.collider.name.Contains("Bullet"))
        {
            string material = null;
            try
            {
                material = hit.collider.GetComponent<HitMaterial>().material;
            }
            catch
            {
            }
            if (hit.collider.name.Contains("Player"))
            {
                try
                {
                    if (hit.collider.name.Contains("Head"))
                    {
                        hit.collider.gameObject.GetComponent<HitEvent>().hit(gunScript.damage * gunScript.headShotMultipler, Public.id);
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<HitEvent>().hit(gunScript.damage, Public.id);
                    }
                }
                catch
                {

                }
            }
            if(material != null)
            {
                //총알 자국 표시
                 PhotonNetwork.Instantiate(Path.bulletHole + material + "_BulletHole",
                    hit.point,
                    Quaternion.FromToRotation(Vector3.up, hit.normal));
                 Destroy(gameObject);
            }
        }
    }
}
