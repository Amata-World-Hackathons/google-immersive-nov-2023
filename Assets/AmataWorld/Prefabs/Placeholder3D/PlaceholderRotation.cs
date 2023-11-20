using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderRotation : MonoBehaviour
{
    [SerializeField]
    Quaternion _angularVelocity = Quaternion.Euler(1, 2, 3);

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation *= _angularVelocity;
    }
}
