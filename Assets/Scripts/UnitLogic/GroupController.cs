using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyScripts
{
    public class GroupController : MonoBehaviour
    {
        [SerializeField] private float _cost;

        public float Cost
        {
            get => _cost;
            set => _cost = value < 0 ? 0 : value;
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}