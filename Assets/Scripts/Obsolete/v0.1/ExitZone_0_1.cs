using UnityEngine;
using System.Collections;


public class ExitZone : MonoBehaviour
{

  private IEnumerator DestroyNextFrame(GameObject obj)
  {
    yield return null;
    Destroy(obj);
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Car")) {
      GameManager.instance.CarExited();
      StartCoroutine(DestroyNextFrame(other.gameObject));
    }
  }
}
