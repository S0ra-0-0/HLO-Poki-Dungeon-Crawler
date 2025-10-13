// Unity
using UnityEngine;

public class MiniMapCameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Update()
    {
        if (target)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
