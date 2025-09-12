using UnityEngine;
using System.Collections.Generic;

public class PowerUpUIManager : MonoBehaviour
{
    public static PowerUpUIManager Instance { get; private set; }

    public Transform powerUpUIContainer;
    public GameObject powerUpSlotPrefab;

    private Dictionary<PowerUpType, PowerUpUISlot> activeSlots = new Dictionary<PowerUpType, PowerUpUISlot>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddPowerUp(PowerUpType type, Sprite icon, float duration)
    {
        if (activeSlots.ContainsKey(type))
        {
            activeSlots[type].SetDuration(duration);
            return;
        }

        GameObject slotObj = Instantiate(powerUpSlotPrefab, powerUpUIContainer);
        PowerUpUISlot slot = slotObj.GetComponent<PowerUpUISlot>();
        slot.Setup(type, icon, duration);
        activeSlots[type] = slot;
    }

    public void RemovePowerUp(PowerUpType type)
    {
        if (activeSlots.ContainsKey(type))
        {
            Destroy(activeSlots[type].gameObject);
            activeSlots.Remove(type);
        }
    }

    public void ToggleUI(bool visible)
    {
        if (powerUpUIContainer != null)
            powerUpUIContainer.gameObject.SetActive(visible);
    }
}
