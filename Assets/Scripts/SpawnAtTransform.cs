using System;
using UnityEngine;
using Zinnia.Rule;


public class SpawnAtTransform:MonoBehaviour
{
    public GameObject targetPosition;
    [SerializeField] private GameObject _units;
    [SerializeField] private MyScripts.PlayerController _playerController;
    
    
    public void SpawnUnits()
    {
        // Way of checking if the targeted place is valid
        if(!targetPosition.activeInHierarchy) return;
        
        _playerController.SpawnPlayerUnits(targetPosition.transform.position, _units);
    }
}
