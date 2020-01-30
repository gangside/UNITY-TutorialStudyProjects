using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    public float speed;

    void Update()
    {
        CursorMove();
    }

    private void CursorMove() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.position += new Vector3(x, y, 0) * speed * Time.deltaTime;

        Vector3 canvasSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        
        //좌표제한
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -canvasSize.x, canvasSize.x),
            Mathf.Clamp(transform.position.y, -canvasSize.y, canvasSize.y),
            transform.position.z);

        Debug.Log(canvasSize);
        Debug.Log(transform.position);
    }
}
