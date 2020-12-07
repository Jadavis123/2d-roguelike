/*
 * FieldOfView.cs - calculates lighting/field of view mesh each frame
 * 
 * Alek DeMaio, Doug McIntyre, Inaya Alkhatib, JD Davis, June Tejada
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [HideInInspector][SerializeField] new Renderer renderer;

    private Mesh mesh;
    private float fov;
    private Vector3 origin;
    private float startingAngle;
    public float viewDistance = 5f;
    
    // Start is called before the first frame update
    private void Start() //sets up mesh and makes sure it always renders on top of everything else
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        renderer = GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "Top";
        renderer.sortingOrder = 10;
        origin = Vector3.zero;
    }

    private void LateUpdate() //LateUpdate called after all other Update() calls - breaks up circle into 200 triangles, then loads them into the mesh
    {
        fov = 360f;
        int rayCount = 200;
        float angle = 0f;
        float angleIncrease = fov / rayCount;
        
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++) //for each triangle, sends out a raycast checking for collision with walls on BlockingLayer, then sets triangle endpoint either to collision point or max distance
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null)
            {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                vertex = raycastHit2D.point;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) - fov/2f;
    }

    public Vector3 GetVectorFromAngle(float angle) //converts angle in degrees to radians, then to unit vector in appropriate direction
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public float GetAngleFromVectorFloat(Vector3 dir) //converts direction of vector to angle in degrees
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) + Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
