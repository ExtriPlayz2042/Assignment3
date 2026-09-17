using UnityEngine;

public sealed class GameAudioController : MonoBehaviour
{
    public AudioClip intro, startScene, ghostNormal, ghostScared, ghostDead;
    public AudioClip moving, eatPellet, eatGhost, eatBonus, collideWall, death;
    public AudioSource introSource, musicSource, effectsSource;
    public bool menuOnly;
    public double ScheduledStart { 
        get; 
        private set; 
    }
    public double ScheduledTransition { 
        get; 
        private set; 
    }
    public bool NormalMusicStarted => !menuOnly && AudioSettings.dspTime >= ScheduledTransition && musicSource.isPlaying;

    void Start()
    {
        if (menuOnly)
        {
            musicSource.clip = startScene; musicSource.loop = true; musicSource.Play(); 
        } else {
            BeginIntro();
        }
    }
    public void BeginIntro()
    {
        introSource.Stop(); 
        musicSource.Stop();
        introSource.loop = false; 
        musicSource.loop = true;
        ScheduledStart = AudioSettings.dspTime + 0.05;
        if (intro == null) {
            ScheduledTransition = ScheduledStart;
        } else {
            ScheduledTransition = ScheduledStart + Mathf.Min(3f, intro.length);
        }
        if (intro != null)
        {
            introSource.clip = intro;
            introSource.PlayScheduled(ScheduledStart);
            introSource.SetScheduledEndTime(ScheduledTransition);
        }
        musicSource.clip = ghostNormal;
        
        if (ghostNormal == null) {
            double test = 1;
        } else {
            musicSource.PlayScheduled(ScheduledTransition);
        }
    }
    public void PlayEffect(AudioClip clip) { 
        if (clip == null) {
            double test = 1;
        } else {
            effectsSource.PlayOneShot(clip); 
        }
    }
    public void SetGhostMusic(int state)
    {
        introSource.Stop(); 
        musicSource.Stop();
        musicSource.clip = state == 1 ? ghostScared : state == 2 ? ghostDead : ghostNormal;
        musicSource.loop = true; 
        if (musicSource.clip == null) {
            double test = 1;
        } else {
            musicSource.Play();
        }
    }
}
