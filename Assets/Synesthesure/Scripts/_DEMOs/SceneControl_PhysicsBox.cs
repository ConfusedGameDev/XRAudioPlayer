using UnityEngine;
using System.Collections;
namespace Synesthesure
{
    public class SceneControl_PhysicsBox : MonoBehaviour
    {
        AudioVisual AV;
        AudioSource audioSource;

        [Space]
        [SerializeField] float zoneWidth = 30f;
        [SerializeField] float zoneHeight = 1.5f;
        [SerializeField] float zoneDepth = 30f;
        [SerializeField] Transform spawn;

        [Space]
        [SerializeField] GameObject prefab;
        [SerializeField] int numberOfObjects = 50;
        Rigidbody[] rb;
        [SerializeField] GameObject prefab2;
        [SerializeField] int numberOfObjects2 = 50;
        Rigidbody[] rb2;

        [Header("Beat Triggers")]
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float bassBeatTriggerDurationMinimum = 0.15f;
        [Tooltip("in seconds")][SerializeField][Range(.01f, .25f)] float sibilanceBeatTriggerDurationMinimum = 0.15f;
        bool bassBeatTriggered;
        bool sibilanceBeatTriggered;
        [SerializeField] float upforce = 400f; // bass
        [SerializeField] float sideforce = 400f; // sibilance
        [SerializeField] float bounceforce = 100f; // sibilance

        void Start()
        {
            AV = AudioVisual.AV;
            if (AV.audioInput != null) audioSource = AV.audioInput;
            else if (audioSource == null) audioSource = AV.GetComponent<AudioSource>();

            AV.BeatTrigger_BASS += BassBeatTriggered;
            AV.BeatTrigger_SIBILANCE += SibilanceBeatTriggered;
            AV.RefreshBeatListenerCount();

            rb = new Rigidbody[numberOfObjects];
            rb2 = new Rigidbody[numberOfObjects];
            float x; float y; float z;
            for (int i = 0; i < numberOfObjects; i++)
            {
                GameObject go = Instantiate(prefab, spawn);
                x = Random.Range(0, zoneWidth) - zoneWidth * .5f;
                y = Random.Range(0, zoneHeight) - zoneHeight * .5f;
                z = Random.Range(0, zoneDepth) - zoneDepth * .5f;
                go.transform.position = spawn.transform.position + new Vector3(x, y, z);
                rb[i] = go.GetComponent<Rigidbody>();
            }
            for (int i = 0; i < numberOfObjects2; i++)
            {
                GameObject go = Instantiate(prefab2, spawn);
                x = Random.Range(0, zoneWidth) - zoneWidth * .5f;
                y = Random.Range(0, zoneHeight) - zoneHeight * .5f;
                z = Random.Range(0, zoneDepth) - zoneDepth * .5f;
                go.transform.position = spawn.transform.position + new Vector3(x, y, z);
                rb2[i] = go.GetComponent<Rigidbody>();
            }
        }

        void OnDestroy()
        {
            AV.BeatTrigger_BASS -= BassBeatTriggered;
            AV.BeatTrigger_SIBILANCE -= SibilanceBeatTriggered;
            //AV.RefreshBeatListenerCount(); // ONLY NEED THIS IF LOADING ANOTHER SCREEN AT RUNTIME
        }


        private void Update()
        {
            for (int i = 0; i < numberOfObjects2; i++)
            {
                rb2[i].AddForce(0f, AV.Sibilance.attack * bounceforce, 0f);
            }
        }


        #region // ------- COMMANDS -------

        public void ExecuteCommands(string buffer)
        {
            int charIndex = buffer.IndexOf((char)127);
            if (charIndex < 0) buffer += (char)127;
            string[] commands = buffer.Split((char)127);
            for (int i = 0; i < commands.Length; i++)
            {
                charIndex = commands[i].IndexOf("|");
                if (charIndex > 0)
                {
                    string command = commands[i].Substring(0, charIndex);
                    string commandParameter = commands[i].Substring(charIndex + 1);
                    Commands(command, commandParameter);
                }
            }
        }
        public void ExecuteCommand(string theCommand)
        {
            int charIndex = theCommand.IndexOf("|");
            if (charIndex > 0)
            {
                string command = theCommand.Substring(0, charIndex);
                string commandParameter = theCommand.Substring(charIndex + 1);
                Commands(command, commandParameter);
            }
        }

        void Commands(string theCommand, string theParameter)
        {
            switch (theCommand.ToUpper().Trim())
            {
                case "MUSICSEEK":
                    MusicPosition(float.Parse(theParameter));
                    break;

            }
        }

        // --- THE COMMANDS  ---

        void MusicPosition(float seek)
        {
            audioSource.time = seek;
        }

        #endregion



        #region ------ BEAT CONTROL ------

        public void BassBeatTriggered()
        {
            StartCoroutine(BassBeat());
        }
        IEnumerator BassBeat()
        {
            if (!bassBeatTriggered)
            {// Triggered here
                 bassBeatTriggered = true;
                for (int i = 0; i < numberOfObjects; i++)
                {
                    rb[i].AddForce(0f, upforce, 0f);
                }

                // loops for the duration of 1 triggered beat
                float animationTimeElapsed = 0f;
                while (animationTimeElapsed < bassBeatTriggerDurationMinimum)
                {
                    animationTimeElapsed = animationTimeElapsed + Time.deltaTime;
                    yield return null;
                }
                bassBeatTriggered = false;
            }
        }

        public void SibilanceBeatTriggered()
        {
            StartCoroutine(SibilanceBeat());
        }
        IEnumerator SibilanceBeat()
        {
            if (!sibilanceBeatTriggered)
            {// Triggered here
                sibilanceBeatTriggered = true;
                for (int i = 0; i < numberOfObjects; i++)
                {
                    rb[i].AddForce(Random.Range(-sideforce, sideforce), 0f, Random.Range(-sideforce, sideforce));
                }

                // loops for the duration of 1 triggered beat
                float animationTimeElapsed = 0f;
                while (animationTimeElapsed < sibilanceBeatTriggerDurationMinimum)
                {
                    animationTimeElapsed = animationTimeElapsed + Time.deltaTime;
                    yield return null;
                }
                sibilanceBeatTriggered = false;
            }
        }

        #endregion
    }
}