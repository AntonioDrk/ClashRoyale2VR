﻿using UnityEngine;
using UnityEngine.AI;

namespace MyScripts
{
    public class MeleeUnitController : UnitController
    {
        void Start()
        {
            TestsChecker();
            
            Health = 100;
            Damage = 20;
            Range = 0.5f;
            Speed = 10;
            TriggerZoneRadius = 5;
    
            anim = gameObject.GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>(); 
            meshRenderer = GetComponent<MeshRenderer>();
            cc = transform.GetChild(0).GetComponent<CapsuleCollider>();
    
            cc.radius = TriggerZoneRadius; 
    
            SetTarget();
        }
    
        /// <summary>
        /// Attacks the current target
        /// </summary>
        protected override void Attack()
        {
            var targetScript = getTargetScript(target);
            targetScript.TakeDamage(Damage);
    
            if(targetScript.Health <= 0 && targets.Contains(target.gameObject)) 
                targets.Remove(target.gameObject); 
        }
    
    }

}

