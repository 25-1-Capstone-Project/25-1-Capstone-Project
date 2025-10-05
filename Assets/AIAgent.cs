using Pathfinding;
using UnityEngine;
using UnityEngine.Rendering;

public class AIAgent : MonoBehaviour
{
    private AIPath path;
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform target;
    [SerializeField] private float stopdistance = 2f;
    private float distanceToTarget;
    void Start()
    {
        target = PlayerScript.Instance.transform;
        path = GetComponent<AIPath>();
    }
    public void Move()
    {
        path.maxSpeed = speed;
        distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget < stopdistance)
        {
            path.destination = transform.position;
        }
        else
        {
            path.destination = target.position;
        }
    }
}
