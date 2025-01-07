using UnityEngine;

namespace Synesthesure
{
    public class PositionGenerator_BounderiesBounce : MonoBehaviour
    {
        public Transform zoneLocation;
        Vector3 zpos;
        public float width = 100f;
        public float height = 100f;
        public float depth = 100f;
        public float speed = 1f;
        public Vector3 speedFactor = new Vector3(1, 1, 1);
        public Transform position;
        public Transform position_smoothed;
        public float smoothFactor = 2f;
        Vector3 pos;

        void Start()
        {
            if (zoneLocation != null) zpos = zoneLocation.position;
            if (position != null) pos = position.position;
            if (position_smoothed != null) pos = position_smoothed.position;
        }

        void Update()
        {
            pos += speedFactor * Time.deltaTime * speed;

            if (pos.x > zpos.x + width / 2f)
            {
                float x = zpos.x + width / 2f;
                pos = new Vector3(x, pos.y, pos.z);
                speedFactor = new Vector3(speedFactor.x * -1, speedFactor.y, speedFactor.z);
            }
            else if (pos.x < zpos.x - width / 2f)
            {
                float x = zpos.x - width / 2f;
                pos = new Vector3(x, pos.y, pos.z);
                speedFactor = new Vector3(speedFactor.x * -1, speedFactor.y, speedFactor.z);
            }

            if (pos.y > zpos.y + height / 2f)
            {
                float y = zpos.y + height / 2f;
                pos = new Vector3(pos.x, y, pos.z);
                speedFactor = new Vector3(speedFactor.x, speedFactor.y * -1, speedFactor.z);
            }
            else if (pos.y < zpos.y - height / 2f)
            {
                float y = zpos.y - height / 2f;
                pos = new Vector3(pos.x, y, pos.z);
                speedFactor = new Vector3(speedFactor.x, speedFactor.y * -1, speedFactor.z);
            }

            if (pos.z > zpos.z + depth / 2f)
            {
                float z = zpos.z + depth / 2f;
                pos = new Vector3(pos.x, pos.y, z);
                speedFactor = new Vector3(speedFactor.x, speedFactor.y, speedFactor.z * -1);
            }
            else if (pos.z < zpos.z - depth / 2f)
            {
                float z = zpos.z - depth / 2f;
                pos = new Vector3(pos.x, pos.y, z);
                speedFactor = new Vector3(speedFactor.x, speedFactor.y, speedFactor.z * -1);
            }

            if (position != null) position.position = pos;

            if (position_smoothed != null) position_smoothed.position = Vector3.Lerp(position_smoothed.position, pos, Time.deltaTime * smoothFactor);

        }
    }
}