using UnityEngine;
[System.Serializable]
public struct SoundClip
{
    public string soundName;//codename
    public AudioClip clip;
}
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundClip[] avaibleSounds;
    private void OnEnable()
    {
        EventManager.OnPlaySound += PlaySound;
    }
    private void OnDisable()
    {
        EventManager.OnPlaySound -= PlaySound;
    }
    private void PlaySound(string soundName)
    {
        //we find the audio clip
        foreach (var  sound in avaibleSounds)
        {
            if (sound.soundName == soundName)
            {
                audioSource.PlayOneShot(sound.clip);
                return;
            }
        }
        Debug.LogWarning($"[AudioManager] Sound not found {soundName}");
    }
}
