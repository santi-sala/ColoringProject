using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class SpriteBrush : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer _currentSpriteRenderer;

    [Header("Settings")]
    [SerializeField] private Color _brushColor;
    [SerializeField] private int _brushSize;
    private Dictionary<int, Texture2D> _originalTextures = new Dictionary<int, Texture2D>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastMultipleSprites();
        }
        else if (Input.GetMouseButton(0))
        {
            RaycastCurrentSprite();
        }
    }

    private void RaycastCurrentSprite()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = Vector2.zero;
        RaycastHit2D[] rayHits = Physics2D.RaycastAll(origin, direction);

        if (rayHits.Length == 0)
        {
            return;
        }

        for (int i = 0; i < rayHits.Length; i++)
        {
            if (!rayHits[i].collider.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                continue;
            }
            
            if (spriteRenderer == _currentSpriteRenderer)
            {
                ColorSpriteAtPosition(rayHits[i].collider, rayHits[i].point);
                break;
            }
        }
    }

    private void RaycastMultipleSprites()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = Vector2.zero;
        RaycastHit2D[] rayHits = Physics2D.RaycastAll(origin, direction);

        if (rayHits.Length == 0)
        {
            return;
        }

        int highestOrderInlayer = -1000;
        int topIndex = -1;

        // Finding the top sprite
        for (int i = 0; i < rayHits.Length; i++)
        {
            SpriteRenderer spriteRenderer = rayHits[i].collider.GetComponent<SpriteRenderer>();

            if (spriteRenderer == null)
            {
                continue;
            }

            int orderInLayer = spriteRenderer.sortingOrder;

            if (orderInLayer > highestOrderInlayer)
            {
                highestOrderInlayer = orderInLayer;
                topIndex = i;
            }
        }

        
        Collider2D topCollider = rayHits[topIndex].collider;

        // Setting the current sprite renderer to the top sprite
        _currentSpriteRenderer = topCollider.GetComponent<SpriteRenderer>();

        // Checking if we have the current original texture in the dictionary, if not adding it.
        int spriteIndexInHierarchy = _currentSpriteRenderer.transform.GetSiblingIndex();
        if(!_originalTextures.ContainsKey(spriteIndexInHierarchy))
        {             
            _originalTextures.Add(spriteIndexInHierarchy, _currentSpriteRenderer.sprite.texture);
        }

        ColorSpriteAtPosition(topCollider, rayHits[topIndex].point);

    }

    private void ColorSpriteAtPosition(Collider2D collider, Vector2 hitPoint)
    {
        SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            return;
        }

        // Converting the hit point (World space) to a texture point (Local space)   
        Vector2 texturePoint = WorldToTexturePoint(spriteRenderer, hitPoint);

        // Getting the sprite copy
        Sprite spriteCopy = spriteRenderer.sprite;

        int currentSpriteIndex = spriteRenderer.transform.GetSiblingIndex();
        Texture2D originalTexture = _originalTextures[currentSpriteIndex];

        // Creating a new texture to modify
        Texture2D newTexture = new Texture2D(spriteCopy.texture.width, spriteCopy.texture.height);

        Graphics.CopyTexture(spriteCopy.texture, newTexture);

        /*
        // Painting in square shape
        for (int i = -_brushSize / 2; i < _brushSize / 2; i++)
        {
            for (int j = -_brushSize / 2; j < _brushSize / 2; j++)
            {
                int pixelX = i + (int)texturePoint.x;
                int pixelY = j + (int)texturePoint.y;

                Color pixelColor = _brushColor;

                // Taking the alpha value from the original texture
                pixelColor.a = spriteCopy.texture.GetPixel(pixelX, pixelY).a;

                // Multiplying the color values to leave the black outline 
                pixelColor = pixelColor * originalTexture.GetPixel(pixelX, pixelY);

                newTexture.SetPixel(pixelX, pixelY, pixelColor);
            }
        }
        */

        /*
        // Painting in circle shape
        for (int i = -_brushSize / 2; i < _brushSize / 2; i++)
        {
            for (int j = -_brushSize / 2; j < _brushSize / 2; j++)
            {
                int pixelX = i + (int)texturePoint.x;
                int pixelY = j + (int)texturePoint.y;

                Vector2 pixelPoint = new Vector2(pixelX, pixelY);

                if (Vector2.Distance(pixelPoint, texturePoint) > _brushSize / 2)
                {
                    continue;
                }

                Color pixelColor = _brushColor;

                // Taking the alpha value from the original texture
                pixelColor.a = spriteCopy.texture.GetPixel(pixelX, pixelY).a;

                // Multiplying the color values to leave the black outline 
                pixelColor = pixelColor * originalTexture.GetPixel(pixelX, pixelY);

                newTexture.SetPixel(pixelX, pixelY, pixelColor);
            }
        }
        */

        // Painting in circle shape with smooth edges
        for (int i = -_brushSize / 2; i < _brushSize / 2; i++)
        {
            for (int j = -_brushSize / 2; j < _brushSize / 2; j++)
            {
                int pixelX = i + (int)texturePoint.x;
                int pixelY = j + (int)texturePoint.y;

                Vector2 pixelPoint = new Vector2(pixelX, pixelY);

                float squaredRadius = i * i + j * j;
                float factor = Mathf.Exp(-squaredRadius / _brushSize);

                Color previousColor = spriteCopy.texture.GetPixel(pixelX, pixelY);
                Color pixelColor = Color.Lerp(previousColor, _brushColor, factor);

                // Taking the alpha value from the original texture
                pixelColor.a = previousColor.a;

                // Multiplying the color values to leave the black outline 
                pixelColor *= originalTexture.GetPixel(pixelX, pixelY);

                newTexture.SetPixel(pixelX, pixelY, pixelColor);
            }
        }



        newTexture.Apply();

        // Creating a new sprite with the new texture (Basically a copy)
        Sprite newSprite = Sprite.Create(newTexture, spriteCopy.rect, Vector2.one / 2, spriteCopy.pixelsPerUnit);

        spriteRenderer.sprite = newSprite;
    }

    private Vector2 WorldToTexturePoint(SpriteRenderer spriteRenderer, Vector2 worldPosition)
    {        

        Vector2 texturePoint = spriteRenderer.transform.InverseTransformPoint(worldPosition);

        // Position between -.5 and .5
        texturePoint.x /= spriteRenderer.bounds.size.x;
        texturePoint.y /= spriteRenderer.bounds.size.y;

        // Position between 0 and 1
        texturePoint += Vector2.one / 2;

        // Position between 0 and texture size (Offset in texture space)
        texturePoint.x *= spriteRenderer.sprite.rect.width;
        texturePoint.y *= spriteRenderer.sprite.rect.height;

        // Position in texture space
        texturePoint.x += spriteRenderer.sprite.rect.x;
        texturePoint.y += spriteRenderer.sprite.rect.y;

        return texturePoint;
    }

    /*
    private void ColorSprite(Collider2D collider)
    {
        SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            return;
        }

        //Texture2D texture = spriteRenderer.sprite.texture;
        Sprite spriteCopy = spriteRenderer.sprite;
        Texture2D newTexture = new Texture2D(spriteCopy.texture.width, spriteCopy.texture.height);

        for (int i = 0; i < newTexture.width; i++)
        {
            for (int j = 0; j < newTexture.height; j++)
            {
                Color pixelColor = _brushColor;

                // Taking the alpha value from the original texture
                pixelColor.a = spriteCopy.texture.GetPixel(i, j).a;

                // Multiplying the color values to leave the black outline 
                pixelColor = pixelColor * spriteCopy.texture.GetPixel(i, j);

                newTexture.SetPixel(i, j, pixelColor);
            }
        }

        newTexture.Apply();

        // Creating a new sprite with the new texture (Basically a copy)
        Sprite newSprite = Sprite.Create(newTexture, spriteCopy.rect, Vector2.one / 2, spriteCopy.pixelsPerUnit);

        spriteRenderer.sprite = newSprite;
    }
    */

}
