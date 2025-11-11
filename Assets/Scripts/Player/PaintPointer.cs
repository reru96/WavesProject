using System.Collections;
using UnityEngine;

public class PaintPointer : MonoBehaviour
{
    public Material cylinderMaterial;
    public RenderTexture paintTexture;
    public Texture2D brushTexture;
    public float brushSize = 0.1f;
    public float paintDuration = 2f;

    private Color playerColor => GetComponent<SpriteRenderer>().color;

    void Update()
    {
        Ray ray = new Ray(transform.position, new Vector3(0,0,50)); 
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("BackGroundCanvas"))
            {
                Vector2 uv = hit.textureCoord;
                StartCoroutine(PaintAtUV(uv, playerColor));
            }
        }
    }

    IEnumerator PaintAtUV(Vector2 uv, Color color)
    {

        RenderTexture.active = paintTexture;
        Texture2D tempTex = new Texture2D(paintTexture.width, paintTexture.height, TextureFormat.RGBA32, false);
        tempTex.ReadPixels(new Rect(0, 0, paintTexture.width, paintTexture.height), 0, 0);
        tempTex.Apply();

        int x = (int)(uv.x * paintTexture.width);
        int y = (int)(uv.y * paintTexture.height);

        for (int i = 0; i < brushTexture.width; i++)
        {
            for (int j = 0; j < brushTexture.height; j++)
            {
                Color brushColor = brushTexture.GetPixel(i, j);
                if (brushColor.a > 0.1f)
                {
                    int px = x + i - brushTexture.width / 2;
                    int py = y + j - brushTexture.height / 2;
                    if (px >= 0 && px < tempTex.width && py >= 0 && py < tempTex.height)
                    {
                        tempTex.SetPixel(px, py, color * brushColor);
                    }
                }
            }
        }

        tempTex.Apply();
        Graphics.Blit(tempTex, paintTexture);
        RenderTexture.active = null;

        yield return new WaitForSeconds(paintDuration);

        ClearPaintAtUV(uv);
    }

    void ClearPaintAtUV(Vector2 uv)
    {
        RenderTexture.active = paintTexture;
        Texture2D tempTex = new Texture2D(paintTexture.width, paintTexture.height, TextureFormat.RGBA32, false);
        tempTex.ReadPixels(new Rect(0, 0, paintTexture.width, paintTexture.height), 0, 0);
        tempTex.Apply();

        int x = (int)(uv.x * paintTexture.width);
        int y = (int)(uv.y * paintTexture.height);

        for (int i = 0; i < brushTexture.width; i++)
        {
            for (int j = 0; j < brushTexture.height; j++)
            {
                int px = x + i - brushTexture.width / 2;
                int py = y + j - brushTexture.height / 2;
                if (px >= 0 && px < tempTex.width && py >= 0 && py < tempTex.height)
                {
                    tempTex.SetPixel(px, py, Color.clear);
                }
            }
        }

        tempTex.Apply();
        Graphics.Blit(tempTex, paintTexture);
        RenderTexture.active = null;
    }
}
