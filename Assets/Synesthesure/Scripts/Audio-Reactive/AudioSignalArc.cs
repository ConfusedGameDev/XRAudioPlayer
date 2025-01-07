using UnityEngine;

namespace Synesthesure
{
    public class AudioSignalArc : MonoBehaviour
    {
        [SerializeField] FrequencyResponseBands frequencyResponseBands;
        [SerializeField][Range(0f, .1f)] float smoothTime = 0.03f;
        float[] velocities;
        [Space]
        [SerializeField] LineRenderer line;
        Transform _transform;
        [SerializeField] int numberOfPoints = 64;
        public float startingDegree = 0f;
        public float endingDegree = 180f;
        float startingRadian;
        float endingRadian;
        float stepSize;
        public float scaleFactor = 20f;
        public float xScale = 1f;
        public float yScale = 1f;
        public bool widthModulatedByAudio;
        public float width = 1f;
        public float minWidth = .1f;
        public float maxWidth = 10f; 
        float[] values;
        float n;
        float x;
        float y;
        AnimationCurve curve = new AnimationCurve();
        Keyframe[] keys;

        void Start()
        {
            values = new float[numberOfPoints];
            velocities = new float[numberOfPoints];
            _transform = line.transform;
            line.positionCount = numberOfPoints;
            startingRadian = (startingDegree + 90f) / 360f * 2f * Mathf.PI;
            endingRadian = (endingDegree + 90f) / 360f * 2f * Mathf.PI;
            stepSize = (startingRadian - endingRadian) / (numberOfPoints - 1f);
            for (int i = 0; i < numberOfPoints; i++)
            {
                line.SetPosition(i, new Vector3(0f, 0f, 0f));
                if (widthModulatedByAudio) curve.AddKey((float)i / (float)(numberOfPoints - 1), width);
            }
            if (widthModulatedByAudio)
            {
                line.widthCurve = curve;
                keys = line.widthCurve.keys;
            }
        }

        void Update()
        {
            startingRadian = (startingDegree + 90f) / 360f * 2f * Mathf.PI;
            endingRadian = (endingDegree + 90f) / 360f * 2f * Mathf.PI;
            stepSize = (startingRadian - endingRadian) / (numberOfPoints - 1f);
            for (int i = 0; i < numberOfPoints; i++)
            {
                n = frequencyResponseBands.GetFrequencyResponse((float)i / ((float)numberOfPoints - 1f)) * scaleFactor;
                n = Mathf.SmoothDamp(values[i], n, ref velocities[i], smoothTime);
                values[i] = n;
                x = Mathf.Cos(startingRadian + stepSize * i) * n * xScale + _transform.position.x;
                y = Mathf.Sin(startingRadian + stepSize * i) * n * yScale + _transform.position.y;
                line.SetPosition(i, new Vector3(x, y, 0f));
                if (widthModulatedByAudio)
                {
                    n *= width;
                    n = Mathf.Clamp(n, minWidth, maxWidth);
                    keys[i].value = n;
                }
            }
            if (widthModulatedByAudio)
            {
                curve.keys = keys;
                line.widthCurve = curve;
            }
        }
    }
}
