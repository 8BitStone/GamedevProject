using UnityEngine;

public class PositionFixerInMenu : MonoBehaviour
{
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }


    private void LateUpdate()
    {
        transform.position = new Vector3(_initialPosition.x, transform.position.y, _initialPosition.z);
    }
}
