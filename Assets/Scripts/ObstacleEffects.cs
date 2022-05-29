using UnityEngine;

[SelectionBase, ExecuteAlways]
public class ObstacleEffects : MonoBehaviour
{
    public enum TileEffect { none, death, drafting, activating }

    public TileEffect tileEffect;

    // Neutral stuff
    [SerializeField] Material neutralTileColor;

    // Death stuff
    [SerializeField] Material deathTileColor;

    // Draft stuff
    [SerializeField] Material draftTileColor;
    [SerializeField] float draftSpeed;
    [SerializeField] float draftHeight;

    // Activate stuff
    [SerializeField] Material activateTileColor;
    [SerializeField] GameObject[] activatedObject;
    [SerializeField] float deactivateAfterTime;
    

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
            case TileEffect.none:
                mr.material = neutralTileColor;
                hitBox.center = Vector3.zero + Vector3.up * transform.localScale.y / 2;
                hitBox.size = Vector3.one;
                break;

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

            case TileEffect.activating:
                if (activateTileColor != null) mr.material = activateTileColor;
                hitBox.center = Vector3.zero + Vector3.up * transform.localScale.y / 2;
                hitBox.size = Vector3.one;

                if (Application.IsPlaying(gameObject))
                {
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
                    character.deathBehaviour();
                    Debug.Log("you died!");
                    break;

                case TileEffect.drafting:
                    Debug.Log("you are now floating!");
                    break;

                case TileEffect.activating:
                    Debug.Log("You activated something!");
                    foreach (GameObject obj in activatedObject)
                    {
                        if (obj != null)
                        {
                            IActivateable temp = obj.GetComponent<IActivateable>();
                            temp.Activate(deactivateAfterTime);
                        }
                    }
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
                    Character.isFloating = true;
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch (tileEffect)
        {
            case TileEffect.none:
                Gizmos.color = new Color(200 / 255f, 200 / 255f, 200 / 255f, 0.4f);
                break;

            case TileEffect.death:
                Gizmos.color = new Color(255 / 255f, 0 / 255f, 17 / 255f, 0.4f);
                break;

            case TileEffect.drafting:
                Gizmos.color = new Color(0 / 255f, 51 / 255f, 0 / 255f, 0.4f);
                break;

            case TileEffect.activating:
                Gizmos.color = new Color(255 / 255f, 255 / 255f, 0 / 255f, 0.4f);
                break;

        }
        Gizmos.DrawCube(hitBox.transform.position + hitBox.center, hitBox.size);
    }
}
