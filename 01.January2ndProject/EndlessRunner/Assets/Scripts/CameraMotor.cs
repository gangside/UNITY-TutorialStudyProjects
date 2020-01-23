using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    Transform lookTarget;
    Vector3 startOffset;
    Vector3 moveVector;

    float transition = 0.0f;
    Vector3 initPos;
    Vector3 animationOffset = new Vector3(0, 7, 3);

    float animationDuration = 2f;

    void Start()
    {
        lookTarget = FindObjectOfType<PlayerMotor>().transform;
        initPos = transform.position;
        startOffset = transform.position - lookTarget.position;
    }


    void Update()
    {
        moveVector = lookTarget.position + startOffset;
        moveVector.x = 0;
        moveVector.y = Mathf.Clamp(moveVector.y, 1, 20);
        
        if(transition > 1.0f) {
            transform.position = moveVector;
        }
        else {
            transform.position = Vector3.Lerp(initPos + animationOffset, moveVector, transition);
            transition += Time.deltaTime * 1/ animationDuration ;
            transform.LookAt(lookTarget.position + Vector3.up);
        }

    }
}
