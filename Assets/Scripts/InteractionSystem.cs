using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    //Detection Point
    public Transform detectionPoint;
    //Detection Radius
    private const float detectionRadius = 0.2f;
    //Detection Layer
    public LayerMask detectionLayer;
    //Cached trigger object
    public GameObject detectedOject;

    // Update is called once per frame
    void Update()
    {
        if(DetectObject())
        {
            detectedOject.GetComponent<Item>().Interact();
        }
    }

    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectObject()
    {
        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
        if(obj == null)
        {
            detectedOject = null;
            return false;
        }
        else
        {
            detectedOject = obj.gameObject;
            return true;
        }
    }
}
