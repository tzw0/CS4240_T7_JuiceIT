using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnObjectOnCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }
}
