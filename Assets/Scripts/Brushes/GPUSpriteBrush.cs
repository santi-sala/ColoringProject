using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUSpriteBrush : MonoBehaviour
{
    //Singleton
    public static GPUSpriteBrush Instance;

    [Header("Elements")]
    [SerializeField] private SpriteRenderer _currentSpriteRenderer;

    [Header("Settings")]
    [SerializeField] private Color _brushColor;
    [Range(0.0001f, 0.1f)]
    [SerializeField] private float _brushSize;
    [SerializeField] private Material _brushMaterial;
    private Dictionary<int, Texture2D> _originalTextures = new Dictionary<int, Texture2D>();
    private Dictionary<int, Texture2D> _editedTextures = new Dictionary<int, Texture2D>();
    private Dictionary<int, RenderTexture> _renderTectures = new Dictionary<int, RenderTexture>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
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

        Texture2D currentTexture = _currentSpriteRenderer.sprite.texture;

        if (!_originalTextures.ContainsKey(spriteIndexInHierarchy))
        {
            _originalTextures.Add(spriteIndexInHierarchy, currentTexture);
            _editedTextures.Add(spriteIndexInHierarchy, new Texture2D(currentTexture.width, currentTexture.height));

            RenderTexture newRenderTexture = new RenderTexture(currentTexture.width, currentTexture.height, 0, RenderTextureFormat.ARGB32, 10);
            newRenderTexture.useMipMap = true;

            _renderTectures.Add(spriteIndexInHierarchy, newRenderTexture);

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
        Texture2D copyTexture = _originalTextures[currentSpriteIndex];

        // Creating a new texture to modify
        Texture2D newTexture = _editedTextures[currentSpriteIndex];

        if(spriteCopy.texture != newTexture)
        {
            Graphics.CopyTexture(spriteCopy.texture, newTexture);
        }

        //Coloring using the GPU here
        _brushMaterial.SetTexture("_MainTex", newTexture);
        _brushMaterial.SetTexture("_CopyTexture", copyTexture);
        _brushMaterial.SetColor("_Color", _brushColor);
        _brushMaterial.SetFloat("_BrushSize", _brushSize);
        _brushMaterial.SetVector("_UVPosition", texturePoint / spriteCopy.texture.width);

        RenderTexture renderTexture = _renderTectures[currentSpriteIndex];
        //renderTexture.useMipMap = true;

        Graphics.Blit(newTexture, renderTexture, _brushMaterial);

        Graphics.CopyTexture(renderTexture, newTexture);


        //newTexture.Apply();

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

    public void SetBrushSize(float brushSize)
    {
        this._brushSize = brushSize;
    }

    public void SetBrushColor(Color color)
    {
        _brushColor = color;
    }
}
