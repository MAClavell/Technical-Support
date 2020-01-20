using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpawnZone : MonoBehaviour
{
    [SerializeField] Vector2 size;

    public Vector3 GetRandomPointInZone()
    {
        Vector3 point = new Vector3(Random.Range(-size.x, size.x), GameManager.gameObjectYPosition, Random.Range(-size.y, size.y));
        return transform.position + point;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Gizmos
    /// </summary>
    [ExecuteInEditMode]
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x*2, 1, size.y*2));
    }
#endif
}
