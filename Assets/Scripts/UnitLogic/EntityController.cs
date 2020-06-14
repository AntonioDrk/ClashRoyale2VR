using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyScripts
{
    public class EntityController : MonoBehaviour
    {
        [SerializeField] private float _health;

        public float Health
        {
            get => _health;
            set => _health = value < 0 ? 0 : value;
        }

        [SerializeField] private float _damage;

        public float Damage
        {
            get => _damage;
            set => _damage = value < 0 ? 0 : value;
        }

        [SerializeField] private float _range;

        public float Range
        {
            get => _range;
            set => _range = value < 0 ? 0 : value;
        }

        [SerializeField] private int _ownerId;

        public int OwnerId
        {
            get => _ownerId;
            set => _ownerId = value < 0 ? 0 : value;
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}