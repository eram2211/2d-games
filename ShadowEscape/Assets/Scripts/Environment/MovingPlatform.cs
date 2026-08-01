using UnityEngine;
using ShadowEscape.Managers;

namespace ShadowEscape.Environment
{
    /// <summary>
    /// Moves a platform back and forth between waypoints. Carries the player/clone
    /// by parenting them while standing on top (detected via trigger above the platform,
    /// or simply by physics friction if using a PhysicsMaterial2D with high friction).
    /// Attach to: any Platform GameObject with Rigidbody2D (Kinematic) + Collider2D.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float speed = 2f;
        [SerializeField] private bool pingPong = true;
        [SerializeField] private float waitAtWaypoint = 0.5f;

        private Rigidbody2D rb;
        private int currentWaypoint;
        private int direction = 1;
        private float waitTimer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private void FixedUpdate()
        {
            if (waypoints == null || waypoints.Length < 2) return;
            if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

            if (waitTimer > 0f)
            {
                waitTimer -= Time.fixedDeltaTime;
                return;
            }

            Transform target = waypoints[currentWaypoint];
            Vector2 newPos = Vector2.MoveTowards(rb.position, target.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);

            if (Vector2.Distance(newPos, target.position) < 0.02f)
            {
                waitTimer = waitAtWaypoint;
                AdvanceWaypoint();
            }
        }

        private void AdvanceWaypoint()
        {
            if (pingPong)
            {
                if (currentWaypoint == waypoints.Length - 1) direction = -1;
                else if (currentWaypoint == 0) direction = 1;
                currentWaypoint += direction;
            }
            else
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            }
        }

        // Parent the player to the platform while standing on it so they move together.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("ShadowClone"))
            {
                collision.transform.SetParent(transform);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("ShadowClone"))
            {
                collision.transform.SetParent(null);
            }
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null) return;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;
                Gizmos.DrawWireSphere(waypoints[i].position, 0.15f);
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
