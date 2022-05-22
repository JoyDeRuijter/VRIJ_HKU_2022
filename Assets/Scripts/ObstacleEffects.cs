using UnityEngine;

[SelectionBase, ExecuteAlways]
public class ObstacleEffects : MonoBehaviour
{
    public enum TileEffect { death, drafting }

    public TileEffect tileEffect;

    // Death stuff
    public Material deathTileColor;

    // Draft stuff
    public Material draftTileColor;
    public float draftSpeed;
    public float draftHeight;

    private BoxCollider hitBox;
    private MeshRenderer mr;

    private void Awake()
    {
        hitBox = GetComponent<BoxCollider>();
        mr = GetComponentInChildren<MeshRenderer>();
    }

    private void Update()
    {
        switch (tileEffect)
        {
            case TileEffect.death:
                if (deathTileColor != null) mr.material = deathTileColor;
                hitBox.center = Vector3.up * 0.15f;
                hitBox.size = new Vector3(1, 0.3f, 1);

                if (Application.IsPlaying(gameObject)) {
                    // do stuff...
                }
                break;

            case TileEffect.drafting:
                if (draftTileColor != null) mr.material = draftTileColor;
                hitBox.center = new Vector3(0, draftHeight / 2, 0);
                hitBox.size = new Vector3(1, draftHeight, 1);

                if (Application.IsPlaying(gameObject)) {
                    // do stuff...
                }
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();
        {
            switch (tileEffect)
            {
                case TileEffect.death:
                    Debug.Log("you died!");
                    break;

                case TileEffect.drafting:
                    Debug.Log("you are now floating!");
                    break;

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character != null)
        {
            switch (tileEffect)
            {
                case TileEffect.death:
                    break;

                case TileEffect.drafting:
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    rb.AddForce(Vector3.up * draftSpeed);
                    break;

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch (tileEffect)
        {
            case TileEffect.death:
                Gizmos.color = new Color(255 / 255f, 0 / 255f, 0 / 17f, 0.4f);
                break;

            case TileEffect.drafting:
                Gizmos.color = new Color(0 / 255f, 51 / 255f, 0 / 255f, 0.4f);
                break;

        }
        Gizmos.DrawCube(hitBox.transform.position + hitBox.center, hitBox.size);
    }
}
