using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BulletUISlot : MonoBehaviour
{
    [Header("UI")]
    public Image icon;                // Icon of this bullet type
    public TextMeshProUGUI ammoText;  // Shows ammo
    public Image highlight;           // Active weapon highlight

    // Setup slot once at start
    public void Setup(BulletType bullet)
    {
        if (icon != null && bullet.icon != null)
            icon.sprite = bullet.icon;

        UpdateSlot(bullet, false);
    }

    // Called whenever ammo or selection changes
    public void UpdateSlot(BulletType bullet, bool isActive)
    {
        if (ammoText != null)
            ammoText.text = $"{bullet.bulletsLeft} / {bullet.reserveAmmo}";

        if (highlight != null)
            highlight.enabled = isActive;
    }
}
