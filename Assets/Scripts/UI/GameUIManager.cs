using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Top Bar UI")]
    public TextMeshProUGUI enemyCounterText;
    public Timer timer;  // timer script now lives in same TopBar panel

    [Header("Bullet Tracking")]
    public Transform bulletUIContainer;
    public GameObject bulletSlotPrefab;
    private BulletUISlot[] bulletSlots;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ---------------- Bullet UI ----------------
    public void SetupBulletUI(BulletType[] bulletTypes)
    {
        foreach (Transform child in bulletUIContainer)
        {
            Destroy(child.gameObject);
        }
        bulletSlots = new BulletUISlot[bulletTypes.Length];

        for (int i = 0; i < bulletTypes.Length; i++)
        {
            GameObject slotObj = Instantiate(bulletSlotPrefab, bulletUIContainer);
            BulletUISlot slot = slotObj.GetComponent<BulletUISlot>();
            slot.Setup(bulletTypes[i]);
            bulletSlots[i] = slot;
        }
    }

    public void UpdateBulletUI(BulletType bullet, int index, bool isActive)
    {
        if (bulletSlots != null && index >= 0 && index < bulletSlots.Length)
        {
            bulletSlots[index].UpdateSlot(bullet, isActive);
        }
    }

    // ---------------- Enemy Counter ----------------
    public void UpdateEnemyCounter(int remaining)
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = $"Enemies Left: {remaining}";
        }
    }

    // ---------------- Pause / Resume ----------------
    public void ToggleUI(bool visible)
    {
        if (bulletUIContainer != null)
        {
            bulletUIContainer.gameObject.SetActive(visible);
        }
        if (enemyCounterText != null)
        {
            enemyCounterText.gameObject.SetActive(visible);
        }
        if (timer != null)
        {
            timer.ToggleUI(visible);
        }
        if (PowerUpUIManager.Instance != null)
        {
            PowerUpUIManager.Instance.ToggleUI(visible);
        }
    }
}
