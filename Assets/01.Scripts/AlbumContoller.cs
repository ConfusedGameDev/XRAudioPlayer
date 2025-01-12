using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
public class AlbumContoller : SerializedMonoBehaviour
{
    public PlayableDirector rightMovement, leftMovement, downMovement,upMovement;

    public List<AlbumMaterialController> materialControllers;

    public AnimationController animationController;

    public bool isPlaying=false;
    public AudioSource audioSource;

    public int currentAlbum;
    public Dictionary<int, AudioClip> clips;
    public Dictionary<int, GameObject> lightContoller;
    public GameObject mellowLight, discoLight;
    [Button]
    public void goToNext()
    {
        currentAlbum++;
        if (currentAlbum >6)
            currentAlbum = -6;

        if (  upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;
       
        if (lastDirector != null && lastDirector != rightMovement)
        {
            StartCoroutine(playDirectorBackwards(leftMovement, 0.5f));
            updateTexturesNext = false;
            updateTexturesPrev = true;
        }
        else
        {
            if (updateTexturesNext)
            {
                for (int i = 0; i < materialControllers.Count; i++)
                {
                    if (i == materialControllers.Count - 1)
                    {
                        materialControllers[materialControllers.Count - 1].offset = materialControllers[0].offset;
                    }
                    else
                    {
                        materialControllers[i].offset = materialControllers[i + 1].offset;

                    }
                    materialControllers[i].updateOffset();

                }
            }
            else
            {
                updateTexturesNext = true;
            }
            rightMovement.Play();
            lastDirector = rightMovement;
        }
         
        
    }
    public PlayableDirector lastDirector;
    bool updateTexturesNext = true;
    bool updateTexturesPrev = true;


    public IEnumerator playDirectorBackwards(PlayableDirector director, float duration)
    {
        var delta = 0f;
        director.timeUpdateMode = DirectorUpdateMode.Manual;
        while(delta < duration)
        {
            yield return null;
            director.time = Mathf.Lerp(0.5f, 0f, delta / duration);
            director.Evaluate();
            delta += Time.deltaTime;

        }
        director.timeUpdateMode = DirectorUpdateMode.GameTime;
        director.Stop();
        lastDirector = null;
    }
    [Button]
    public void goToPrev()
    {
        currentAlbum--;
        if (currentAlbum < -6)
            currentAlbum = 6;
        if ( upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;
       
        if (lastDirector != null && lastDirector!= leftMovement)
        {
            StartCoroutine(playDirectorBackwards(rightMovement, 0.5f));
            updateTexturesPrev = false;
            updateTexturesNext = true;
        }
        else
        {
            if (updateTexturesPrev)
            {

                for (int i = materialControllers.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        materialControllers[0].offset = materialControllers[materialControllers.Count - 1].offset;
                    }

                    else
                    {
                        materialControllers[i].offset = materialControllers[i - 1].offset;

                    }
                    materialControllers[i].updateOffset();

                }
            }
            else
            {
                updateTexturesPrev=true;
            }
            leftMovement.Play();
            lastDirector = leftMovement;
        }
        


    }
    public bool canGoUp { get; set; }
    [Button]
    public void goUp()
    {
        if (lightContoller.ContainsKey(currentAlbum))
        {
            lightContoller[currentAlbum].SetActive(false);
        }
        Debug.Log("TRYING TO GO UP " + canGoUp);
        if (!canGoUp) return;
        if(downMovement.state== PlayState.Playing)
            downMovement.Stop();
        upMovement.Play();
        isPlaying = false;
        animationController.closeSongMenu();
        audioSource.Stop();
        canGoUp=false;
    }
   public int playingAlbum = -99;
    public GameObject fakeLP;
    float lastvolume;

    IEnumerator fadeOutSound()
    {
        var delta = 0f;
        while(delta<1.5f)
        {
            delta += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(lastvolume, 0f, delta / 1.5f);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = lastvolume;
    }
    [Button]
    public void goDown()
    {
        if (playingAlbum== currentAlbum ||  upMovement.time > 0f || rightMovement.time > 0 || leftMovement.time > 0f) return;

        Debug.Log("GoingDown");
        if(downMovement.state== PlayState.Playing)
        {
            lastvolume= audioSource.volume; 
            StartCoroutine(fadeOutSound()); 
            animationController.closeSongMenu();
            downMovement.Stop();
            if(fakeLP)
                fakeLP.SetActive(true);
        }
           
        if (lightContoller.ContainsKey(currentAlbum))
        {
            lightContoller[currentAlbum].SetActive(false);
        }
        playingAlbum = currentAlbum;
        downMovement.Play();
        isPlaying = true;
        StartCoroutine(playAfterDelay());
        StartCoroutine(canGoUpEnable());    

    }
    public IEnumerator canGoUpEnable()
    {
        yield return new WaitForSeconds(3.316667f);
        canGoUp = true;
    }

    
    public IEnumerator playAfterDelay()
    {
        yield return new WaitForSeconds(3.316667f);
        if (fakeLP)
            fakeLP.SetActive(false);
        if (clips.ContainsKey(currentAlbum))
        {
            audioSource.clip = clips[currentAlbum];
        }
        if (lightContoller.ContainsKey(currentAlbum))
        {
            lightContoller[currentAlbum].SetActive(true);
        }
        audioSource.Play();
        animationController.openSongMenu();

    }

    private void RightMovement_stopped(PlayableDirector obj)
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canGoUp = false;
        if (rightMovement != null)
            rightMovement.stopped += RightMovement_stopped;
        lightContoller = new Dictionary<int, GameObject>();
        for (int i = -6;i<7; i++)
        {
            lightContoller.Add(i, i%2==0? mellowLight:discoLight);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
