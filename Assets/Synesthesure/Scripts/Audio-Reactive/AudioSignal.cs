using UnityEngine;

namespace Synesthesure
{
    public class AudioSignal : MonoBehaviour
    {
        [SerializeField] FrequencyResponseBands frequencyResponseBands;
        [SerializeField][Range(0f, .1f)] float smoothTime = 0.03f;
        float[] velocities;
        [Space]
        public float signalWidth = 16f;
        [SerializeField] int numberOfPoints = 64;
        public float scaleFactor = 20f;
        [Space]
        [SerializeField] LineRenderer[] lines;
        float[] values;
        float n;
        AnimationCurve curve = new AnimationCurve();
        Keyframe[] keys;
        public bool widthModulatedByAudio;
        public float width = 1f;
        public float minWidth = .1f;
        public float maxWidth = 10f;

        void Start()
        {
            values = new float[numberOfPoints];
            velocities = new float[numberOfPoints];
            for (int line = 0; line < lines.Length; line++)
            {
                curve = new AnimationCurve();
                lines[line].positionCount = numberOfPoints;
                for (int i = 0; i < numberOfPoints; i++)
                {
                    lines[line].SetPosition(i, new Vector3((signalWidth / (numberOfPoints - 1f) * i) - (signalWidth * .5f), 0f, 0));
                    if (widthModulatedByAudio) curve.AddKey((float)i / (float)(numberOfPoints - 1), width);
                }
                if (widthModulatedByAudio) lines[line].widthCurve = curve;
            }
            if (widthModulatedByAudio) keys = lines[0].widthCurve.keys;
        }

        void Update()
        {
            for (int line = 0; line < lines.Length; line++)
            {

                for (int i = 0; i < numberOfPoints; i++)
                {
                    n = frequencyResponseBands.GetFrequencyResponse((float)i / ((float)numberOfPoints - 1f)) * scaleFactor;
                    n = Mathf.SmoothDamp(values[i], n, ref velocities[i], smoothTime);
                    values[i] = n;
                    lines[line].SetPosition(i, new Vector3((signalWidth / (numberOfPoints - 1f) * i) - (signalWidth * .5f), n, 0));
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
                    lines[line].widthCurve = curve;
                }
            }
        }
    }
}
