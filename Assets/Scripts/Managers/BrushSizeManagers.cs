using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushSizeManagers : MonoBehaviour
{
    public static BrushSizeManagers Instance;
    [Header("Elements")]
    [SerializeField] private Image[] _brushSizeImages;


    [Header("Settings")]
    [SerializeField] private float[] _brushSizes;
    [SerializeField] private Color _selectedColor = Color.red;
    [SerializeField] private Color _unselectedColor = Color.gray;
    private int _selectedBrushSizeIndex = 0;
    // Start is called before the first frame update

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
    


    public void BrushSizeButtonClickedCallback(int sizeIndex)
    {
        if (sizeIndex < 0 || sizeIndex > _brushSizes.Length - 1)
        {
            Debug.LogWarning("Brush zixe not Found!");
            return;
        }
        float targetBrushSize = _brushSizes[sizeIndex];

        _selectedBrushSizeIndex = sizeIndex;

        GPUSpriteBrush.Instance.SetBrushSize(targetBrushSize);

        for (int i = 0; i < _brushSizeImages.Length; i++)
        {
            _brushSizeImages[i].color = (i == sizeIndex) ? _selectedColor : _unselectedColor;
            //if (i == sizeIndex)
            //{
            //    _brushSizeImages[i].color = _selectedColor;
            //}
            //else
            //{
            //    _brushSizeImages[i].color = _unselectedColor;
            //}
        }

    }

    public void SetSelectedColor(Color color)
    {
        _selectedColor = color;

        for (int i = 0; i < _brushSizeImages.Length; i++)
        {
            _brushSizeImages[i].color = (i == _selectedBrushSizeIndex) ? _selectedColor : _unselectedColor;
           
        }

    }
}
