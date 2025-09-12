using UnityEngine;

public class TempInfiniteAmmo : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); //Destroy self
        }
    }
}
