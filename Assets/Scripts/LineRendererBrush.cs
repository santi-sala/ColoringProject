using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererBrush : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private LineRenderer _lineRendererPrefab;

    [Header("Settings")]
    [SerializeField] private Color _brushColor;
    [SerializeField] private float _minimumDistanceBetweenPoints = 0.1f;

    private LineRenderer _currentLineRenderer;
    private Vector2 _lastMouseClickPostion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        if(Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }else if(Input.GetMouseButton(0))
        {
            PaintOnPaintingCanvas();
        }
    }

    

    private void CreateLine()
    {
        RaycastHit raycastHit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 20);

        if(raycastHit.collider == null)
        {
            return;
        }

        _currentLineRenderer = Instantiate(_lineRendererPrefab, raycastHit.point, Quaternion.identity, transform);

        _currentLineRenderer.SetPosition(0, raycastHit.point);
        _currentLineRenderer.startColor = _brushColor;
        _currentLineRenderer.endColor = _currentLineRenderer.startColor;

        _lastMouseClickPostion = Input.mousePosition;
    }

    private void PaintOnPaintingCanvas()
    {
        if (_currentLineRenderer == null)
        {
            return;
        }

        if(Vector2.Distance(_lastMouseClickPostion, Input.mousePosition) < _minimumDistanceBetweenPoints )
        {
            return;
        }

        _lastMouseClickPostion = Input.mousePosition;

        RaycastHit raycastHit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 20);

        if (raycastHit.collider == null)
        {
            return;
        }

        AddPoint(raycastHit.point);
    }

    private void AddPoint(Vector3 worldPosition)
    {
        _currentLineRenderer.positionCount++;
        _currentLineRenderer.SetPosition(_currentLineRenderer.positionCount - 1, worldPosition);
    }
}
