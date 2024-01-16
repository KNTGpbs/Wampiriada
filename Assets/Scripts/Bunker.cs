using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

public class Bunker : MonoBehaviour
{
    public Texture2D splat;
    public Texture2D original { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public new BoxCollider2D collision { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        collision = GetComponent<BoxCollider2D> ();
        original = spriteRenderer.sprite.texture;

        ResetBunker();
    }

    public void ResetBunker()
    {
        CopyTexture(original);
        gameObject.SetActive(true);
    }

    private void CopyTexture(Texture2D source)
    {
        Texture2D copy = new Texture2D(source.width, source.height, source.format, false);

        copy.filterMode = source.filterMode;
        copy.anisoLevel = source.anisoLevel;
        copy.wrapMode = source.wrapMode;
        copy.SetPixels(source.GetPixels());
        copy.Apply();

        Sprite sprite = Sprite.Create(copy, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), spriteRenderer.sprite.pixelsPerUnit);
        spriteRenderer.sprite = sprite;
    }

    public bool CheckColl(BoxCollider2D other, Vector3 hp)
    {
        Vector2 offset = other.size / 2;
        return Splat(hp) || Splat(hp + (Vector3.down * offset.y)) || Splat(hp + (Vector3.up * offset.y)) || Splat(hp + (Vector3.left * offset.x)) || Splat(hp + (Vector3.right * offset.x));
    }

    private bool Splat(Vector3 hp)
    {
        int py;
        int px;

        if (CheckPoint(hp, out px, out py))
        {
            return false;
        }

        Texture2D texture = spriteRenderer.sprite.texture;

        px -= texture.width/2;
        py -= texture.height/2;

        int startX = px;

        for (int i = 0 ; i < splat.height; i++) 
        {
            px = startX;
            for (int j = 0 ; j < splat.width; j++)
            {
                Color pixel = texture.GetPixel(px, py);
                pixel.a *= splat.GetPixel(j, i).a;
                texture.SetPixel(px, py, pixel);
                px++;
            }
            py++;
        }
        texture.Apply();
        return true;
    }

    private bool CheckPoint(Vector3 hp, out int px, out int py)
    {
        Vector3 localPt = transform.InverseTransformPoint(hp);

        localPt.x += collision.size.x / 2;
        localPt.y += collision.size.y / 2;

        Texture2D texture = spriteRenderer.sprite.texture;

        px = (int)((localPt.x / collision.size.x) * texture.width);
        py = (int)((localPt.y / collision.size.y) * texture.height);

        return texture.GetPixel(px, py).a != 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            this.gameObject.SetActive(false);
        }
    }
}
