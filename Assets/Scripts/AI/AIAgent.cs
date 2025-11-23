using Pathfinding;
using UnityEngine;


public class AIAgent : MonoBehaviour
{
    private AIPath path;
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform target;

    public void Start()
    {
        target = GameManager.Instance.playerScript.transform;
        path = GetComponent<AIPath>();
        path.maxSpeed = speed;
    }
    public void Move()
    {
        if (target == null) return;
        path.canMove = true;
        path.destination = target.position;
    }
    public void Stop()
    {
        path.canMove = false;

    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
