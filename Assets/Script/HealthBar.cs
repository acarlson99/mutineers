using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = -1f;
    public float maxBarWidth = 0f;
    public float minBarWidth = 0f;

    // Start is called before the first frame update
    void Start()
    {
        maxBarWidth = transform.localScale.x;
        if (currentHealth < 0) currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Mathf.Lerp(minBarWidth, maxBarWidth, currentHealth / maxHealth),
            transform.localScale.y,
            transform.localScale.z);
    }
}
