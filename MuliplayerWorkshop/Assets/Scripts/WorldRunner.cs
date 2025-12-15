using UnityEngine;

public class WorldRunner : MonoBehaviour
{
    [SerializeField] private float runSpeed = 5f;
    private void FixedUpdate()
    {
        transform.Translate(Vector3.left * runSpeed * Time.fixedDeltaTime);
    }
}
