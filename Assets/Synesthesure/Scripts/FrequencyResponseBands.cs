// THIS SCRIPT IS USED TO GET A DATA ARRAY OF THE VALUES FOR FREQUENCY BINS

using UnityEngine;
namespace Synesthesure
{
    public class FrequencyResponseBands : MonoBehaviour
    {
        AudioVisual AV;
        
        [SerializeField] string description;
        [Tooltip("Used to compensate for real audio RMS levels" + "\r\n" + "vs. how a human *perceives* them.")]
        [SerializeField] ResponseCurve _responseCurve;
        [SerializeField] AnimationCurve responseCurve;
        enum Bins { narrow = 32, normal = 64, wide = 128 }
        [SerializeField] Bins bandwidth = Bins.normal;
        Bins previousbandwidth;
        int numberOfBins = 128;
        float[] frequencyBins;
        public float scaleFactor = 1f;

        void Awake()
        {
            AV = AudioVisual.AV;
            SetBinSize();
            if (_responseCurve != null)
            {
                description = _responseCurve.responseCurve.description;
                responseCurve = _responseCurve.responseCurve.curve;
            }
        }
        void SetBinSize()
        {
            previousbandwidth = bandwidth;
            numberOfBins = (int)bandwidth;
            frequencyBins = new float[numberOfBins];
        }

        void Update()
        {
            if (bandwidth != previousbandwidth) SetBinSize();
            for (int i = 0; i < numberOfBins; i++)
            {
                float n = AV.spectrum[i] * responseCurve.Evaluate((float)i / (numberOfBins - 1f)) * scaleFactor;
                frequencyBins[i] = n;
            }
        }

        /// <summary>
        /// A value of 0 - 1.
        /// </summary>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public float GetFrequencyResponse(float frequency)
        {
            frequency = Mathf.Clamp(frequency, 0.0f, 1.0f);
            float bin = frequency * (numberOfBins - 1);
            int lowerBin = (int)bin;
            int upperBin = lowerBin + 1;
            if (upperBin > numberOfBins - 1) upperBin = numberOfBins - 1;
            return Mathf.Lerp(frequencyBins[lowerBin], frequencyBins[upperBin], frequency - (int)frequency);
        }
    }
}