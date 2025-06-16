using UnityEngine;

public class Despawner : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            GameManager.instance.CarDespawned();
            Destroy(other.gameObject);
        }
    }
}