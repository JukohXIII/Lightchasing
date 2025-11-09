using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform target; // ton joueur
    public float followSpeed = 5f;
    public SpriteRenderer mapRenderer;

    [Header("Limits")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    
    void Start()
    {
        Bounds b = mapRenderer.bounds;

        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        minX = b.min.x + camHalfWidth;
        maxX = b.max.x - camHalfWidth;
        minY = b.min.y + camHalfHeight;
        maxY = b.max.y - camHalfHeight;
    }
    
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);


        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);

        transform.position = smoothedPosition;
    }
}
