using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Erlaubt Zugriff von überall: AudioManager.instance.PlaySound(...)

    public AudioSource audioSource;
    public AudioClip[] spawnSounds;
    public AudioClip[] mutationSound;
    public AudioClip[] attackSound;
    public AudioClip[] arrowSound;
    public AudioClip[] monsterSound;
    public AudioClip gameOver;
    public AudioClip cancelSound;
    public AudioClip gameWon;
    void Awake()
    {
        instance = this;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayRandomSpawnSound()
    {
        if (spawnSounds.Length > 0)
        {
            // Wählt eine zufällige Zahl zwischen 0 und der Anzahl der Sounds
            int randomIndex = Random.Range(0, spawnSounds.Length);
            audioSource.PlayOneShot(spawnSounds[randomIndex]);
        }
    }

    public void PlayRandomAttackSound()
    {
        if (spawnSounds.Length > 0)
        {
            // Wählt eine zufällige Zahl zwischen 0 und der Anzahl der Sounds
            int randomIndex = Random.Range(0, attackSound.Length);
            audioSource.PlayOneShot(attackSound[randomIndex]);
        }
    }

    public void PlayRandomArrowSound()
    {
        if (spawnSounds.Length > 0)
        {
            // Wählt eine zufällige Zahl zwischen 0 und der Anzahl der Sounds
            int randomIndex = Random.Range(0, arrowSound.Length);
            audioSource.PlayOneShot(arrowSound[randomIndex]);
        }
    }

    public void PlayRandomMutationsSound()
    {
        if (spawnSounds.Length > 0)
        {
            // Wählt eine zufällige Zahl zwischen 0 und der Anzahl der Sounds
            int randomIndex = Random.Range(0, mutationSound.Length);
            audioSource.PlayOneShot(mutationSound[randomIndex]);
        }
    }
    public void PlayRandomMonsterSound()
    {
        if (spawnSounds.Length > 0)
        {
            // Wählt eine zufällige Zahl zwischen 0 und der Anzahl der Sounds
            int randomIndex = Random.Range(0, monsterSound.Length);
            audioSource.PlayOneShot(monsterSound[randomIndex]);
        }
    }
}
