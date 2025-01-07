using UnityEngine;

namespace Synesthesure
{
    public class GradientModulation : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;
        public AudioVisual.DynamicTypes blendSource;
        float _blend;
        [SerializeField] ResponseCurve _responseCurve;
        public AnimationCurve responseCurve; 
        
        [Space]
        [SerializeField] GradientLibrary gradients;
        public int gradientToUse1 = -1;
        public int gradientToUse2 = -1;
        int previousGradient1 = -1;
        int previousGradient2 = -1;
        public Gradient input1;
        public Gradient input2;
        [HideInInspector] public Gradient outputGradient;
        const int resolution = 8;
        GradientColorKey[] colorKeys;
        GradientAlphaKey[] alphaKeys;
        Color tmpColor1;
        Color tmpColor2;
        float n;

        [Header("Objects with Gradients")]
        [SerializeField] LineRenderer[] lines;

        void Awake()
        {
            if (_responseCurve != null) responseCurve = _responseCurve.responseCurve.curve;
            else if (responseCurve == null || responseCurve.keys.Length < 2)
            { 
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0f); curve.AddKey(1f, 1f);
                responseCurve = curve;
            }
        }

        void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();

            previousGradient1 = gradientToUse1;
            previousGradient2 = gradientToUse2;
            if (gradientToUse1 > -1 && gradients != null) SetGradient1(gradientToUse1);
            if (gradientToUse2 > -1 && gradients != null) SetGradient2(gradientToUse2);
            outputGradient = new Gradient();
            alphaKeys = new GradientAlphaKey[resolution];
            colorKeys = new GradientColorKey[resolution];
            for (int i = 0; i < resolution; i++)
            {
                n = i / (float)(resolution - 1f);
                tmpColor1 = input1.Evaluate(n);
                tmpColor2 = input2.Evaluate(n);
                _blend = responseCurve.Evaluate(AV.ReactionValue(blendSource));
                alphaKeys[i].alpha = ((2f - 2f * _blend) * tmpColor1.a + 2f * _blend * tmpColor2.a) * .5f;
                alphaKeys[i].time = n;
                colorKeys[i].color = ((2f - 2f * _blend) * tmpColor1 + 2f * _blend * tmpColor2) * .5f;
                colorKeys[i].time = n;
            }
            outputGradient.SetKeys(colorKeys, alphaKeys);
            if (lines != null)
            {
                for (int line = 0; line < lines.Length; line++)
                {
                    lines[line].colorGradient = outputGradient;
                }
            }
        }

        void Update()
        {
            if (!audioSource.isPlaying) return;
            
            for (int i = 0; i < resolution; i++)
            {
                n = i / (float)(resolution - 1f);
                tmpColor1 = input1.Evaluate(n);
                tmpColor2 = input2.Evaluate(n);
                _blend = responseCurve.Evaluate(AV.ReactionValue(blendSource));
                alphaKeys[i].alpha = ((2f - 2f * _blend) * tmpColor1.a + 2f * _blend * tmpColor2.a) * .5f;
                alphaKeys[i].time = n;
                colorKeys[i].color = ((2f - 2f * _blend) * tmpColor1 + 2f * _blend * tmpColor2) * .5f;
                colorKeys[i].time = n;
            }
            outputGradient.SetKeys(colorKeys, alphaKeys);
            if (lines != null)
            {
                for (int line = 0; line < lines.Length; line++)
                {
                    lines[line].colorGradient = outputGradient;
                }
            }
        }

        public void SetInput1(int index)
        {
            if (index > -1 && index < gradients.gradients.Length) input1 = gradients.gradients[index];
        }
        public void SetInput1(float index)
        {
            if (index > -1 && index < gradients.gradients.Length) input1 = gradients.gradients[(int)index];
        }
        public void SetInput2(int index)
        {
            if (index > -1 && index < gradients.gradients.Length) input2 = gradients.gradients[index];
        }
        public void SetInput2(float index)
        {
            if (index > -1 && index < gradients.gradients.Length) input2 = gradients.gradients[(int)index];
        }

        // ------------------------------------------------------

        void OnValidate()
        {
            if (gradientToUse1 != previousGradient1)
            {
                previousGradient1 = gradientToUse1;
                SetGradient1(gradientToUse1);
            }
            if (gradientToUse2 != previousGradient2)
            {
                previousGradient2 = gradientToUse2;
                SetGradient2(gradientToUse2);
            }
        }

        public void SetGradient1(int index)
        {
            if (index > -1 && index < gradients.gradients.Length) input1 = gradients.gradients[index];
        }
        public void SetGradient2(int index)
        {
            if (index > -1 && index < gradients.gradients.Length) input2 = gradients.gradients[index];
        }

    }
}