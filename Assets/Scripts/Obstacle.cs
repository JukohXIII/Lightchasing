using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public ObstacleMovementType[] allowedMovements;
    public float speed = 2f;                 // vitesse de base
    public float speedMultiplier = 1f;       // défini par le spawner

    private Vector2 direction;

    void Start()
    {
        ChooseRandomMovement();
    }

    private void ChooseRandomMovement()
    {
        if (allowedMovements == null || allowedMovements.Length == 0)
        {
            Debug.LogWarning(name + " has no allowed movement types!");
            direction = Vector2.zero;
            return;
        }

        ObstacleMovementType chosen = allowedMovements[Random.Range(0, allowedMovements.Length)];

        switch (chosen)
        {
            case ObstacleMovementType.LeftToRight:
                direction = Vector2.right;
                break;
            case ObstacleMovementType.RightToLeft:
                direction = Vector2.left;
                break;
            case ObstacleMovementType.TopToBottom:
                direction = Vector2.down;
                break;
            case ObstacleMovementType.BottomToTop:
                direction = Vector2.up;
                break;
        }
    }

    void Update()
    {
        float finalSpeed = speed * speedMultiplier;
        transform.Translate(direction * finalSpeed * Time.deltaTime);
    }
}
