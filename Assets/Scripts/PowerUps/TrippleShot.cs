using TMPro;
using UnityEngine;

public class TrippleShot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CountDownPowerUpText;

    [SerializeField] float remainingTime;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(gameObject); //Destroy self
        }
    }
}
