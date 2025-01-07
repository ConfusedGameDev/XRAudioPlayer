using UnityEngine;

namespace Synesthesure
{
    public class ParticleControl : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;

        [SerializeField] string note;

        public ParticleSystem particles;
        ParticleSystem.MainModule main;
        ParticleSystem.EmissionModule emissionModule;
        ParticleSystem.NoiseModule noiseModule;

        [Space]
        public AudioVisual.DynamicTypes colorControlSource;
        [SerializeField] ResponseCurve _colorResponseCurve;
        public AnimationCurve colorResponseCurve;
        [SerializeField] GradientLibrary gradients;
        public int gradientToUse = -1;
        [SerializeField] Gradient gradient;
        //int previousGradient;
        [Space]
        public AudioVisual.DynamicTypes amountControlSource;
        [SerializeField] ResponseCurve _amountResponseCurve;
        public AnimationCurve amountResponseCurve;
        public float minAmount;
        public float maxAmount;
        [Space]
        public AudioVisual.DynamicTypes speedControlSource;
        [SerializeField] ResponseCurve _speedResponseCurve;
        public AnimationCurve speedResponseCurve;
        public float minSpeed;
        public float maxSpeed;
        [Space]
        public AudioVisual.DynamicTypes noiseStrengthControlSource;
        [SerializeField] ResponseCurve _noiseStrengthResponseCurve;
        public AnimationCurve noiseStrengthResponseCurve;
        public float minNoiseStrength;
        public float maxNoiseStrength;
        [Space]
        public AudioVisual.DynamicTypes noiseFrequencyControlSource;
        [SerializeField] ResponseCurve _noiseFrequencyResponseCurve;
        public AnimationCurve noiseFrequencyResponseCurve;
        public float minNoiseFrequency;
        public float maxNoiseFrequency;
        [Space]
        public AudioVisual.DynamicTypes noiseSpeedControlSource;
        [SerializeField] ResponseCurve _noiseSpeedResponseCurve;
        public AnimationCurve noiseSpeedResponseCurve;
        public float minNoiseSpeed;
        public float maxNoiseSpeed;
        [Space]
        public AudioVisual.DynamicTypes noiseOctaveScaleControlSource;
        [SerializeField] ResponseCurve _noiseOctaveScaleResponseCurve;
        public AnimationCurve noiseOctaveScaleResponseCurve;
        public float minNoiseOctaveScale;
        public float maxNoiseOctaveScale;
        [Space]
        public AudioVisual.DynamicTypes noisePositionAmountControlSource;
        [SerializeField] ResponseCurve _noisePositionAmountResponseCurve;
        public AnimationCurve noisePositionAmountResponseCurve;
        public float minNoisePositionAmount;
        public float maxNoisePositionAmount;
        [Space]
        public AudioVisual.DynamicTypes noiseRotationAmountControlSource;
        [SerializeField] ResponseCurve _noiseRotationAmountResponseCurve;
        public AnimationCurve noiseRotationAmountResponseCurve;
        public float minNoiseRotationAmount;
        public float maxNoiseRotationAmount;
        [Space]
        public AudioVisual.DynamicTypes noiseSizeAmountControlSource;
        [SerializeField] ResponseCurve _noiseSizeAmountResponseCurve;
        public AnimationCurve noiseSizeAmountResponseCurve;
        public float minNoiseSizeAmount;
        public float maxNoiseSizeAmount;

        void Awake()
        {
            main = particles.main;
            emissionModule = particles.emission;
            noiseModule = particles.noise;

            //previousGradient = gradientToUse;
            if (gradientToUse > -1 && gradients != null) SetGradient(gradientToUse);

            if (_colorResponseCurve != null) colorResponseCurve = _colorResponseCurve.responseCurve.curve;
            else if (colorResponseCurve == null || colorResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                colorResponseCurve = curve;
            }
            if (_amountResponseCurve != null) amountResponseCurve = _amountResponseCurve.responseCurve.curve;
            else if (amountResponseCurve == null || amountResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                amountResponseCurve = curve;
            }
            if (_speedResponseCurve != null) speedResponseCurve = _speedResponseCurve.responseCurve.curve;
            else if (speedResponseCurve == null || speedResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                speedResponseCurve = curve;
            }
            if (_noiseStrengthResponseCurve != null) noiseStrengthResponseCurve = _noiseStrengthResponseCurve.responseCurve.curve;
            else if (noiseStrengthResponseCurve == null || noiseStrengthResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                noiseStrengthResponseCurve = curve;
            }
            if (_noiseFrequencyResponseCurve != null) noiseFrequencyResponseCurve = _noiseFrequencyResponseCurve.responseCurve.curve;
            else if (noiseFrequencyResponseCurve == null || noiseFrequencyResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                noiseFrequencyResponseCurve = curve;
            }
            if (_noiseSpeedResponseCurve != null) noiseSpeedResponseCurve = _noiseSpeedResponseCurve.responseCurve.curve;
            else if (noiseSpeedResponseCurve == null || noiseSpeedResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                noiseSpeedResponseCurve = curve;
            }
            if (_noiseOctaveScaleResponseCurve != null) noiseOctaveScaleResponseCurve = _noiseOctaveScaleResponseCurve.responseCurve.curve;
            else if (noiseOctaveScaleResponseCurve == null || noiseOctaveScaleResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                noiseOctaveScaleResponseCurve = curve;
            }
            if (_noisePositionAmountResponseCurve != null) noisePositionAmountResponseCurve = _noisePositionAmountResponseCurve.responseCurve.curve;
            else if (noisePositionAmountResponseCurve == null || noisePositionAmountResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                noisePositionAmountResponseCurve = curve;
            }
            if (_noiseRotationAmountResponseCurve != null) noiseRotationAmountResponseCurve = _noiseRotationAmountResponseCurve.responseCurve.curve;
            else if (noiseRotationAmountResponseCurve == null || noiseRotationAmountResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                noiseRotationAmountResponseCurve = curve;
            }
            if (_noiseSizeAmountResponseCurve != null) noiseSizeAmountResponseCurve = _noiseSizeAmountResponseCurve.responseCurve.curve;
            else if (noiseSizeAmountResponseCurve == null || noiseSizeAmountResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                noiseSizeAmountResponseCurve = curve;
            }
        }

