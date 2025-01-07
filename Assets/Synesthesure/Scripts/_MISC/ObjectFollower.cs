using UnityEngine;
namespace Synesthesure
{
    public class ObjectFollower : MonoBehaviour
    {
        public GameObject objectToFollow;
        public bool follow = true;
        public bool gotoOriginalLocationWhenStopped = false;
        Vector3 originalLocation;

        void Start()
        {
            originalLocation = gameObject.transform.position;
        }

        void Update()
        {
            if (follow) gameObject.transform.position = objectToFollow.transform.position;
            else gameObject.transform.position = originalLocation;
        }
    }
}
