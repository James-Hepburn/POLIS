using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // ── Music ──────────────────────────────────────────────────────────────
    [Header ("Music Sources")]
    public AudioSource musicSource;
    public AudioSource festivalSource;

    [Header ("Music Clips")]
    public AudioClip musicMainTheme;
    public AudioClip musicAthensDay;
    public AudioClip musicAthensNight;
    public AudioClip musicHome;
    public AudioClip musicAgora;
    public AudioClip musicGymnasium;
    public AudioClip musicAcropolis;
    public AudioClip musicHarbour;
    public AudioClip musicKerameikos;
    public AudioClip musicTheatre;
    public AudioClip musicFestival;
    public AudioClip musicEndOfDay;
    public AudioClip musicLegacyScreen;

    // ── SFX ────────────────────────────────────────────────────────────────
    [Header ("SFX Source")]
    public AudioSource sfxSource;

    [Header ("SFX Clips")]
    public AudioClip sfxWorking;
    public AudioClip sfxTalkingToNPC;
    public AudioClip sfxRelationshipGain;
    public AudioClip sfxDrachmaGained;
    public AudioClip sfxHonourGained;
    public AudioClip sfxPrayerOffering;
    public AudioClip sfxDivineFavourIncrease;
    public AudioClip sfxFestivalNotification;
    public AudioClip sfxCareerLevelUp;
    public AudioClip sfxMarriage;
    public AudioClip sfxLegacyPanelOpens;
    public AudioClip sfxDayEndSleep;

    // ── Volume Settings ────────────────────────────────────────────────────
    [Header ("Volume")]
    public float musicVolume   = 0.4f;
    public float festivalVolume = 0.12f;
    public float sfxVolume     = 0.75f;

    [Header ("Fade")]
    public float fadeDuration  = 1f;

    // ── Internal ───────────────────────────────────────────────────────────
    private AudioClip currentMusicClip;
    private bool      festivalLayerActive = false;

    // ══════════════════════════════════════════════════════════════════════
    private void Awake ()
    {
        if (Instance != null && Instance != this) { Destroy (gameObject); return; }
        Instance = this;
        DontDestroyOnLoad (gameObject);

        musicSource.loop   = true;
        musicSource.volume = musicVolume;

        festivalSource.loop   = true;
        festivalSource.volume = 0f;
        festivalSource.clip   = musicFestival;

        sfxSource.loop   = false;
        sfxSource.volume = sfxVolume;
    }

    private void OnEnable ()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable ()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        AudioClip clip = GetMusicForScene (scene.name);
        if (clip != null)
            PlayMusic (clip);

        // Festival layer
        bool isFestival = FestivalManager.Instance != null
                       && FestivalManager.Instance.IsFestivalDay;
        SetFestivalLayer (isFestival);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Music
    // ══════════════════════════════════════════════════════════════════════

    public void PlayMusic (AudioClip clip)
    {
        if (clip == null || clip == currentMusicClip) return;
        currentMusicClip = clip;
        StartCoroutine (CrossfadeMusic (clip));
    }

    private IEnumerator CrossfadeMusic (AudioClip newClip)
    {
        // Fade out
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp (startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play ();

        // Fade in
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp (0f, musicVolume, elapsed / fadeDuration);
            yield return null;
        }
        musicSource.volume = musicVolume;
    }

    public void SetFestivalLayer (bool active)
    {
        if (active == festivalLayerActive) return;
        festivalLayerActive = active;
        StartCoroutine (FadeFestivalLayer (active));
    }

    private IEnumerator FadeFestivalLayer (bool fadeIn)
    {
        float target  = fadeIn ? festivalVolume : 0f;
        float start   = festivalSource.volume;
        float elapsed = 0f;

        if (fadeIn && !festivalSource.isPlaying)
        {
            festivalSource.clip = musicFestival;
            festivalSource.Play ();
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            festivalSource.volume = Mathf.Lerp (start, target, elapsed / fadeDuration);
            yield return null;
        }
        festivalSource.volume = target;

        if (!fadeIn) festivalSource.Stop ();
    }

    // ── Day/night music switch for Athens ─────────────────────────────────
    private void Update ()
    {
        if (SceneManager.GetActiveScene ().name != "Athens") return;
        if (TimeManager.Instance == null) return;

        float hour        = TimeManager.Instance.GetCurrentHour ();
        AudioClip athensClip = (hour >= 18f || hour < 6f)
            ? musicAthensNight
            : musicAthensDay;

        if (athensClip != currentMusicClip)
            PlayMusic (athensClip);
    }

    // ══════════════════════════════════════════════════════════════════════
    // SFX
    // ══════════════════════════════════════════════════════════════════════

    public void PlaySFX (AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot (clip, sfxVolume);
    }

    // Convenience methods called from other scripts
    public void PlayWorking ()              => PlaySFX (sfxWorking);
    public void PlayTalkingToNPC ()         => PlaySFX (sfxTalkingToNPC);
    public void PlayRelationshipGain ()     => PlaySFX (sfxRelationshipGain);
    public void PlayDrachmaGained ()        => PlaySFX (sfxDrachmaGained);
    public void PlayHonourGained ()         => PlaySFX (sfxHonourGained);
    public void PlayPrayerOffering ()       => PlaySFX (sfxPrayerOffering);
    public void PlayDivineFavourIncrease () => PlaySFX (sfxDivineFavourIncrease);
    public void PlayFestivalNotification () => PlaySFX (sfxFestivalNotification);
    public void PlayCareerLevelUp ()        => PlaySFX (sfxCareerLevelUp);
    public void PlayMarriage ()             => PlaySFX (sfxMarriage);
    public void PlayLegacyPanelOpens ()     => PlaySFX (sfxLegacyPanelOpens);
    public void PlayDayEndSleep ()          => PlaySFX (sfxDayEndSleep);

    // ══════════════════════════════════════════════════════════════════════
    // Scene to music mapping
    // ══════════════════════════════════════════════════════════════════════

    private AudioClip GetMusicForScene (string sceneName)
    {
        switch (sceneName)
        {
            case "Loading":
            case "MainMenu":
            case "CharacterCreation": return musicMainTheme;
            case "Athens":            return musicAthensDay; // Update() handles day/night switch
            case "HomeInterior":      return musicHome;
            case "AgoraInterior":     return musicAgora;
            case "GymnasiumInterior": return musicGymnasium;
            case "AcropolisInterior": return musicAcropolis;
            case "HarbourInterior":   return musicHarbour;
            case "KerameikosInterior":return musicKerameikos;
            case "TheatreInterior":   return musicTheatre;
            case "EndOfDay":          return musicEndOfDay;
            default:                  return null;
        }
    }
}