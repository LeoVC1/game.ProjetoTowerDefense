﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageArea : PlayerSkill
{
    [Header("Attack Objects:")]
    public GameObject attackParticle;
    public GameObject attackPreview;
    public LayerMask layerMask;
    public GameEventListener listener;

    private GameObject attackPreviewInstance;
    private Vector3 attackPosition;

    [Range(1, 30)]
    public float attackRange;
    [Range(1, 30)]
    public float attackRadius;

    public float moveSkillSpeed = 5;

    [Header("Animation Properties:")]
    public string animationParameter;

    private void Update()
    {
        WaitingConfirmation();
    }

    public override void CastSkill()
    {
        if (this.enabled)
        {
            inputManager.isCastingSpell = true;
            inputManager.isWaitingConfirmEvent = false;
            mainResource.Value -= resourceCost;
            StartCoroutine(Cooldown());
            onCooldownStart.Raise();
            DestroyPreview();
            Animate();
            inputManager.FreezeCamera();
            inputManager.LockMovement();
            GameObject attack = Instantiate(attackParticle, attackPosition, Quaternion.identity);
        }
    }

    public override void WaitingConfirmation()
    {
        if (waitingConfirmation)
        {
            listener.enabled = true;
            if (attackPreviewInstance == null)
            {
                attackPreviewInstance = Instantiate(attackPreview);
                attackPreviewInstance.transform.position = (transform.position - (new Vector3(0, 1f, 0))) + transform.forward * attackRange;
            }
            else
            {
                Vector3 mousePosition = GetMousePosition();
                attackPreviewInstance.transform.position = mousePosition;
            }
            attackPosition = attackPreviewInstance.transform.position + Vector3.up * 0.5f;
        }
        else
        {
            listener.enabled = false;
            if (attackPreviewInstance != null)
            {
                Destroy(attackPreviewInstance);
                attackPreviewInstance = null;
            }
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider)
            {
                mousePosition = hit.point;
            }
        }
        return mousePosition;
    }

    private void Animate()
    {
        anim.SetTrigger(animationParameter);
    }

    public void OnAnimationEnd()
    {
        inputManager.UnfreezeCamera();
        inputManager.UnlockMovement();
        inputManager.isCastingSpell = false;
        inputManager.canAttack = true;
        EnableOtherSkills(true);
    }

    public override void DestroyPreview()
    {
        Destroy(attackPreviewInstance);
        attackPreviewInstance = null;
        waitingConfirmation = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (waitingConfirmation)
        {
            float theta = 0;
            float x = attackRadius * Mathf.Cos(theta);
            float y = attackRadius * Mathf.Sin(theta);
            Vector3 pos = attackPosition + new Vector3(x, 0, y);
            Vector3 newPos = pos;
            Vector3 lastPos = pos;
            for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
            {
                x = attackRadius * Mathf.Cos(theta);
                y = attackRadius * Mathf.Sin(theta);
                newPos = attackPosition + new Vector3(x, 0, y);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.DrawLine(pos, lastPos);
        }
    }

}
