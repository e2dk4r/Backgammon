using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ThrowLocation : MonoBehaviour
{
    public Vector2 direction;

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(direction.x, direction.y, 0), Color.blue);
    }
}
