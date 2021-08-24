using UnityEngine;

public class killSelf : MonoBehaviour {
    public float timeTillDeath = 0.5f;
    float time = 0;

    void Update() {
        time += Time.deltaTime;
        if (time >= timeTillDeath)
            Destroy(gameObject);
    }
}
