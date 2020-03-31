using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float _damage;
    public float Damage { get => _damage; set => _damage = value < 0 ? 0 : value; }
    public int ownerId;
    
    private Vector3 _myPos;

    private float _arcHeightCoef;
    /// <summary>
    /// ArcHeight represents the height of the arc the projectile will follow, in units
    /// The arcHeight is calculated as = dist(targetPos,ProjectileStartPos) * ArcHeightCoef
    /// If the arcHeightCoef is ever 0 then the projectile will go in a straight line and never go down.
    /// </summary>
    public float ArcHeightCoef { get => _arcHeightCoef; set => _arcHeightCoef = value <= 0 ? 0.0000001f : value; }
    
    private float _speed;
    /// <summary>
    /// Speed of the projectile towards it's target
    /// </summary>
    public float Speed { get => _speed; set => _speed = value <= 0 ? 1 : value; }
    
    private Vector3 _targetPos;
    /// <summary>
    /// Where the projectile should land
    /// </summary>
    public Vector3 TargetPos { get => _targetPos; set => _targetPos = value; }


    private void Start()
    {
        // Make sure it has the proper layer set
        if(gameObject.layer != LayerMask.NameToLayer("Projectile"))
            Debug.LogError("The projectile prefab/gameobject should have the layer set to \"Projectile\"");

        // Caching the position
        _myPos = transform.position;
        
        // Makes the projectile look towards the target ignoring Y axis
        transform.LookAt(new Vector3(TargetPos.x, 0, TargetPos.z));

        // Destroy the game object in given seconds (if it didn't hit anything)
        // TODO: only destroy it in X seconds after it touched the ground
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        MoveInArc();
    }
    
    /// <summary>
    /// Moves the object this script is attached to in a arc with a controllable height (by the ArcHeightCoef) towards the TargetPos with a given Speed
    /// Explanation of the algorithm here: http://luminaryapps.com/blog/arcing-projectiles-in-unity/
    /// </summary>
    void MoveInArc()
    {
        // Getting the position of points disregarding the Y coord
        Vector3 startXzPos = new Vector3(_myPos.x, 0, _myPos.z);
        Vector3 targetXzPos = new Vector3(TargetPos.x, 0, TargetPos.z);
        
        // Calculating the distance between target and start position
        float dist = Vector3.Distance(startXzPos, targetXzPos);
        // The height of the arc itself in UNITS, this value can be changed however one wishes
        float arcHeight = dist * ArcHeightCoef;
        
        // Calculating the next position the projectile needs to take
        Vector3 nextXzPos = Vector3.MoveTowards(new Vector3(transform.position.x, 0, transform.position.z), 
            targetXzPos, Speed * Time.deltaTime);
        
        // Calculating the height that the projectile will have at the nextXzPoz
        float height = Mathf.Lerp(_myPos.y, TargetPos.y, Vector3.Distance(nextXzPos, startXzPos) / dist);
        // Calculating the amount of arc to add in
        // I say add and not subtract because the value of arc is negative (can't explain why)
        float arc = arcHeight * Vector3.Distance(startXzPos,nextXzPos ) * Vector3.Distance(nextXzPos, targetXzPos) /
                    (-0.25f * dist * dist);
        // combine all the elements in a nice 3d point
        Vector3 nextPos = new Vector3(nextXzPos.x, height - arc, nextXzPos.z);
        
        // Move to the next pos
        transform.position = nextPos;
    }
    
    // When the projectile hits something (CHECK LAYER COLLISION MATRIX)
    private void OnTriggerEnter(Collider other)
    {
        var objectTag = other.gameObject.tag;

        if (objectTag == "Unit" || objectTag == "RangedUnit" || objectTag == "Tower")
        {
            // Get the script of the object this projectile is colliding with
            var targetScript = other.gameObject.GetComponent<EntityController>();
            // If it's the owned by the same player exit function
            if (targetScript.OwnerId == ownerId) return;
            
            // Otherwise make the unit take damage
            targetScript.TakeDamage(Damage);
            // And destroy the game object
            Destroy(gameObject); 

            /* Bullet controller shouldn't modify things in other controllers, this should be an independent script
                 if (targetScript.Health <= 0 && Owner.targets.Contains(other.gameObject))
                    Owner.targets.Remove(other.gameObject);*/
        }
    }
}
