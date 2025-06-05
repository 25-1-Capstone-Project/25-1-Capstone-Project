using UnityEngine;

public class BoomerangProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float travelDistance;
    private float speed;
    private float traveled;
    private bool returning = false;
    private bool isWall = false;

    private Transform playerTransform;

    public void Initialize(Vector2 dir, float distance, float spd, Transform player)
    {
        direction = dir.normalized;
        travelDistance = distance;
        speed = spd;
        playerTransform = player;

        traveled = 0f;
        returning = false;
        isWall = false;
    }

    void Update()
    {
        Vector3 move = direction * speed * Time.deltaTime;
        transform.position += move;

        traveled = Vector3.Distance(playerTransform.position, transform.position);

        if ((!returning && traveled >= travelDistance) || isWall)
        {
            returning = true;
        }

        if (returning)
        {
            direction = (playerTransform.position - transform.position).normalized;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 10)
        {
            isWall = true;
        }

        if (other.gameObject.layer == 3 && returning)
        {

            gameObject.SetActive(false);
        }
    }
}
