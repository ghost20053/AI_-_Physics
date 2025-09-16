using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BulletUISlot : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;          // Bullet icon
    public TextMeshProUGUI ammoText; // "30 / 90"
    public Image highlight;          // Outline/highlight

    private BulletType currentBullet;

    public void Setup(BulletType bullet)
    {
        currentBullet = bullet;

        if (iconImage != null && bullet.icon != null)
            iconImage.sprite = bullet.icon;

        UpdateSlot(bullet, false);
    }

    public void UpdateSlot(BulletType bullet, bool isActive)
    {
        if (bullet == null) return;

        // Update ammo UI
        if (ammoText != null)
            ammoText.text = $"{bullet.bulletsLeft} / {bullet.reserveAmmo}";

        // Highlight if active
        if (highlight != null)
            highlight.enabled = isActive;
    }
}
