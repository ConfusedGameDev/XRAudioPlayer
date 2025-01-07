using UnityEngine;
using System.Collections.Generic;
namespace Synesthesure
{
    public class ObjectsArrangement : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Transform location;
        enum DistributionType
        {
            Linear,
            Random,
            FibonacciDisc,
            FibonacciSphere
        }
        [SerializeField] DistributionType distribution;
        [SerializeField] int numberOfObjects = 8;
        int previousNumberOfObjects;
        public float radius = 8;
        float previousRadius = 8;
        List<GameObject> objects = new List<GameObject>();

        private void Start()
        {
            if (location == null) location = this.transform;
            previousRadius = radius;
            previousNumberOfObjects = numberOfObjects;
            CreateObjects(Distribution[(int)distribution]);
        }

        private void Update()
        {
            if (numberOfObjects != previousNumberOfObjects)
            {
                previousNumberOfObjects = numberOfObjects;
                CreateObjects(Distribution[(int)distribution]);
            }
            if (radius != previousRadius)
            {
                for (int i = 0; i < numberOfObjects; i++)
                {
                    objects[i].transform.localPosition = Distribution[(int)distribution](i);
                }
                previousRadius = radius;
            }
        }


        private void ClearObjects()
        {
            int count = objects.Count;
            for (int i = 0; i < count; i++)
            {
                Destroy(objects[i]);
            }
            objects.Clear();
        }

        private void CreateObjects(System.Func<int, Vector3> pfunc)
        {
            if (objects.Count > 0) ClearObjects();
            for (int i = 0; i < numberOfObjects; i++)
            {
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(location);
                go.transform.localPosition = pfunc(i);
                objects.Add(go);
            }
        }


        private System.Func<int, Vector3>[] Distribution => new System.Func<int, Vector3>[] {
        Linear,
        Rand,
        FibDisc,
        FibSphere,
    };

        private Vector3 Linear(int i)
        {
            return Vector3.right * i * radius;
        }

        private Vector3 Rand(int i)
        {
            return Random.insideUnitSphere.normalized * radius;
        }

        private Vector3 FibDisc(int i)
        {
            var k = i + .5f;
            var r = Mathf.Sqrt((k) / numberOfObjects);
            var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;
            var x = r * Mathf.Cos(theta) * radius;
            var y = r * Mathf.Sin(theta) * radius;
            return new Vector3(x, y, 0);
        }

        private Vector3 FibSphere(int i)
        {
            var k = i + .5f;
            var phi = Mathf.Acos(1f - 2f * k / numberOfObjects);
            var theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;
            var x = Mathf.Cos(theta) * Mathf.Sin(phi);
            var y = Mathf.Sin(theta) * Mathf.Sin(phi);
            var z = Mathf.Cos(phi);
            return new Vector3(x, y, z) * radius;
        }
    }
}