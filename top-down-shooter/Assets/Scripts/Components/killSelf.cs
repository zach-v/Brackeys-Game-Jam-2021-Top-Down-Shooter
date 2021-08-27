using System.Collections;
using UnityEngine;

public class killSelf : MonoBehaviour {
    public float timeTillDeath = 0.5f;

    void Update() {
        StartCoroutine("DoKill");
    }
    IEnumerator DoKill()
	{
        yield return new WaitForSeconds(timeTillDeath);
        Destroy(gameObject);
    }
}
