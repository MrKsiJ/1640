using UnityEngine;

public class Cube : MonoBehaviour
{
    private Quaternion _originalRotation;

    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        _originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        angle++;

        Quaternion rotationY = Quaternion.AngleAxis(angle, Vector3.up);
        Quaternion rotationX = Quaternion.AngleAxis(angle * 2,Vector3.left);

        transform.rotation = _originalRotation * rotationY * rotationX;
    }
}