        private void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();            
        }


        void Update()
        {
            if (!audioSource.isPlaying) return;

            if (colorControlSource != AudioVisual.DynamicTypes.None)
                main.startColor = gradient.Evaluate(colorResponseCurve.Evaluate(AV.ReactionValue(colorControlSource)));
            if (amountControlSource != AudioVisual.DynamicTypes.None || minAmount != maxAmount)
                emissionModule.rateOverTime = Mathf.Lerp(minAmount, maxAmount, amountResponseCurve.Evaluate(AV.ReactionValue(amountControlSource)));
            if (speedControlSource != AudioVisual.DynamicTypes.None || minSpeed != maxSpeed)
                main.startSpeed = Mathf.Lerp(minSpeed, maxSpeed, speedResponseCurve.Evaluate(AV.ReactionValue(speedControlSource)));
            if (noiseStrengthControlSource != AudioVisual.DynamicTypes.None || minNoiseStrength != maxNoiseStrength)
                noiseModule.strength = Mathf.Lerp(minNoiseStrength, maxNoiseStrength, noiseStrengthResponseCurve.Evaluate(AV.ReactionValue(noiseStrengthControlSource)));
            if (noiseFrequencyControlSource != AudioVisual.DynamicTypes.None || minNoiseFrequency != maxNoiseFrequency)
                noiseModule.frequency = Mathf.Lerp(minNoiseFrequency, maxNoiseFrequency, noiseFrequencyResponseCurve.Evaluate(AV.ReactionValue(noiseFrequencyControlSource)));
            if (noiseSpeedControlSource != AudioVisual.DynamicTypes.None || minNoiseSpeed != maxNoiseSpeed)
                noiseModule.scrollSpeed = Mathf.Lerp(minNoiseSpeed, maxNoiseSpeed, noiseSpeedResponseCurve.Evaluate(AV.ReactionValue(noiseSpeedControlSource)));
            if (noiseOctaveScaleControlSource != AudioVisual.DynamicTypes.None || minNoiseOctaveScale != maxNoiseOctaveScale)
                noiseModule.octaveScale = Mathf.Lerp(minNoiseOctaveScale, maxNoiseOctaveScale, noiseOctaveScaleResponseCurve.Evaluate(AV.ReactionValue(noiseOctaveScaleControlSource)));
            if (noisePositionAmountControlSource != AudioVisual.DynamicTypes.None || minNoisePositionAmount != maxNoisePositionAmount)
                noiseModule.positionAmount = Mathf.Lerp(minNoisePositionAmount, maxNoisePositionAmount, noisePositionAmountResponseCurve.Evaluate(AV.ReactionValue(noisePositionAmountControlSource)));
            if (noiseRotationAmountControlSource != AudioVisual.DynamicTypes.None || minNoiseRotationAmount != maxNoiseRotationAmount)
                noiseModule.rotationAmount = Mathf.Lerp(minNoiseRotationAmount, maxNoiseRotationAmount, noiseRotationAmountResponseCurve.Evaluate(AV.ReactionValue(noiseRotationAmountControlSource)));
            if (noiseSizeAmountControlSource != AudioVisual.DynamicTypes.None || minNoiseSizeAmount != maxNoiseSizeAmount)
                noiseModule.sizeAmount = Mathf.Lerp(minNoiseSizeAmount, maxNoiseSizeAmount, noiseSizeAmountResponseCurve.Evaluate(AV.ReactionValue(noiseSizeAmountControlSource)));
        }

        // ------------------------------------------------------

        void OnValidate()
        {
            //if (gradientToUse != previousGradient)
            //{
            //    previousGradient = gradientToUse;
                SetGradient(gradientToUse);
            //}
        }

        public void SetGradient(int index)
        {
            if (gradients == null) return;
            if (index < 0 || index >= gradients.gradients.Length) return;
            gradient = gradients.gradients[index];
            //   previousGradient = index;
#if UNITY_EDITOR
            GradientCacheWrapper.ClearCache();
#endif
        }

    }
}