using UnityEngine;

namespace Synesthesure
{
    public class LookAt : MonoBehaviour
    {
        public Transform target;

        void Update()
        {
            transform.LookAt(target);

            //transform.rotation = Quaternion.Slerp(position_smoothed.rotation, targetRotation, Time.deltaTime * smoothFactor);

        }
    }
}