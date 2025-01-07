using UnityEngine;

namespace Synesthesure
{
    public class MeterBarsArc : MonoBehaviour
    {
        [SerializeField] FrequencyResponseBands frequencyResponseBands;
        [SerializeField][Range(0f, .1f)] float smoothTime = 0.03f;
        float[] velocities;
        [Space]
        [SerializeField] GameObject meterBarObject;
        [SerializeField] Transform meterBarsTransform;
        [SerializeField] int numberOfBars = 64;
        public float meterBarsWidth = .1f;
        public float meterBarsDepth = .5f;
        public float meterBarsHeightFactor = 20f;
        GameObject[] meterBars;
        Vector3[] meterBars_InitialPosition;
        GameObject[] meterBars_XMirror;
        GameObject[] meterBars_XYMirror1;
        GameObject[] meterBars_XYMirror2;
        Transform[] meterBars_MasterTransform;
        Transform[] meterBars_MasterTransform_XMirror;
        Transform[] meterBars_MasterTransform_XYMirror1;
        Transform[] meterBars_MasterTransform_XYMirror2;
        public float radius = 10f;
        [SerializeField] [Range(20f, 360f)] float arc = 360f;
        [SerializeField] float arcOffset = 0f;
        enum MirrorMode { none = 0, XMirror = 1, XYMirror = 2}
        [SerializeField] MirrorMode mirrorMode = MirrorMode.none;
        [SerializeField] bool depthStick = true;

        [Space]
        public Texture2D colorTexture;
        public float colorYScale = 10f;
        Material[] meterBarsMaterial;

        void Start()
        {
            meterBarsMaterial = new Material[numberOfBars];
            meterBars = new GameObject[numberOfBars];
            meterBars_InitialPosition = new Vector3[numberOfBars];
            meterBars_XMirror = new GameObject[numberOfBars];
            meterBars_XYMirror1 = new GameObject[numberOfBars];
            meterBars_XYMirror2 = new GameObject[numberOfBars];
            meterBars_MasterTransform = new Transform[numberOfBars];
            meterBars_MasterTransform_XMirror = new Transform[numberOfBars];
            meterBars_MasterTransform_XYMirror1 = new Transform[numberOfBars];
            meterBars_MasterTransform_XYMirror2 = new Transform[numberOfBars];
            velocities = new float[numberOfBars];

            GameObject tmpGO = new GameObject();
            for (int i = 0; i < numberOfBars; i++)
            {
                GameObject go = Instantiate(tmpGO,
                    new Vector3(meterBarsTransform.position.x,
                    meterBarsTransform.position.y,
                    meterBarsTransform.position.z),
                    Quaternion.identity, meterBarsTransform);
                meterBars_MasterTransform[i] = go.transform;
                meterBars[i] = Instantiate(meterBarObject,
                    new Vector3(0f, radius, 0f),
                    Quaternion.identity, meterBars_MasterTransform[i]);
                meterBars[i].transform.localScale = new Vector3(meterBarsWidth, .001f, meterBarsDepth);
                meterBars_MasterTransform[i].Rotate(0f, 0f, 360f - i / (numberOfBars - 1f) * arc + arcOffset);
                meterBarsMaterial[i] = meterBars[i].GetComponent<Renderer>().material;

                if (mirrorMode == MirrorMode.XMirror || mirrorMode == MirrorMode.XYMirror)
                {
                    go = Instantiate(tmpGO,
                        new Vector3(meterBarsTransform.position.x,
                        meterBarsTransform.position.y,
                        meterBarsTransform.position.z),
                        Quaternion.identity, meterBarsTransform);
                    meterBars_MasterTransform_XMirror[i] = go.transform;
                    meterBars_XMirror[i] = Instantiate(meterBarObject,
                        new Vector3(0f, radius, 0f),
                        Quaternion.identity, meterBars_MasterTransform_XMirror[i]);
                    meterBars_XMirror[i].transform.localScale = new Vector3(meterBarsWidth, .001f, meterBarsDepth);
                    meterBars_MasterTransform_XMirror[i].Rotate(0f, 0f, i / (numberOfBars - 1f) * arc + arcOffset);
                    meterBars_XMirror[i].GetComponent<Renderer>().material = meterBarsMaterial[i];
                }
                if (mirrorMode == MirrorMode.XYMirror)
                {
                    go = Instantiate(tmpGO,
                        new Vector3(meterBarsTransform.position.x,
                        meterBarsTransform.position.y,
                        meterBarsTransform.position.z),
                        Quaternion.identity, meterBarsTransform);
                    meterBars_MasterTransform_XYMirror1[i] = go.transform;
                    meterBars_XYMirror1[i] = Instantiate(meterBarObject,
                        new Vector3(0f, radius, 0f),
                        Quaternion.identity, meterBars_MasterTransform_XYMirror1[i]);
                    meterBars_XYMirror1[i].transform.localScale = new Vector3(meterBarsWidth, .001f, meterBarsDepth);
                    meterBars_MasterTransform_XYMirror1[i].Rotate(0f, 0f, 180f - i / (numberOfBars - 1f) * arc + arcOffset);
                    meterBars_XYMirror1[i].GetComponent<Renderer>().material = meterBarsMaterial[i];

                    go = Instantiate(tmpGO,
                        new Vector3(meterBarsTransform.position.x,
                        meterBarsTransform.position.y,
                        meterBarsTransform.position.z),
                        Quaternion.identity, meterBarsTransform);
                    meterBars_MasterTransform_XYMirror2[i] = go.transform;
                    meterBars_XYMirror2[i] = Instantiate(meterBarObject,
                        new Vector3(0f, radius, 0f),
                        Quaternion.identity, meterBars_MasterTransform_XYMirror2[i]);
                    meterBars_XYMirror2[i].transform.localScale = new Vector3(meterBarsWidth, .001f, meterBarsDepth);
                    meterBars_MasterTransform_XYMirror2[i].Rotate(0f, 0f, -180 + i / (numberOfBars - 1f) * arc + arcOffset);
                    meterBars_XYMirror2[i].GetComponent<Renderer>().material = meterBarsMaterial[i];
                }

                meterBars_InitialPosition[i] = meterBars[i].transform.position;
            }
            Destroy(tmpGO);
        }

