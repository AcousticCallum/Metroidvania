using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera instance;

    public Camera mainCamera;
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        instance = this;
    }
}
