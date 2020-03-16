using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIController : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        healthText.text = transform.parent.GetComponent<EntityController>().Health.ToString();
        transform.rotation = mainCamera.transform.rotation;
    }
}
