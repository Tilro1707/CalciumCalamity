using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Church : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp;
    public int souls = 10;
    
    void Start()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.gameOver);
            GameManager.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}
