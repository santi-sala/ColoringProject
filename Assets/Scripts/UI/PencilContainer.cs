using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PencilContainer : MonoBehaviour
{
    [Header ("Elements")]
    [SerializeField] private RectTransform _pencilParent;
    [SerializeField] private Image[] _pencilImages;

    [Header("Settings")]
    [SerializeField] private float  _moveMagnitude;
    [SerializeField] private float _moveDuration;
    private Vector2 _selectedPosition;
    private Vector2 _deselectedPosition;
    private PencilManager _pencilManager;
    private Color _pencilColor;

    // Start is called before the first frame update
    private void Awake()
    {
        _deselectedPosition = _pencilParent.anchoredPosition;
        _selectedPosition = _deselectedPosition + _moveMagnitude * Vector2.right;
    }    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Select();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Deselect();
        }
    }

    public void ConfigurePencil(Color color, PencilManager pencilManager)
    {
        this._pencilColor = color;
        for (int i = 0; i < _pencilImages.Length; i++)
        {
            _pencilImages[i].color = color;
        }

        this._pencilManager = pencilManager; 
    }

    public void Select()
    {
        Debug.Log("Selecting Pencil");
        LeanTween.cancel(_pencilParent);
        LeanTween.move(_pencilParent, _selectedPosition, _moveDuration).setEase(LeanTweenType.easeInOutCubic);
    }

    public void Deselect()
    {
        LeanTween.cancel(_pencilParent);
        LeanTween.move(_pencilParent, _deselectedPosition, _moveDuration).setEase(LeanTweenType.easeInOutCubic);
    }

    public void ClickedCallback()
    {

        _pencilManager.PencilContainerClickedCalback(this);
    }

    public Color GetColor()
    {
        return _pencilColor;
    }
}
