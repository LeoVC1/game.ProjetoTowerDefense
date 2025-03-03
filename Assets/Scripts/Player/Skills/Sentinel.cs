﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentinel : PlayerSkill
{
    [Header("Sentinel Properties:")]
    public GridManager gridManager;
    public GameObject sentinelPrefab;
    public GameObject sentinelPreview;
    public GameEventListener listener;
    public LayerMask layerMask;

    private GameObject sentinelPreviewInstance;
    private Transform sentinelRangeInstance;
    private Vector3 sentinelLocation;

    [Range(1, 30)]
    public float sentinelRange;

    private void Update()
    {
        WaitingConfirmation();
    }

    public override void CastSkill()
    {
        (Vector3 newPosition, Vector3 ID) = gridManager.GetClosestPoint(/*transform.position + transform.forward * gridManager.cellSize*/GetMousePosition());
        if (!gridManager.UsingID(ID))
        {
            DestroyPreview();
            GameObject sentinel = Instantiate(sentinelPrefab);
            sentinel.transform.position = newPosition;
            sentinel.GetComponentInChildren<SentinelInstance>().SetDistance(sentinelRange);
            gridManager.SetID(ID);
        }
    }

    public override void WaitingConfirmation()
    {
        if (waitingConfirmation)
        {
            listener.enabled = true;
            if (sentinelPreviewInstance == null)
            {
                sentinelPreviewInstance = Instantiate(sentinelPreview);
                sentinelPreviewInstance.transform.position = GetMousePosition();
                sentinelPreviewInstance.transform.localScale = new Vector3(gridManager.cellSize, gridManager.cellSize, gridManager.cellSize);
                sentinelRangeInstance = GetRangeObject();
            }
            else
            {
                (Vector3 newPosition, Vector3 ID) = gridManager.GetClosestPoint(GetMousePosition());
                if (!gridManager.UsingID(ID))
                    sentinelPreviewInstance.transform.position = newPosition;

                if (sentinelRangeInstance)
                    sentinelRangeInstance.localScale = new Vector3(sentinelRange, sentinelRangeInstance.localScale.y, sentinelRange);
            }
        }
        else
        {
            listener.enabled = false;
            if (sentinelPreviewInstance != null)
            {
                Destroy(sentinelPreviewInstance);
                sentinelPreviewInstance = null;
            }
        }
    }

    public Transform GetRangeObject()
    {
        foreach(Transform t in sentinelPreviewInstance.GetComponentsInChildren<Transform>())
        {
            if (t.name == "Range")
                return t;
        }
        return null;
    }

    public void DestroyPreview()
    {
        Destroy(sentinelPreviewInstance);
        sentinelPreviewInstance = null;
        sentinelRangeInstance = null;
        waitingConfirmation = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (waitingConfirmation)
        {
            (Vector3 newPosition, Vector3 ID) = gridManager.GetClosestPoint(transform.position + transform.forward * gridManager.cellSize);

            if (!gridManager.UsingID(ID))
            {
                float theta = 0;
                float x = sentinelRange * Mathf.Cos(theta);
                float y = sentinelRange * Mathf.Sin(theta);
                Vector3 pos = newPosition + new Vector3(x, 0, y);
                Vector3 newPos = pos;
                Vector3 lastPos = pos;
                for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
                {
                    x = sentinelRange * Mathf.Cos(theta);
                    y = sentinelRange * Mathf.Sin(theta);
                    newPos = newPosition + new Vector3(x, 0, y);
                    Gizmos.DrawLine(pos, newPos);
                    pos = newPos;
                }
                Gizmos.DrawLine(pos, lastPos);
            }

        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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

}
