using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float Damage;
    public int OwnerId;

    public UnitController Owner;

    private void Start()
    {
        if(gameObject.layer != LayerMask.NameToLayer("Projectile"))
            Debug.LogError("The projectile prefab/gameobject should have the layer set to \"Projectile\"");
        
        // Destroy the game object in 2 seconds (if it didn't hit anything)
        Destroy(gameObject, 2f);
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
            if (targetScript.OwnerId == OwnerId) return;
            
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
