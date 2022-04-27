using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourCamera : MonoBehaviour
{
    void Start()
    {
        transform.position = (new Vector3(Manager.Instance.world.size, Manager.Instance.world.size) / 2) - Vector3.forward;
        Camera.main.orthographicSize = Manager.Instance.world.size;
    }
}
