using UnityEngine;
using UnityEngine.Playables;

public class AnimationController : MonoBehaviour
{
    public PlayableDirector openSongSelection, closeSongSelection;

    public void openSongMenu()=> openSongSelection.Play();
    public void closeSongMenu()=> closeSongSelection.Play();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
