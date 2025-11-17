using Pathfinding;
using UnityEngine;
using UnityEngine.Rendering;

public class AIAgent : MonoBehaviour
{
    private AIPath path;
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform target;
    [SerializeField] private float stopdistance = 2f;

    public void Start()
    {
        target = GameManager.Instance.playerScript.transform;
        path = GetComponent<AIPath>();
        path.maxSpeed = speed;
    }
    public void Move()
    {
        path.canMove = true;
        path.destination = target.position;
    }
     public void Stop()
    {
         path.canMove = false;
   
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
