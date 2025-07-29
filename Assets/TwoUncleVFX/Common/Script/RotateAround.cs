using UnityEngine;

namespace VFXTools
{
    public class RotateAround : MonoBehaviour
    {
        public Transform target;            
        public float rotationSpeed = 30.0f; 
        public float rotationRadius = 2.0f; 
        private Vector3 initialPosition;

        void Start()
        {
            initialPosition = transform.position;
        }

        void Update()
        {
            if (target != null)
            {
                float angle = Time.time * rotationSpeed;
                float x = initialPosition.x + rotationRadius * Mathf.Cos(angle);
                float z = initialPosition.z + rotationRadius * Mathf.Sin(angle);

                transform.position = new Vector3(x, transform.position.y, z);

                transform.LookAt(target, Vector3.up);
            }
        }
    }
}
