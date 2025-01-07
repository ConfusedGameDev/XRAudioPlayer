using UnityEngine;

namespace Synesthesure
{
    public class PositionModulation : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;

        [SerializeField] string note;
        [SerializeField] GameObject theObject;

        public AudioVisual.DynamicTypes controlSource;
        [SerializeField] ResponseCurve _responseCurve;
        public AnimationCurve responseCurve;

        public enum Methods
        {
            Absolute = 0, Relative = 1, Transforms = 2
        }
        public Methods method;

        Vector3 initialPosition;

        [SerializeField] Vector3 startPosition;
        [SerializeField] Vector3 endPosition;

        [SerializeField] Vector3 offsetPosition;

        [SerializeField] Transform startTransformPosition;
        [SerializeField] Transform endTransformPosition;

        Vector3 vec3;
        float tmpFloat;

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

        private void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();

            initialPosition = theObject.transform.position;
        }

        void Update()
        {
            if (!audioSource.isPlaying) return;

            if (controlSource != AudioVisual.DynamicTypes.None)
            {
                tmpFloat = responseCurve.Evaluate(AV.ReactionValue(controlSource));
                switch (method)
                {
                    case Methods.Absolute:
                        vec3 = Vector3.Lerp(startPosition, endPosition, tmpFloat);
                        theObject.transform.position = vec3;
                        break;
                    case Methods.Relative:
                        vec3 = Vector3.Lerp(initialPosition, initialPosition + offsetPosition, tmpFloat);
                        theObject.transform.position = vec3;
                        break;
                    case Methods.Transforms:
                        if (startTransformPosition == null || endTransformPosition == null) break;
                        vec3 = Vector3.Lerp(startTransformPosition.position, endTransformPosition.position, tmpFloat);
                        theObject.transform.position = vec3;
                        break;
                }
            }
        }
    }
}
