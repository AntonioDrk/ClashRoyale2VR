using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyScripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
    
        private Vector3 _cameraOffset;
    
        private float _smoothFactor = 0.5f;
    
        // Start is called before the first frame update
        void Start()
        {
            _cameraOffset = transform.position - target.position;
            transform.LookAt(target);
        }
    
        // Update is called once per frame
        void Update()
        {
            Vector3 newPosition = target.position + _cameraOffset;
    
            transform.position = Vector3.Slerp(transform.position, newPosition, _smoothFactor);
        }
    }
}

