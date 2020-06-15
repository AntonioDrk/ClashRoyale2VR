using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
//using System.Diagnostics;

namespace MyScripts
{
    public class UnitController : EntityController
    {
        [SerializeField]
        private float _speed;
        public float Speed { get => _speed; set => _speed = value < 0 ? 0 : value; }

        [SerializeField]
        private float _triggerZoneRadius;
        public float TriggerZoneRadius { get => _triggerZoneRadius; set => _triggerZoneRadius = value < 0 ? 0 : value; }

        protected NavMeshAgent agent;

        private float distance = 999;
        
        // cooldown of the attacks
        protected float _attackCooldown = 1f;
        private float _cooldown = 0;

        // Represents the closest enemy tower to move towards
        protected GameObject mainTarget;
        
        // Represents other target that is in the way, i.e enemy units
        protected GameObject target;
        protected Animator anim;
        protected CapsuleCollider cc;
        protected MeshRenderer meshRenderer;
        
        protected List<GameObject> targets = new List<GameObject>();

        void Update()
        {
            // TARGET SELECTION
            // If the closest tower isn't set, then set it
            if (mainTarget == null)
            {
                SetTarget();
                // If we searched for towers and there still aren't any it means we won (or game ended)
                if (mainTarget == null)
                    return;
            }
            
            // If the unit has set target a tower, check to see if we don't have units nearby to target
            if (target == mainTarget)
            {
                if (targets.Count > 0)
                {
                    // If we have other targets held into the list remove them from the list and set them as target
                    EntityController unitController;
                    do
                    {
                        // Cycle through the list, remove any dead targets if there are,
                        // if not, set them as current target
                        unitController = getTargetScript(targets[0]);
                        target = targets[0];
                        targets.RemoveAt(0);
                    } while (unitController != null && unitController.Health <= 0);
                }
            }
            else if (target == null)
            {
                // If there are no more units, set current target as the closest tower to the unit
                target = mainTarget;
            }
            else
            {
                // If there's a unit targeted right now, check to see if the first entry of the list isn't closer than the current target
                // Prioritize closer targets always!
                if (targets.Count > 0 && distance > DistanceToTarget(targets[0]))
                {
                    GameObject tmp = target;
                    target = targets[0];
                    targets.RemoveAt(0);
                    AddTarget(tmp);
                }
            }

            
            // MOVEMENT
            // If we haven't reached into our range the target
            if (distance > Range && target != null)
            {
                // Check to see if a new target appeared
                
                // Set the destination to the target
                agent.destination = target.transform.position;
            }
            else
            {
                // else if we are in range of our target
                // Hold your ground
                agent.destination = transform.position;

                // If we have already a selected target, put the attack in cooldown
                _cooldown += Time.deltaTime;
                if (_cooldown > _attackCooldown)
                {
                    _cooldown = 0;
                    Attack();
                }
            }

            // At the end, recalculate the distance between this unit and the selected target
            if (target != null)
            {
                Vector3 targetPos = target.transform.position;
                Vector3 myPos = transform.position;
                // We make the unit look towards it's target
                transform.LookAt(new Vector3(targetPos.x, myPos.y, targetPos.z));

                distance = DistanceToTarget(target);
            }
        }
        
        /// <summary>
        /// Returns the distance to a specified target, taking into account the size of the objects
        /// </summary>
        /// <param name="targetObject">The object to calculate the distance to</param>
        /// <returns></returns>
        protected float DistanceToTarget(GameObject targetObject)
        {
            // If the object we're calculating the distance to is null, return infinity distance
            if (targetObject == null)
                return 999;
            
            Vector3 targetPos = targetObject.transform.position;
            Vector3 myPos = transform.position;
            
            float myWidthHalved = meshRenderer.bounds.size.x / 2;
            float targetWidthHalved = targetObject.GetComponent<MeshRenderer>().bounds.size.x / 2;
            // Remaining distance till target, we subtract the radius of each unit so they don't go into eachother
            return Vector3.Distance(myPos, targetPos) - (myWidthHalved + targetWidthHalved);
        }

