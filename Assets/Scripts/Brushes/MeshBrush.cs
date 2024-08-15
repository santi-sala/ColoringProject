using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshBrush : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private MeshFilter _meshFilterPrefab;

    [Header("Settings")]
    [SerializeField] private float _brushSize;
    [SerializeField] private float _minimumDistanceBetweenPoints = 0.1f;

    private Mesh _mesh;
    private Vector2 _lastMouseClickPostion;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();
    private List<Vector2> _uvs = new List<Vector2>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateMesh();
        }
        else if (Input.GetMouseButton(0))
        {
            PaintMesh();
        }
    }    

    private void CreateMesh()
    {
        RaycastHit raycastHit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, Mathf.Infinity);

        if (raycastHit.collider == null)
        {
            return;
        }

        _mesh = new Mesh();

        //Emptying the lists
        _vertices.Clear();
        _triangles.Clear(); 
        _uvs.Clear();

        //Creating the vertices
        _vertices.Add(raycastHit.point + (Vector3.up + Vector3.right) * _brushSize / 2);
        _vertices.Add(_vertices[0] + Vector3.down * _brushSize);
        _vertices.Add(_vertices[1] + Vector3.left * _brushSize);
        _vertices.Add(_vertices[2] + Vector3.up * _brushSize);

        //Creating the UVs
        _uvs.Add(Vector2.one);
        _uvs.Add(Vector2.right);
        _uvs.Add(Vector2.zero);
        _uvs.Add(Vector2.up);

        //Creating the triangles
        _triangles.Add(0);
        _triangles.Add(1);
        _triangles.Add(2);

        _triangles.Add(0);
        _triangles.Add(2);
        _triangles.Add(3);

        //Assigning the vertices and triangles to the mesh
        _mesh.vertices = _vertices.ToArray();
        _mesh.uv = _uvs.ToArray();
        _mesh.triangles = _triangles.ToArray();

        /*
        GameObject meshGameObject = new GameObject();
        meshGameObject.AddComponent<MeshFilter>().mesh = _mesh;
        meshGameObject.AddComponent<MeshRenderer>();
        meshGameObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color"));
        */
        float zPosition = transform.childCount * 0.01f;
        Vector3 position = Vector3.back * zPosition;

        MeshFilter meshFilterInstance = Instantiate(_meshFilterPrefab, position, Quaternion.identity, transform);
        meshFilterInstance.sharedMesh = _mesh;
        
        meshFilterInstance.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();

        _lastMouseClickPostion = Input.mousePosition;


    }

    private void PaintMesh()
    {
        if (Vector2.Distance(_lastMouseClickPostion, Input.mousePosition) < _minimumDistanceBetweenPoints)
        {
            return;
        }

        _lastMouseClickPostion = Input.mousePosition;

        RaycastHit raycastHit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, Mathf.Infinity);

        if (raycastHit.collider == null)
        {
            return;
        }

        int verticesCount = _mesh.vertices.Length;

        _vertices.Add(raycastHit.point + (Vector3.up + Vector3.right) * _brushSize / 2);
        _vertices.Add(_vertices[verticesCount + 0] + Vector3.down * _brushSize);
        _vertices.Add(_vertices[verticesCount + 1] + Vector3.left * _brushSize);
        _vertices.Add(_vertices[verticesCount + 2] + Vector3.up * _brushSize);

        _uvs.Add(Vector2.one);
        _uvs.Add(Vector2.right);
        _uvs.Add(Vector2.zero);
        _uvs.Add(Vector2.up);

        _triangles.Add(verticesCount);
        _triangles.Add(verticesCount + 1);
        _triangles.Add(verticesCount + 2);

        _triangles.Add(verticesCount + 0);
        _triangles.Add(verticesCount + 2);
        _triangles.Add(verticesCount + 3);

        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.uv = _uvs.ToArray();

        // Reassigning the mesh to the last child
        transform.GetChild(transform.childCount - 1).GetComponent<MeshFilter>().sharedMesh = _mesh;
    }
}
