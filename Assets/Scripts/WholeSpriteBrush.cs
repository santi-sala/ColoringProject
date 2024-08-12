using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WholeSpriteBrush : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color brushColor = Color.red;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            RaycastSprite();
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

    private void ColorSprite(Collider2D collider)
    {
        SpriteRenderer spriteRenderer = collider.GetComponent<SpriteRenderer>();

        if(spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = brushColor;
    }
}
