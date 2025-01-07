using UnityEngine;

namespace Synesthesure
{
    public class MeterBars : MonoBehaviour
    {
        [SerializeField] FrequencyResponseBands frequencyResponseBands;
        [SerializeField][Range(0f, .1f)] float smoothTime = 0.03f;
        float[] velocities;
        [Space]
        public GameObject meterBarObject;
        public Transform meterBarsTransform;
        public float meterWidth = 16f;
        [SerializeField] int numberOfBars = 64;
        public float meterBarsWidth = .1f;
        public float meterBarsDepth = .5f;
        public float meterBarsHeightFactor = 20f;
        GameObject[] meterBars;
        [Space]
        public Texture2D colorTexture;
        public float colorYScale = 10f;
        Material[] meterBarsMaterial;
        int textureWidth;
        int textureHeight;

        void Start()
        {
            velocities = new float[numberOfBars];
            meterBarsMaterial = new Material[numberOfBars];
            meterBars = new GameObject[numberOfBars];
            for (int i = 0; i < numberOfBars; i++)
            {
                meterBars[i] = Instantiate(meterBarObject, new Vector3(meterBarsTransform.position.x + (meterWidth / (float)numberOfBars * i) - (meterWidth / 2f), meterBarsTransform.position.y, meterBarsTransform.position.z), Quaternion.identity, meterBarsTransform);
                meterBars[i].transform.localScale = new Vector3(meterBarsWidth, .001f, meterBarsDepth);
                // Color
                meterBarsMaterial[i] = meterBars[i].GetComponent<Renderer>().material;
            }
            if (colorTexture)
            {
                textureWidth = colorTexture.width;
                textureHeight = colorTexture.height;
            }
        }

        void Update()
        {
            for (int i = 0; i < numberOfBars; i++)
            {
                float n = frequencyResponseBands.GetFrequencyResponse((float)i / ((float)numberOfBars - 1f)) * meterBarsHeightFactor;
                n = Mathf.SmoothDamp(meterBars[i].transform.localScale.z, n, ref velocities[i], smoothTime);
                meterBars[i].transform.localScale = new Vector3(meterBarsWidth, n, meterBarsDepth);
                // Colorization
                if (colorTexture)
                {
                    meterBarsMaterial[i].color = colorTexture.GetPixel(
                    (int)((float)i / ((float)numberOfBars - 1f) * ((float)textureWidth - 1f)),
                    (int)Mathf.Clamp(n * colorYScale, 0, textureHeight));
                }
            }
        }

    }
}
