using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
public class PlayerController : MonoBehaviour
{
    [Header("Skeleton Prefabs")]
    public GameObject swordPrefab;
    public GameObject axPrefab;
    public GameObject rangerPrefab;
    private GameObject activePrefab;

    public GameObject gravePrefab;
    public Church church;             // Zieh hier deine Kirche rein
    private int activeIndex = 0;

    private UIManager uiManager;

    public Tilemap spawnableTilemap;
    public float spawnRadius = 5f;    // Wie weit weg von der Kirche darf man spawnen?

    public int skeletonCost = 3;
    public int maxSkeletons = 15;

    public int graveCost = 10;
    public int maxGrave = 5;

    public static int currentSkeletonCount = 0; // Hält das globale Limit im Auge
    public static int currentGraveCount = 0;

    [Header("Mutationen Einstellungen")]
    public string activeMutationType = ""; // "Fire", "Lightning", "Ice", "Earth" oder ""
    public int mutationCost = 5;

    private void Start()
    {
        currentSkeletonCount = 0;
        currentGraveCount = 0;

        activePrefab = swordPrefab;
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateHotbarUI(0);     // Setzt den Slot 1 als "aktiv"
            uiManager.UpdateInfoPanel(0);    // Lädt sofort die Info für das Sword Skeleton
        }
    }

    void Update()
    {
        SwitchHotbar();

        if (Input.GetMouseButtonDown(0))
        {
            if (activePrefab == swordPrefab || activePrefab == axPrefab || activePrefab == rangerPrefab)
            {
                TrySpawnSkeleton();
            }
            else if (activePrefab == gravePrefab)
            {
                TrySpawnGrave();
            }
            else
            {
                TryApplyMutation();
            }
        }
    }

    void TrySpawnSkeleton()
    {
        // 1. Haben wir noch Platz im Limit?
        if (currentSkeletonCount >= maxSkeletons)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.cancelSound);
            Debug.Log("Maximales Skelett-Limit erreicht! (15/15)");
            return;
        }

        // 2. Haben wir genug Souls?
        if (church.souls < skeletonCost)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.cancelSound);
            Debug.Log("Nicht genug Seelen! Du brauchst " + skeletonCost);
            return;
        }

        // 3. Klick-Position im Weltraum berechnen
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // 2D-Sicherheit

        // 4. Prüfen, ob der Klick nah genug an der Kirche war
        float distanceToChurch = Vector2.Distance(mouseWorldPos, church.transform.position);
        Vector3Int cellPosition = spawnableTilemap.WorldToCell(mouseWorldPos);

        if (distanceToChurch >= spawnRadius || !spawnableTilemap.HasTile(cellPosition))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.cancelSound);
            Debug.Log("Zu weit weg von der Kirche geklickt!");
        }
        else
        {
            // Erfolgreich! Souls abziehen, Counter hoch und spawnen
            church.souls -= skeletonCost;
            currentSkeletonCount++;

            Instantiate(activePrefab, mouseWorldPos, Quaternion.identity);

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayRandomSpawnSound();
            }

            Debug.Log("Skelett erwacht! Verbleibende Seelen: " + church.souls);
        }
    }

    void TrySpawnGrave()
    {
        if (currentGraveCount >= maxGrave)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.cancelSound);
            Debug.Log("Maximales Grave-Limit erreicht! (5/5)");
            return;
        }

        if (church.souls < graveCost)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.cancelSound);
            Debug.Log("Nicht genug Seelen! Du brauchst " + graveCost);
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f; // 2D-Sicherheit

        // 4. Prüfen, ob der Klick nah genug an der Kirche war
        float distanceToChurch = Vector2.Distance(mouseWorldPos, church.transform.position);
        Vector3Int cellPosition = spawnableTilemap.WorldToCell(mouseWorldPos);

        if (distanceToChurch >= spawnRadius || !spawnableTilemap.HasTile(cellPosition))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.cancelSound);
            Debug.Log("Zu weit weg von der Kirche geklickt!");
        }
        else
        {
            // Erfolgreich! Souls abziehen, Counter hoch und spawnen
            church.souls -= graveCost;
            currentGraveCount++;

            Instantiate(gravePrefab, mouseWorldPos, Quaternion.identity);
            Debug.Log("Grave erbaut! Verbleibende Seelen: " + church.souls);
        }
    }

    void TryApplyMutation()
    {
        if (string.IsNullOrEmpty(activeMutationType)) return;

        if (church.souls < mutationCost)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.cancelSound);
            Debug.Log("Nicht genug Seelen für eine Mutation! Du brauchst " + mutationCost);
            return;
        }

        // 3. Mausposition im Weltraum berechnen
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // 4. Per Raycast prüfen, ob ein Objekt unter der Maus liegt
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null)
        {
            // Prüfen, ob das getroffene Objekt ein Skelett ist
            if (hit.collider.CompareTag("Skeleton"))
            {
                SkeletonBase skeleton = hit.collider.GetComponent<SkeletonBase>();

                if (skeleton != null)
                {
                    // Wichtig: Prüfen, ob das Skelett diese Mutation vielleicht schon hat!
                    if (skeleton.currentElement == activeMutationType)
                    {
                        AudioManager.instance.PlaySound(AudioManager.instance.cancelSound);
                        Debug.Log("Dieses Skelett hat bereits die " + activeMutationType + "-Milch!");
                        return;
                    }

                    // Erfolg! Seelen abziehen und Mutation auf dem Skelett aktivieren
                    church.souls -= mutationCost;
                    skeleton.ApplyElementMutation(activeMutationType);

                    Debug.Log("Skelett erfolgreich mutiert zu: " + activeMutationType);
                }
            }
        }
    }

    void SwitchHotbar()
    {

        int previousIndex = activeIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1)) { activePrefab = swordPrefab; activeMutationType = ""; activeIndex = 0; skeletonCost = 2; }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { activePrefab = axPrefab; activeMutationType = ""; activeIndex = 1; skeletonCost = 4; }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { activePrefab = rangerPrefab; activeMutationType = ""; activeIndex = 2; skeletonCost = 7; }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { activePrefab = gravePrefab; activeMutationType = ""; activeIndex = 3;}
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { activePrefab = null; activeMutationType = "Fire"; activeIndex = 4; }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { activePrefab = null; activeMutationType = "Lightning"; activeIndex = 5; }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) { activePrefab = null; activeMutationType = "Ice"; activeIndex = 6; }
        else if (Input.GetKeyDown(KeyCode.Alpha8)) { activePrefab = null; activeMutationType = "Earth"; activeIndex = 7; }
        if (Input.anyKeyDown)
        {
            if (uiManager != null)
            {
                uiManager.UpdateHotbarUI(activeIndex);
            }
        }
        if (previousIndex != activeIndex)
        {
            if (uiManager != null) // <--- CRITICAL SAFETY CHECK
            {
                uiManager.UpdateHotbarUI(activeIndex);
                uiManager.UpdateInfoPanel(activeIndex);
            }
            else
            {
                // Try to find it again if it was lost
                uiManager = FindFirstObjectByType<UIManager>();
            }
        }
    }
}