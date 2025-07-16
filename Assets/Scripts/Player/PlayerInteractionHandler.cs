using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    private RaycastHit hit;
    [SerializeField] private float interactionDistance = 0.3f;

    private void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hit,interactionDistance)) 
        {
            GameObject interactedObject = hit.collider.gameObject;
            EventManager.Instance.onInteraction.Invoke(interactedObject);
        }
    }
}
