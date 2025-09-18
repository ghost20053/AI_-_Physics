using System.Collections.Generic;
using UnityEngine;

public class PowerUpUIManager : MonoBehaviour
{
    public static PowerUpUIManager Instance { get; private set; }

    [Header("UI References")]
    public Transform powerUpContainer;    // Parent object for slots
    public GameObject powerUpSlotPrefab;  // Prefab with PowerUpUISlot script

    private Dictionary<PowerUpType, PowerUpUISlot> activeSlots = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddPowerUp(PowerUpType type, Sprite icon, float duration)
    {
        if (activeSlots.ContainsKey(type))
        {
            // Reset existing slot duration
            activeSlots[type].SetDuration(duration);
            return;
        }

        GameObject slotObj = Instantiate(powerUpSlotPrefab, powerUpContainer);
        PowerUpUISlot slot = slotObj.GetComponent<PowerUpUISlot>();
        slot.Setup(type, icon, duration);

        activeSlots.Add(type, slot);
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
        if (powerUpContainer != null)
            powerUpContainer.gameObject.SetActive(visible);
    }
}
