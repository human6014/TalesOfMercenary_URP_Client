using UnityEngine;

public class Rotator : MonoBehaviour {
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update() {
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}
