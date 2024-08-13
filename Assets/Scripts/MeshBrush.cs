using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBrush : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private MeshFilter _meshFilterPrefab;

    [Header("Settings")]
    [SerializeField] private float _brushSize;
    private Mesh _mesh;
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateMesh();
        }
}

    private void CreateMesh()
    {
        RaycastHit raycastHit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 20);

        if (raycastHit.collider == null)
        {
            return;
        }

        _mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];

        vertices[0] = raycastHit.point + (Vector3.up + Vector3.right) * _brushSize / 2;
        vertices[1] = vertices[0] + Vector3.down * _brushSize;
        vertices[2] = vertices[1] + Vector3.left * _brushSize;
        vertices[3] = vertices[2] + Vector3.up * _brushSize;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;

        /*
        GameObject meshGameObject = new GameObject();
        meshGameObject.AddComponent<MeshFilter>().mesh = _mesh;
        meshGameObject.AddComponent<MeshRenderer>();
        meshGameObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color"));
        */

        MeshFilter meshFilterInstance = Instantiate(_meshFilterPrefab, transform);
        meshFilterInstance.sharedMesh = _mesh;
    }
}
