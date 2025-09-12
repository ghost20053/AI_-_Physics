using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BulletUISlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI ammoText;
    public Image highlight;

    public void Setup(BulletType bullet)
    {
        if (icon != null && bullet.icon != null)
            icon.sprite = bullet.icon;

        UpdateSlot(bullet, false);
    }

    public void UpdateSlot(BulletType bullet, bool isActive)
    {
        if (ammoText != null)
            ammoText.text = $"{bullet.bulletsLeft} / {bullet.reserveAmmo}";

        if (highlight != null)
            highlight.enabled = isActive;
    }
}