        void Update()
        {
            int textureWidth = colorTexture.width;
            int textureHeight = colorTexture.height;
            for (int i = 0; i < numberOfBars; i++)
            {
                float n = frequencyResponseBands.GetFrequencyResponse((float)i / ((float)numberOfBars - 1f)) * meterBarsHeightFactor;

                n = Mathf.SmoothDamp(meterBars[i].transform.localScale.z, n, ref velocities[i], smoothTime);

                meterBarsMaterial[i].color = colorTexture.GetPixel(
                    (int)((float)i / ((float)numberOfBars - 1f) * ((float)textureWidth - 1f)),
                    (int)Mathf.Clamp(n * colorYScale, 0, textureHeight));

                meterBars[i].transform.position = meterBars_InitialPosition[i];

                meterBars[i].transform.localScale = new Vector3(meterBarsWidth, meterBarsDepth, n);
                if (mirrorMode == MirrorMode.XMirror || mirrorMode == MirrorMode.XYMirror)
                    meterBars_XMirror[i].transform.localScale = new Vector3(meterBarsWidth, meterBarsDepth, n);
                if (mirrorMode == MirrorMode.XYMirror)
                {
                    meterBars_XYMirror1[i].transform.localScale = new Vector3(meterBarsWidth, meterBarsDepth, n);
                    meterBars_XYMirror2[i].transform.localScale = new Vector3(meterBarsWidth, meterBarsDepth, n);
                }
                if (depthStick)
                {
                    meterBars[i].transform.position = new Vector3(
                        meterBars[i].transform.position.x,
                        meterBars[i].transform.position.y,
                        n * -0.5f);
                    if (mirrorMode == MirrorMode.XMirror || mirrorMode == MirrorMode.XYMirror)
                    {
                        meterBars_XMirror[i].transform.position = new Vector3(
                            meterBars_XMirror[i].transform.position.x,
                            meterBars_XMirror[i].transform.position.y,
                            n * -0.5f);
                    }
                    if (mirrorMode == MirrorMode.XYMirror)
                    {
                        meterBars_XYMirror1[i].transform.position = new Vector3(
                            meterBars_XYMirror1[i].transform.position.x,
                            meterBars_XYMirror1[i].transform.position.y,
                            n * -0.5f);
                        meterBars_XYMirror2[i].transform.position = new Vector3(
                            meterBars_XYMirror2[i].transform.position.x,
                            meterBars_XYMirror2[i].transform.position.y,
                            n * -0.5f);
                    }
                }
            }
        }


    }
}
