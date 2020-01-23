using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    Animator animator;
    CharacterController characterController;
    Vector3 moveVector;

    float speed = 8f;
    float verticalVelocity = 0.5f;
    float gravity = 12f;

    float animationDuration = 2f;
    float startTime;

    bool isDead = false;

    public event Action OnDeath; 

    private void Start() {
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();

        startTime = Time.time;
    }

    private void Update() {

        if (isDead) return;

        //카메라 애니메이션 시간과 맞춰주고 플레이어는 입력 불가
        if (Time.time - startTime < animationDuration) {
            characterController.Move(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        moveVector = Vector3.zero;

        if (characterController.isGrounded) {
            verticalVelocity = -0.5f;
        }
        else {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveVector.x = Input.GetAxisRaw("Horizontal") * speed;
        moveVector.y = verticalVelocity;
        moveVector.z = speed;


        characterController.Move(moveVector * Time.deltaTime);
    }

    public void SetSpeed(float speed) {
        this.speed += speed;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.point.z > transform.position.z + characterController.radius) {
            Die();
        }
    }

    private void Die() {
        isDead = true;
        animator.SetTrigger("Die");
        OnDeath();
    }
}
