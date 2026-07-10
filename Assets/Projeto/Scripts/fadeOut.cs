using UnityEngine;
using System.Collections;

public class fadeOut : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float fadeSpeed = 1f;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    public IEnumerator FadeOut()
{
    while (spriteRenderer.color.a > 0f)
    {
        float alpha = Mathf.MoveTowards(spriteRenderer.color.a, 0f, fadeSpeed * Time.deltaTime);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        yield return null;
    }
}
}
