using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpriteTexture : MonoBehaviour
{
    public Texture2D internalTexture;
    public Sprite internalSprite;
    public bool Animated;
    public int Frame;

    private Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private float lifeTime;
    public float AnimationFPS = 10.0f;

    void Start()
    {
        CheckRenderer();
        Sprite sprite = internalSprite;
        if (internalTexture != null)
        {
            sprites = Resources.LoadAll<Sprite>(internalTexture.name);
            sprite = sprites[Frame];
        }
        spriteRenderer.sprite = sprite;
    }

    void CheckRenderer()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        CheckRenderer();
        if (!Animated || sprites == null)
        {
            spriteRenderer.sprite = sprites[Frame];
            return;
        }

        lifeTime += Time.deltaTime;
        var frame = (int)Mathf.Floor(lifeTime * AnimationFPS) % sprites.Length;
        spriteRenderer.sprite = sprites[frame];
    }
}
