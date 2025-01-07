using UnityEngine;
#if USING_HDRP
using UnityEngine.Rendering.HighDefinition;
# endif

namespace Synesthesure
{
    public class LightModulation : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;

        [Space]
        [SerializeField] Light _light;
        enum LightTypes
        {
            NotSupported = -1, Directional = 0, Point = 1, Spot = 2
        }
        LightTypes lightType;
#if USING_HDRP
        HDAdditionalLightData lightData;
#endif

        [Space]
        [SerializeField] GradientLibrary gradients;
        public int gradientToUse = -1;
        [SerializeField] Gradient gradient;
        int previousGradient;
        public AudioVisual.DynamicTypes colorControlSource;
        [SerializeField] ResponseCurve _colorResponseCurve;
        public AnimationCurve colorResponseCurve;

        [Space]
        public AudioVisual.DynamicTypes intensityControlSource;
        [SerializeField] ResponseCurve _intensityResponseCurve;
        public AnimationCurve intensityResponseCurve;

        [Space]
        public AudioVisual.DynamicTypes rangeControlSource;
        [SerializeField] ResponseCurve _rangeResponseCurve;
        public AnimationCurve rangeResponseCurve;

        [Space]
        public AudioVisual.DynamicTypes spotControlSource;
        [SerializeField] ResponseCurve _spotAngleResponseCurve;
        public AnimationCurve spotAngleCurve;

        void Awake()
        {
            previousGradient = gradientToUse;
            if (gradientToUse > -1 && gradients != null) SetGradient(gradientToUse);
            if (_colorResponseCurve != null) colorResponseCurve = _colorResponseCurve.responseCurve.curve;
            else if (colorResponseCurve == null || colorResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                colorResponseCurve = curve;
            }

            if (_intensityResponseCurve != null) intensityResponseCurve = _intensityResponseCurve.responseCurve.curve;
            else if (intensityResponseCurve == null || intensityResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                intensityResponseCurve = curve;
            }

            if (_rangeResponseCurve != null) rangeResponseCurve = _rangeResponseCurve.responseCurve.curve;
            else if (rangeResponseCurve == null || rangeResponseCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                rangeResponseCurve = curve;
            }

            if (_spotAngleResponseCurve != null) spotAngleCurve = _spotAngleResponseCurve.responseCurve.curve;
            else if (spotAngleCurve == null || spotAngleCurve.keys.Length < 2)
            {
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                spotAngleCurve = curve;
            }

            lightType = LightTypes.NotSupported;
            if (_light)
            {
                if (_light.type == LightType.Directional)
                    lightType = LightTypes.Directional;
                else if (_light.type == LightType.Point)
                    lightType = LightTypes.Point;
                else if (_light.type == LightType.Spot)
                    lightType = LightTypes.Spot;
            }
#if USING_HDRP
            lightData = _light.GetComponent<HDAdditionalLightData>();
#endif
        }

        private void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();
        }


        void Update()
        {
            if (!audioSource.isPlaying || lightType == LightTypes.NotSupported) return;

            if (colorControlSource != AudioVisual.DynamicTypes.None)
            {
                _light.color = gradient.Evaluate(colorResponseCurve.Evaluate(AV.ReactionValue(colorControlSource)));
            }

            if (intensityControlSource != AudioVisual.DynamicTypes.None)
            {
#if USING_HDRP
                lightData.intensity = intensityResponseCurve.Evaluate(AV.ReactionValue(intensityControlSource));
# else
                _light.intensity = intensityResponseCurve.Evaluate(AV.ReactionValue(intensityControlSource));
# endif
            }

            if (lightType != LightTypes.Directional)
            {
                if (rangeControlSource != AudioVisual.DynamicTypes.None)
                {
                    _light.range = rangeResponseCurve.Evaluate(AV.ReactionValue(rangeControlSource));
                }
            }

            if (lightType == LightTypes.Spot)
            {
                if (spotControlSource != AudioVisual.DynamicTypes.None)
                {
                    _light.spotAngle = spotAngleCurve.Evaluate(AV.ReactionValue(spotControlSource));
                }
            }

        }

        // ------------------------------------------------------

        void OnValidate()
        {
            if (gradientToUse != previousGradient)
            {
                previousGradient = gradientToUse;
                SetGradient(gradientToUse);
            }
        }

        public void SetGradient(int index)
        {
            if (gradients == null) return;
            if (index > -1 && index < gradients.gradients.Length)
            {
                gradient = gradients.gradients[index];
                previousGradient = index;
            }
        }
    }

}