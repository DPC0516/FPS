using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Varriables;



public class SpectatorController : MonoBehaviour
{
    private float mouseSensitivityX = 100;
    private float mouseSensitivityY = 100;
    //현재 카메라 각도
    [SerializeField]
    private float playerRotationX = 0f;
    [SerializeField]
    private float playerRotationY = 0f;
    //Player의 최대 위/아래 각도
    private const int maxPlayerRotationUp = -90;
    private const int maxPlayerRotationDown = 90;

    // Update is called once per frame
    void Update()
    {
        if (!Public.isPause)
        {
            float isX = Input.GetAxisRaw("Mouse X");
            float isY = Input.GetAxisRaw("Mouse Y");

            //감도에 의해 이동 각도 계산
            float toRotateX = isX * mouseSensitivityX * Time.deltaTime;
            float toRotateY = isY * mouseSensitivityY * Time.deltaTime;

            //X각도 변환
            playerRotationX += toRotateX;

            //Player의 위/아래 최고 각도에 의해 Y 이동 각도 재계산
            playerRotationY -= toRotateY;
            playerRotationY = Mathf.Clamp(playerRotationY, maxPlayerRotationUp, maxPlayerRotationDown);

            transform.localRotation = Quaternion.Euler(playerRotationY, playerRotationX, 0f);

            float isV = Input.GetAxisRaw("Vertical");

            //현재 이동 속도
            float moveSpeed;
            float multipler = 1f;
            if (Input.GetKey(Key.Run))
            {
                multipler = 5f;
            }
            else
            {
                multipler = 1f;
            }

            //키보드가 움직였을시
            if (!(isV == 0))
            {
                Vector3 dir = Quaternion.Euler(0f, transform.localRotation.y, 0f) * new Vector3(0, 0, isV);

                moveSpeed = 10f * multipler;

                transform.Translate(dir * moveSpeed * Time.deltaTime);
            }
        }
    }
}