        /// <summary>
        /// Virtual function so each unit type implements it's own
        /// </summary>
        protected virtual void Attack() { }

        /*private void OnTriggerEnter(Collider other)
        {
            var objectTag = other.gameObject.tag;

            if (objectTag == "Unit" || objectTag == "RangedUnit" || objectTag == "Tower")
            {
                var targetScript = getTargetScript(other.gameObject);

                if (other != target && targetScript.OwnerId != OwnerId)
                {
                    if (target != null && target.gameObject.CompareTag("Tower"))
                        target = other.gameObject;
                    else
                        targets.Add(other.gameObject);
                }
            }
        }*/

        // When a unit exits the attack range, it gets removed from the targets to attack
        /*private void OnTriggerExit(Collider other)
        {
            if (targets.Contains(other.gameObject))
                targets.Remove(other.gameObject);
        }*/

        /// <summary>
        /// Helper function for automatic test on Start for all derivatives of this base class,
        /// i.e if the unit of the prefab has the appropriate layer selected,
        /// if everything is set properly in the inspector etc.
        /// Feel free to overload this method (while CALLING the BASE METHOD) for class specific tests
        /// </summary>
        protected void TestsChecker()
        {
            // Test to make sure layers are set correctly
            if(gameObject.layer != LayerMask.NameToLayer("UnitBody"))
                Debug.LogError("The unit prefab/gameobject should have the layer set to \"UnitBody\"");
        }
        
        /// <summary>
        /// Function to set the mainTarget to the closest enemy tower
        /// </summary>
        protected void SetTarget()
        {
            distance = 999;
            GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag("Tower");

            foreach (var pt in possibleTargets)
            {
                var targetScript = pt.GetComponent<TowerController>();

                if (targetScript.OwnerId != OwnerId && targetScript.Health > 0)
                {
                    var currentDistance = Vector3.Distance(transform.position, pt.transform.position);

                    if (currentDistance < distance)
                    {
                        mainTarget = pt;
                        distance = currentDistance;
                    }
                }
            }
        }
        
        /// <summary>
        /// Returns the appropriate component depending on the type of structure/unit
        /// </summary>
        /// <param name="entity">The gameobject to get the component from</param>
        /// <returns></returns>
        protected EntityController getTargetScript(GameObject entity)
        {
            if (entity == null) return null;
            var targetType = entity.gameObject.tag;
            switch (targetType)
            {
                case "Unit":
                    return entity.GetComponent<MeleeUnitController>();
                case "RangedUnit":
                    return entity.GetComponent<RangedUnitController>();
                case "Tower":
                    return entity.GetComponent<TowerController>();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Adds the given gameobject to the targets internal list, the unit will kill each target one by one
        /// </summary>
        /// <param name="unitToAdd">The gameobject that needs to be targeted by the unit</param>
        public void AddTarget(GameObject unitToAdd)
        {
            if (unitToAdd.CompareTag("Unit") || unitToAdd.CompareTag("RangedUnit"))
            {
                EntityController entityController = getTargetScript(unitToAdd);
                if (entityController.OwnerId != OwnerId &&  !targets.Contains(unitToAdd) && target != unitToAdd)
                {
                    targets.Add(unitToAdd);
                    // After adding a target, sort the whole list after the distance to our unit
                    // (so it always focuses the nearest unit)
                    
                    targets = targets.OrderBy(
                        DistanceToTarget
                    ).ToList();
                }
                    
            }
        }
        
        /// <summary>
        /// Removes a given gameobject from the target list and from targeting, helpful when a unit exited the range so units go back to a new target
        /// </summary>
        /// <param name="unitToRemove">The game object to remove</param>
        public void RemoveTarget(GameObject unitToRemove)
        {
            if (targets.Contains(unitToRemove))
            {
                targets.Remove(unitToRemove);
                // Resort the list once we removed something
                targets = targets.OrderBy(
                    DistanceToTarget
                ).ToList();
            }
            if (target == unitToRemove)
                target = null;
        }
    }
}

