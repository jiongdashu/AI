using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CJS.AI
{
    public class Vision : MonoBehaviour
    {
        public LayerMask enemyLayerMask;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != enemyLayerMask)
                return;
            GameObject target = other.gameObject;
            Vector3 direction = target.transform.position - transform.position;
            float distance = direction.magnitude;

        }
    }
}

