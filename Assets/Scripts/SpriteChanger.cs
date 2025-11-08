using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private UnderwaterBuoyancyController buoyancyController;

    private void Update()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        float buoy = buoyancyController.GetCurrentBuoyancy();
        float normalized = Mathf.InverseLerp(buoyancyController.minBuoyancy, buoyancyController.maxBuoyancy, buoy);
        int index = Mathf.FloorToInt(normalized * (sprites.Length - 1));
        index = Mathf.Clamp(index, 0, sprites.Length - 1);

        Sprite selectedSprite = sprites[index];
        if (selectedSprite != null)
        {
            spriteRenderer.sprite = selectedSprite;
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // opaque, visible
        }
        else
        {
            Debug.LogWarning($"Sprite à l'index {index} est null !");
        }

        Debug.Log($"Buoy: {buoy}, Normalized: {normalized}, Index: {index}");
        Debug.Log($"SpriteRenderer Color: {spriteRenderer.color}");
        Debug.Log($"Position: {transform.position}, Scale: {transform.localScale}");
    }
}
