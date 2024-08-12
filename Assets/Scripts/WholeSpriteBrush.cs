using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WholeSpriteBrush : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color _brushColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastMultipleSprites();
        }
    }

    private void RaycastSprite()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = Vector2.zero;
        RaycastHit2D rayHit = Physics2D.Raycast(origin, direction);

        if(rayHit.collider == null)
        {
            return;
        }

        ColorSprite(rayHit.collider);
        Debug.Log(rayHit.collider.name);
    }

    private void RaycastMultipleSprites()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = Vector2.zero;
        RaycastHit2D[] rayHits = Physics2D.RaycastAll(origin, direction);

        if(rayHits.Length == 0)
        {
            return;
        }

        int highestOrderInlayer = -1000;
        int topIndex = -1;

        for(int i = 0; i < rayHits.Length; i++)
        {
            SpriteRenderer spriteRenderer = rayHits[i].collider.GetComponent<SpriteRenderer>();

            if(spriteRenderer == null)
            {
                continue;
            }

            int orderInLayer = spriteRenderer.sortingOrder;

            if(orderInLayer > highestOrderInlayer)
            {
                highestOrderInlayer = orderInLayer;
                topIndex = i;
            }
        }

        Collider2D topCollider = rayHits[topIndex].collider;
        ColorSprite(topCollider);


    }

    private void ColorSprite(Collider2D collider)
    {
        SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();

        if(spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = _brushColor;
    }
}
