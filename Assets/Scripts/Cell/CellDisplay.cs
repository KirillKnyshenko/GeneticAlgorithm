using System;
using UnityEngine;

public class CellDisplay : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        Cell cell = Manager.Instance.cells[(int)transform.position.x, (int)transform.position.y];
        Vector3 drawVector = new Vector3(cell.Rotation.x, cell.Rotation.y) + transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(drawVector, Vector3.one);
    }
}