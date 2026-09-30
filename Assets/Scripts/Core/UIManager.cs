using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Das Singleton: Macht diesen Manager von überall aufrufbar via UIManager.Instance
    public static UIManager Instance { get; private set; }

    [Header("Text Zuweisungen")]
    public TMP_Text timerText;
    public TMP_Text churchHpText;
    public TMP_Text soulsText;
    public TMP_Text skeletonsText;
    public TMP_Text gravesText;

    [Header("Hotbar Einstellungen")]
    public Image[] hotbarSlotImages;
    private Color activeColor = new Color(1f, 1f, 1f, 1f);       // #FFFFFF (Weiß)
    private Color inactiveColor = new Color(0.647f, 0.647f, 0.647f, 1f); // #A5A5A5 (Grau)

    [Header("Info-Panel Settings")]
    public TMP_Text nameText;
    public TMP_Text descText;
    public TMP_Text statsText;

    [Header("Timer Einstellungen")]
    public float matchDurationInSeconds = 180f; // 3 Minuten für den Jam
    private float currentTime;
    private bool isGameActive = true;

    public UnitData[] slotInfoData = new UnitData[8];

    [SerializeField] private GameObject winScreenPanel;
    private Church church;
    private PlayerController playerController;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        // Findet automatisch die wichtigen Skripte in der aktuellen Szene
        currentTime = matchDurationInSeconds;
        church = FindFirstObjectByType<Church>();
        playerController = FindFirstObjectByType<PlayerController>();
        if (winScreenPanel != null) winScreenPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameActive)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                UpdateTimerDisplay(currentTime);
            }
            else
            {
                currentTime = 0;
                UpdateTimerDisplay(currentTime);
                WinGame();
            }
        }
        // Sicherheitshalber prüfen, ob die Kirche noch lebt
        if (church != null)
        {
            churchHpText.text = "Church HP: " + church.currentHp + " / " + church.maxHp;
            soulsText.text = "Souls: " + church.souls;
        }
        else
        {
            churchHpText.text = "GAME OVER";
        }

        // Skelett- und Friedhof-Zähler abfragen
        if (playerController != null)
        {
            skeletonsText.text = "Skeletons: " + PlayerController.currentSkeletonCount + " / " + playerController.maxSkeletons;
            gravesText.text = "Graves: " + PlayerController.currentGraveCount + " / " + playerController.maxGrave;
        }
    }
    void UpdateTimerDisplay(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void WinGame()
    {
        isGameActive = false;
        if (winScreenPanel != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.gameWon);
            winScreenPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void UpdateInfoPanel(int index)
    {
        if (index >= 0 && index < slotInfoData.Length && slotInfoData[index] != null)
        {
            nameText.text = slotInfoData[index].unitName;
            descText.text = slotInfoData[index].description;
            statsText.text = slotInfoData[index].stats;
        }
    }

    public void UpdateHotbarUI(int activeIndex)
    {
        for (int i = 0; i < hotbarSlotImages.Length; i++)
        {
            if (hotbarSlotImages[i] != null)
            {
                hotbarSlotImages[i].color = (i == activeIndex) ? activeColor : inactiveColor;
            }
        }
    }
}