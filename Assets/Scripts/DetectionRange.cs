using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    [SerializeField] private UnitController unitController;

    void Start()
    {
        if (unitController == null)
        {
            Debug.LogError("The unit controller hasn't been set in the inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        unitController.AddTarget(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        unitController.RemoveTarget(other.gameObject);
    }
}
