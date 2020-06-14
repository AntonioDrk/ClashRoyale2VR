using System;
using UnityEngine;
using Zinnia.Rule;


public class SpawnAtTransform:MonoBehaviour
{
    public GameObject targetPosition;
    [SerializeField] private GameObject meleeUnits;
    [SerializeField] private GameObject rangedUnits;
    [SerializeField] private MyScripts.PlayerController _playerController;
    
    
    public void SpawnUnits()
    {
        //TODO: Make sure the ray isn't red 
        if(OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))
            _playerController.SpawnPlayerUnits(targetPosition.transform.position, meleeUnits);
        if(OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch))
            _playerController.SpawnPlayerUnits(targetPosition.transform.position, rangedUnits);
    }
}
