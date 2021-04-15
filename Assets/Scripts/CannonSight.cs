using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CannonSight : MonoBehaviour
{
    public float fov = 90f;

    public Vector2 direction = Vector2.up;
    public float fromDistance = 0f;
    public float toDistance = 10f;

    public Ship shipOwner;
    public List<int> shipInRange = new List<int>();


    public PolygonCollider2D collider2D;

    Mesh mesh;

    public float ToDistance
    {
        get
        {
            return toDistance;
        }
        set
        {
            toDistance = value;
            DrawDirection(Vector2.zero, direction, fov, fromDistance, toDistance);
        }
    }

    private void Awake()
    {

        mesh = new Mesh();
        GetComponent<MeshRenderer>().sortingLayerName = "Surface";
        GetComponent<MeshFilter>().mesh = mesh;
        collider2D = GetComponent<PolygonCollider2D>();
        DrawDirection(Vector2.zero, direction, fov, fromDistance, toDistance);
    }


    // Start is called before the first frame update
    void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
    }


    private void DrawMesh()
    {
        float fov = 90f;
        Vector3 origin = Vector3.zero;
        int rayCount = 50;
        float angel = 0f;
        float angelIncrease = fov / rayCount;
        float viewDistance = 10f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];
        int[] triangles = new int[rayCount * 3];


        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = origin + GetVectorFromAngel(angel) * viewDistance;
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angel -= angelIncrease;
        }
        // for (int i = 0; i < colors.Length; i++)
        // {
        //     colors[i] = new Color(0f, 0f, 0f, 0f);
        // }
        colors[0] = new Color(1, 1, 1, 1f);
        // colors[0] = new Color(1, 0, 0, 1f);

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;
    }
    private void DrawMeshCannon()
    {
        DrawDirection(Vector2.zero, new Vector2(1, -1), 30, 0f, 10f); //front
        DrawDirection(Vector2.zero, new Vector2(-1, -1), 90, 0f, 10f); //right
        DrawDirection(Vector2.zero, new Vector2(1, 1), 90, 0f, 10f); //left
        DrawDirection(Vector2.zero, new Vector2(-1, 1), 30, 0f, 10f); //back
    }

    private void DrawDirection(Vector3 origin, Vector2 direction, float fov, float fromDistance, float toDistance)
    {
        int rayCount = 50;
        float angel = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + fov / 2;

        float angelIncrease = fov / rayCount;
        float viewDistance = toDistance;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] colliders = new Vector2[vertices.Length];
        Vector2[] uv = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];
        int[] triangles = new int[rayCount * 3];


        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = origin + GetVectorFromAngel(angel) * viewDistance;
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angel -= angelIncrease;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            colliders[i] = vertices[i];
        }



        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0, 0, 0, 0.1f);
        }
        colors[0] = new Color(1, 1, 1, 1f);
        // colors[0] = new Color(224f, 122f, 122f, 1f);

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;


        collider2D.SetPath(0, colliders);
    }

    private Vector3 GetVectorFromAngel(float angel)
    {
        float angelRad = angel * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angelRad), Mathf.Sin(angelRad));
    }

    public void ValidColorMesh()
    {
        Color[] colors = mesh.colors;
        if (shipInRange.Count > 0)
        {
            colors[0] = new Color(224f, 122f, 122f, 1f);
        }
        else
        {
            colors[0] = new Color(1, 1, 1, 1f);
        }
        mesh.colors = null;
        mesh.colors = colors;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (shipOwner == null) return;
        if (other.tag.Equals(GameSettings.TAG_SHIP))
        {
            Ship target = other.gameObject.GetComponent<Ship>();
            if (!shipOwner.IsSameShip(target) && !shipInRange.Contains(target.shipId))
            {
                shipInRange.Add(target.shipId);
                ValidColorMesh();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (shipOwner == null) return;
        if (other.tag.Equals(GameSettings.TAG_SHIP))
        {
            Ship target = other.gameObject.GetComponent<Ship>();
            if (!shipOwner.IsSameShip(target) && shipInRange.Contains(target.shipId))
            {
                shipInRange.Remove(target.shipId);
                ValidColorMesh();
            }
        }
    }

}
