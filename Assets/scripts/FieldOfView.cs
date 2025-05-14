using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class FieldOfView : MonoBehaviourPunCallbacks
{
    [SerializeField] private LayerMask _layerMask;
    private Mesh _mesh;
    private float _fov;
    private float _startingAngle;

    private List<GameObject> playersToCheck;
    
    void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _fov = 90f;

        playersToCheck = new();
        if (photonView.IsMine) FindAllOtherPlayers();
        else this.enabled = false; // Les autres joueurs ne doivent pas exécuter leur propre FOV
    }
    
    void LateUpdate()
    {
        int rayCount = 50;
        float angle = _startingAngle;
        float angleIncrease = _fov / rayCount;
        float viewDistance = 20f;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
    
        vertices[0] = Vector3.zero;
        int vertexIndex = 1;
        int triangleIndex = 0;

        HashSet<GameObject> visibleObjects = new HashSet<GameObject>();

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 dir = Utils.GetVectorFromAngle(angle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewDistance, _layerMask);

            Vector3 vertex;

            if (hit.collider != null)
            {
                vertex = hit.point;
                
                foreach (GameObject player in playersToCheck)
                {
                    if (hit.collider.gameObject == player)
                    {
                        visibleObjects.Add(player);
                    }
                }
            }
            else
            {
                vertex = transform.position + dir * viewDistance;
            }

            vertices[vertexIndex] = transform.InverseTransformPoint(vertex);

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
        _mesh.RecalculateBounds();

        // Affiche ou cache les joueurs
        foreach (GameObject player in playersToCheck)
        {
            Transform visual = player.transform.Find("playerVisual"); // à adapter si besoin
            if (visual != null)
            {
                visual.gameObject.SetActive(visibleObjects.Contains(player));
            }
        }
    }


    public void SetAimDirection(Vector3 aimDirection)
    {
        _startingAngle = Utils.GetAngleFromVector(aimDirection) + _fov/2f;
    }
    
    private void FindAllOtherPlayers()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView view = go.GetComponent<PhotonView>();
            if (view != null && !view.IsMine) playersToCheck.Add(go);
        }
    }
}
