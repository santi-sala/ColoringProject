using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBrush : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private MeshFilter _meshFilterPrefab;
    [Header("Settings")]
    private Mesh _mesh;
    // Start is called before the first frame update
    void Start()
    {
        _mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[6];

        vertices[0] = Vector3.up;
        vertices[1] = Vector3.zero;
        vertices[2] = Vector3.left;
        vertices[3] = vertices[0] + Vector3.left;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
