using UnityEngine;

namespace Synesthesure
{
    public class TorqueModulation : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;
        public AudioVisual.DynamicTypes controlSource;
        float reaction;
        [SerializeField] ResponseCurve _responseCurve;
        public AnimationCurve responseCurve;

        [Space]
        [SerializeField] GameObject theObject;
        Rigidbody rb;
        public Vector3 torque = new Vector3(1f, 0f, 0f);

        void Awake()
        {
            rb = theObject.GetComponent<Rigidbody>();
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
        }


        void Update()
        {
            if (!audioSource.isPlaying || rb == null) return;

            if (controlSource != AudioVisual.DynamicTypes.None)
            {
                reaction = responseCurve.Evaluate(AV.ReactionValue(controlSource));
                rb.AddTorque(new Vector3(reaction * torque.x, reaction * torque.y, reaction * torque.z));
            }
        }
    }
}