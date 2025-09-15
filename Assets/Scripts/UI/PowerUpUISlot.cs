using UnityEngine;
using UnityEngine.UI;

public class PowerUpUISlot : MonoBehaviour
{
    public Image iconImage;
    public Slider timerSlider;

    private PowerUpType type;
    private float duration;
    private float timer;

    public void Setup(PowerUpType type, Sprite icon, float duration)
    {
        this.type = type;
        this.duration = duration;
        this.timer = duration;

        if (iconImage != null && icon != null)
            iconImage.sprite = icon;

        if (timerSlider != null)
        {
            timerSlider.maxValue = duration;
            timerSlider.value = duration;
        }
    }

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
        timer = newDuration;
        if (timerSlider != null)
        {
            timerSlider.maxValue = duration;
            timerSlider.value = duration;
        }
    }

    private void Update()
    {
        if (duration <= 0) return;

        timer -= Time.deltaTime;
        if (timerSlider != null)
        {
            timerSlider.value = timer;
        }
        if (timer <= 0)
        {
            PowerUpUIManager.Instance.RemovePowerUp(type);
        }
    }
}
