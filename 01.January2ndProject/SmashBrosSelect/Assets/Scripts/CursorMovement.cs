using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    public float speed;

    RectTransform rectTransform;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        CursorMove();
    }

    private void CursorMove() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.position += new Vector3(x, y, 0) * speed * Time.deltaTime;

        //게임 화면의 가로 새로만큼의 스크린 좌표계를 월드 좌표계로 변환
        //그러면 월드좌표계의 캔버스사이즈, 이번같은 경우느 카메라 디스플레이를 바탕으로 스크린좌표를 덮고 있으므로.. ㅇㅋ..
        Vector3 canvasSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        Vector3 canvas = new Vector3(Screen.width, Screen.height, 0);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -canvasSize.x, canvasSize.x),
            Mathf.Clamp(transform.position.y, -canvasSize.y, canvasSize.y),
            transform.position.z);
    }
}
