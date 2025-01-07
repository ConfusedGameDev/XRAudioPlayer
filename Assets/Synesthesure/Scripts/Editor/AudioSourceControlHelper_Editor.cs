using UnityEngine;
using UnityEditor;

namespace Synesthesure
{
    [CustomEditor(typeof(AudioSourceControlHelper))]
    public class AudioSourceControlHelper_Editor : Editor
    {
        string playPauseText = "Pause";
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (Application.isPlaying)
            {
                AudioSourceControlHelper helper = (AudioSourceControlHelper)target;
                if (GUILayout.Button(playPauseText))
                {
                    if (Application.isPlaying)
                    {
                        helper.PlayPause();
                        if (helper.audioSource.isPlaying) playPauseText = "Pause";
                        else playPauseText = "Play";
                    }
                }
                if (GUILayout.Button("Stop"))
                {
                    if (Application.isPlaying)
                    {
                        helper.Stop();
                        playPauseText = "Play";
                    }
                }
            }
        }
    }
}