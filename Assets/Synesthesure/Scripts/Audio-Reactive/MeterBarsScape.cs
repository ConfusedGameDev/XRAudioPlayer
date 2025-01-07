using UnityEngine;

namespace Synesthesure
{
    public class MeterBarsScape : MonoBehaviour
    {
        [SerializeField] FrequencyResponseBands frequencyResponseBands;
        public float refreshRate = 30;
        float previousRefresh;
        [Space]
        public Texture2D colorTexture;
        public float colorYScale = 10f;
        [Space]
        public GameObject meterBarObject;
        public Transform meterBarsTransform;
        public float meterWidth = 16f;
        public float meterDepth = 16f;
        [SerializeField] int numberOfBarsWidth = 64;
        [SerializeField] int numberOfBarsDepth = 32;
        public float meterBarsWidth = .1f;
        public float meterBarsDepth = .5f;
        public float meterBarsHeightFactor = 20f;
        GameObject[,] meterBars;
        GameObject[,] meterBarsMirror;
        Material[,] meterBarsMaterial;

        void Start()
        {
            meterBars = new GameObject[numberOfBarsWidth, numberOfBarsDepth];
            meterBarsMirror = new GameObject[numberOfBarsWidth, numberOfBarsDepth];
            meterBarsMaterial = new Material[numberOfBarsWidth, numberOfBarsDepth];
            float offset = .5f * meterWidth / (float)numberOfBarsWidth;
            for (int ii = 0; ii < numberOfBarsDepth; ii++)
            {
                for (int i = 0; i < numberOfBarsWidth; i++)
                {
                    meterBars[i, ii] = Instantiate(meterBarObject,
                        new Vector3(.5f * offset + meterBarsTransform.position.x - .25f * meterWidth + (offset * i) - (.5f * meterWidth / 2f),
                        meterBarsTransform.position.y,
                        meterBarsTransform.position.z - (meterDepth / (float)numberOfBarsDepth * ii) - (meterDepth / 2f)),
                        Quaternion.identity, meterBarsTransform);
                    meterBars[i, ii].transform.localScale = new Vector3(meterBarsWidth, .001f, meterBarsDepth);
                    meterBarsMirror[i, ii] = Instantiate(meterBarObject,
                        new Vector3(-.5f * offset + meterBarsTransform.position.x + .75f * meterWidth - (offset * i) - (.5f * meterWidth / 2f),
                        meterBarsTransform.position.y,
                        meterBarsTransform.position.z - (meterDepth / (float)numberOfBarsDepth * ii) - (meterDepth / 2f)),
                        Quaternion.identity, meterBarsTransform);
                    meterBarsMirror[i, ii].transform.localScale = new Vector3(meterBarsWidth, .001f, meterBarsDepth);
                    // Color
                    meterBarsMaterial[i, ii] = meterBars[i, ii].GetComponent<Renderer>().material;
                    meterBarsMirror[i, ii].GetComponent<Renderer>().material = meterBarsMaterial[i, ii];
                }
            }

        }

        void Update()
        {
            int textureWidth = colorTexture.width;
            int textureHeight = colorTexture.height;
            //  Current Input
            for (int i = 0; i < numberOfBarsWidth; i++)
            {
                float n = frequencyResponseBands.GetFrequencyResponse((float)i / ((float)numberOfBarsWidth - 1f)) * meterBarsHeightFactor;
                meterBars[i, 0].transform.localScale = new Vector3(meterBarsWidth, n, meterBarsDepth);
                meterBarsMirror[i, 0].transform.localScale = new Vector3(meterBarsWidth, n, meterBarsDepth);
                // Colorization
                meterBarsMaterial[i, 0].color = colorTexture.GetPixel(
                    (int)((float)i / ((float)numberOfBarsWidth - 1f) * ((float)textureWidth - 1f)),
                    (int)Mathf.Clamp(n * colorYScale, 0, textureHeight));
            }
            // Histogram
            if (Time.time - previousRefresh > 1f / refreshRate)
            {
                previousRefresh = Time.time;
                for (int ii = numberOfBarsDepth - 1; ii > 0; ii--)
                {
                    for (int i = 0; i < numberOfBarsWidth; i++)
                    {
                        meterBars[i, ii].transform.localScale =
                            new Vector3(meterBarsWidth, meterBars[i, ii - 1].transform.localScale.y, meterBarsDepth);
                        meterBarsMirror[i, ii].transform.localScale = meterBars[i, ii].transform.localScale;
                        // Color
                        meterBarsMaterial[i, ii].color = meterBarsMaterial[i, ii - 1].color;
                    }
                }
            }

        }
    }
}