﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public InputManager inputManager;
    public Transform camT;

    public float speed = 300;
    public float runSpeed = 600;
    private float actualSpeed;

    public bool isRunning = false;
    private bool isMoving = false;

    private Rigidbody rb;
    private PlayerAnimation anim;

    public LayerMask layerMask;

    public bool onEnemy;
    private Transform target;

    public AudioSource run;

    public AudioClip[] clips;

    private void Awake()
    {
        anim = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!inputManager.isMovementLocked)
        {
            VerifyEnemy();
            RunningMovement();
            RotateToForward();
            BasicMovement();
            AudioPress();
        }
    }

    void BasicMovement()
    {
        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        Vector3 moveDir = (transform.forward * ver + transform.right * hor);

        if (moveDir.magnitude > 1)
            moveDir.Normalize();

        moveDir *= actualSpeed;

        //Vector3 MoveDirection = new Vector3(moveDir.x * actualSpeed, 0, moveDir.z * actualSpeed);

        
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);
        //rb.AddForce(new Vector3(moveDir.x * actualSpeed, 0, moveDir.z * actualSpeed) * Time.deltaTime);
        //rb.MovePosition(transform.position + MoveDirection * Time.deltaTime);

        isMoving = (ver != 0 || hor != 0) ? true : false;

        Animate(hor, ver);
    }

    void AudioPress()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            run.clip = clips[1];
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            run.clip = clips[0];
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if(!run.isPlaying)
                run.Play();
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            run.Stop();
        }
        

    }

    void RotateToForward()
    {
        Vector3 direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        direction.y = transform.forward.y;
        transform.forward = (Vector3.Slerp(transform.forward, direction, 0.8f));
    }

    void RunningMovement()
    {
        if (Input.GetKey(inputManager.sprintInput))
        {
            actualSpeed = runSpeed;
            isRunning = true;
        }
        else
        {
            actualSpeed = speed;
            isRunning = false;
        }
    }

    void Animate(float hor, float ver)
    {
        if (!isMoving)
        {
            anim.SetMovementSpeed(0, 0);
        }
        else if (isRunning)
        {
            anim.SetMovementSpeed(hor * 2, ver * 2);
        }
        else
        {
            anim.SetMovementSpeed(hor, ver);
        }
    }

    void VerifyEnemy()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit, float.MaxValue, layerMask)){
            if (hit.collider)
            {
                onEnemy = true;
                target = hit.collider.gameObject.transform;
            }
            else
            {
                onEnemy = false;
                target = null;
            }
        }
        else
        {
            onEnemy = false;
            target = null;
        }
    }

    public void FreezeMovement()
    {
        anim.SetMovementSpeed(0, 0);
        rb.velocity = Vector3.zero;
    }
}