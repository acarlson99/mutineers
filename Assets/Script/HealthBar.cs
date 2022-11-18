using Google.Protobuf.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;

    public float maxHealth = 100f;
    public float currentHealth = -1f;
    public float maxBarWidth = 0f;
    public float minBarWidth = 0f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        maxBarWidth = transform.localScale.x;
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform)
        {
            maxBarWidth = rectTransform.sizeDelta.x;
        }
        if (currentHealth < 0) currentHealth = maxHealth;
        if (bar)
        {
            bar.type = Image.Type.Filled;
            bar.fillMethod = Image.FillMethod.Horizontal;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (maxHealth <= 0) return;

        if (bar)
        {
            bar.fillAmount = currentHealth / maxHealth;
        }
        else
        {
            var w = Mathf.Lerp(minBarWidth, maxBarWidth, currentHealth / maxHealth);
            transform.localScale = new Vector3(w, transform.localScale.y, transform.localScale.z);
        }
    }
}
