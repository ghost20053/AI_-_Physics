using UnityEngine;

public class AmmoCache : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); //Destroy self

        }
    }
}
