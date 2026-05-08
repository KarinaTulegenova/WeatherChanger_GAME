using UnityEngine;

public class SimpleWind : MonoBehaviour
{
    public float speed = 1f;
    public float strength = 2f;

    public bool isActive = false;

    private Vector3 startRotation;

    void Start()
    {
        startRotation = transform.eulerAngles;
    }

    void Update()
    {
        if (!isActive)
        {
            transform.eulerAngles = startRotation;
            return;
        }

        float sway = Mathf.Sin(Time.time * speed) * strength;

        transform.eulerAngles = new Vector3(
            startRotation.x,
            startRotation.y,
            startRotation.z + sway
        );
    }
}