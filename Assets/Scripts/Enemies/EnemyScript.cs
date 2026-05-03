using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * HOW TO USE:
 * 1. Attach to an Enemy GameObject with a CircleCollider2D (Trigger).
 * 2. Assign 'Target Mask' to your Player layer and 'Obstacle Mask' to Walls/Environment.
 * 3. Assign a MeshFilter to 'View Mesh Filter' to visualize the FOV cone in-game.
 * 4. Adjust 'View Radius' and 'View Angle' to set the detection zone.
 * 5. Requires 'LevelManager' in the scene to trigger a reset upon detection.
 */
public class EnemyScript : MonoBehaviour
{
    [Header("Detection Settings")]
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    [Range(0, 360)] public float fovRotation = 90f;

    [Header("Physics Layers")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Visual Mesh")]
    public MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    public float meshResolution = 1f;
    public int edgeResolveIterations = 4;
    public float edgeDstThreshold = 0.5f;

    // RESTORED FOR THE EDITOR SCRIPT
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        if (viewMeshFilter != null) viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetsWithDelay", 0f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FixedUpdate() { DrawFieldOfView(); }

    // --- NEW FUNCTION: CATCH ON TOUCH ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object touched is on the target layer
        if (((1 << collision.gameObject.layer) & targetMask) != 0)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.ResetOnDeath();
            }
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        // 1. Find the player using a circular overlap
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        foreach (Collider2D targetCollider in targetsInViewRadius)
        {
            Transform target = targetCollider.transform;
            Vector2 dirToTarget = (target.position - transform.position).normalized;

            // 2. Angle Check (Is the player in the "slice of pie"?)
            if (Vector2.Angle(new Vector2(Mathf.Sin(fovRotation * Mathf.Deg2Rad), Mathf.Cos(fovRotation * Mathf.Deg2Rad)), dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // 3. RaycastAll to look "through" the enemy's own body
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, dirToTarget, dstToTarget, obstacleMask | targetMask);

                foreach (var hit in hits)
                {
                    // If we hit a Wall (Obstacle) first, the view is blocked. Stop looking at this target.
                    if (((1 << hit.collider.gameObject.layer) & obstacleMask) != 0)
                    {
                        break;
                    }

                    // If we hit the Player, check if they are hiding
                    if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
                    {
                        Movement charMovement = target.GetComponent<Movement>();
                        if (charMovement == null || !charMovement.isHiding)
                        {
                            visibleTargets.Add(target);
                            target.GetComponent<Movement>()._rb.linearVelocity = Vector3.zero;
                            if (LevelManager.Instance != null)
                            {
                                LevelManager.Instance.ResetOnDeath();
                            }
                        }
                        break; // Found the player, no need to check further hits for this ray
                    }
                }
            }
        }
    }

    // --- MESH GENERATION ---
    void DrawFieldOfView()
    {
        int rayCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / rayCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= rayCount; i++)
        {
            float angle = fovRotation - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            if (i > 0)
            {
                if (oldViewCast.hit != newViewCast.hit)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero) viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero) viewPoints.Add(edge.pointB);
                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;
        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            if (newViewCast.hit == minViewCast.hit) { minAngle = angle; minPoint = newViewCast.point; }
            else { maxAngle = angle; maxPoint = newViewCast.point; }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);
        if (hit.collider != null) return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        else return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal) angleInDegrees += fovRotation;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public struct ViewCastInfo
    {
        public bool hit; public Vector3 point; public float dst; public float angle;
        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle) { hit = _hit; point = _point; dst = _dst; angle = _angle; }
    }
    public struct EdgeInfo
    {
        public Vector3 pointA; public Vector3 pointB;
        public EdgeInfo(Vector3 _pointA, Vector3 _pointB) { pointA = _pointA; pointB = _pointB; }
    }
}