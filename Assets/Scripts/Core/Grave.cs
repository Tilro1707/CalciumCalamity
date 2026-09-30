using UnityEngine;

public class Grave : MonoBehaviour
{
    public float productionCooldown = 2f;
    private float timer;
    public Church church;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        church = FindAnyObjectByType<Church>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        soulProduction();
    }

    public void soulProduction()
    {
        if(productionCooldown <= timer)
        {
            church.souls += 1;
            timer = 0;
        }

    }
}
