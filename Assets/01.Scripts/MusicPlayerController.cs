using UnityEngine;

public class MusicPlayerController : MonoBehaviour
{ 
    public AudioClip[] audioClips;
    public AudioSource AudioSource;
    int currentClip=0;
    void Start()
    {
        
    }

    public void Playtrack(int i = 0)
    {
        if(AudioSource && audioClips.Length>i)
        {
            AudioSource.Stop();
            currentClip = i;
            AudioSource.clip=audioClips[currentClip];
            AudioSource.Play();
        }

    }
    public void NextTrack()
    {
        currentClip = currentClip + 1 < audioClips.Length ? currentClip + 1 : 0;
        Playtrack(currentClip);
    }
    void Update()
    {
        
    }
}
