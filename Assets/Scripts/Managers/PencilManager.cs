using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PencilManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private PencilContainer _pencilContainerPrefab;
    [SerializeField] private Transform _pencilContainerParent;
    private PencilContainer _selectedPencilContainer;

    [Header("Settings")]
    [SerializeField] private Color[] _colors;

    // Start is called before the first frame update
    private void Awake()
    {
       // LeanTween.init();
        
    }
    void Start()
    {
        CreatePencils();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CreatePencils()
    {
        for (int i = 0; i < _colors.Length; i++)
        {
            CreateAPencil(_colors[i], i);
        }
    }


    private void CreateAPencil(Color color, int pencilIndex)
    {
        PencilContainer pencilContainerInstance = Instantiate(_pencilContainerPrefab, _pencilContainerParent);
        pencilContainerInstance.ConfigurePencil(color, this);

        //if(pencilIndex == 0)
        //{
        //    PencilContainerClickedCalback(pencilContainerInstance);
        //}

    }
    public void PencilContainerClickedCalback(PencilContainer pencilContainer)
    {
        if(_selectedPencilContainer == pencilContainer && _selectedPencilContainer != null)
        {
            return;
        }

        pencilContainer.Select();

        if(_selectedPencilContainer != null )
        {
            _selectedPencilContainer.Deselect();
        }
        _selectedPencilContainer = pencilContainer;

        GPUSpriteBrush.Instance.SetBrushColor(pencilContainer.GetColor());
        BrushSizeManagers.Instance.SetSelectedColor(pencilContainer.GetColor());

    }
}
