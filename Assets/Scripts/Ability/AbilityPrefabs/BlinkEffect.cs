using UnityEngine;
using UnityEngine.UI;

public class BlinkEffect : MonoBehaviour
{
    public float blinkSpeed = 3f;
    private Image img;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        if (img != null)
        {
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; // 0~1 사이값
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }
}
