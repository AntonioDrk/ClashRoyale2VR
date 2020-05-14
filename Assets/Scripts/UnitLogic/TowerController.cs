using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerController: EntityController
{
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    public GameObject bulletPrefab;

    [SerializeField]
    private float _triggerZoneRadius;
    public float TriggerZoneRadius { get => _triggerZoneRadius; set => _triggerZoneRadius = value < 0 ? 0 : value; }

    // cooldown of the attacks
    protected float _attackCooldown = 1f;
    private float _cooldown = 0;
    private float distance = 999;

    protected CapsuleCollider cc;
    protected MeshRenderer meshRenderer;

    protected GameObject target;
    protected List<GameObject> targets = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Health = 1000;
        Damage = 50;
        TriggerZoneRadius = 2f;
        Range = 2f;

        meshRenderer = GetComponent<MeshRenderer>();
        cc = GetComponent<CapsuleCollider>();
        cc.radius = TriggerZoneRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null && targets.Count > 0)
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
        else if (target != null)
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

        if (distance < Range && target != null)
        {
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

    private void OnTriggerEnter(Collider other)
    {
        AddTarget(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveTarget(other.gameObject);
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
            if (entityController.OwnerId != OwnerId && !targets.Contains(unitToAdd) && target != unitToAdd)
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

    /// <summary>
    /// Attacks the current target
    /// </summary>
    public void Attack()
    {
        if (target)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.transform.parent = null;
            var bulletScript = bullet.GetComponent<BulletController>();
            bulletScript.Speed = 2f;
            bulletScript.ArcHeightCoef = .75f;
            bulletScript.Damage = Damage;
            bulletScript.ownerId = OwnerId;
            bulletScript.TargetPos = target.transform.position;
        }
    }
}
