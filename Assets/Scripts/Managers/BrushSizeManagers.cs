using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushSizeManagers : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image[] _brushSizeImages;


    [Header("Settings")]
    [SerializeField] private float[] _brushSizes;
    [SerializeField] private Color _selectedColor = Color.yellow;
    [SerializeField] private Color _unselectedColor = Color.gray;
    // Start is called before the first frame update
    void Start()
    {
        BrushSizeButtonClickedCallback(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BrushSizeButtonClickedCallback(int sizeIndex)
    {
        if (sizeIndex < 0 || sizeIndex > _brushSizes.Length - 1)
        {
            Debug.LogWarning("Brush zixe not Found!");
            return;
        }
        float targetBrushSize = _brushSizes[sizeIndex];

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
}
