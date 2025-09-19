using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera floor1VC;
    [SerializeField] private CinemachineVirtualCamera floor2VC;

    private CinemachineVirtualCamera currentVC;

    private const int DefaultPriority = 5;
    private const int ActivePriority = 10;

    protected override void Awake()
    {
        base.Awake();
         currentVC = floor1VC;
    }
    public void SwitchCamera(CameraArea area)
    {
        CinemachineVirtualCamera targetVC = area switch
        {
            CameraArea.Floor1 => floor1VC,
            CameraArea.Floor2 => floor2VC,
            _ => null
        };

        if (targetVC == null) return;

        if (currentVC != null)
            currentVC.Priority = DefaultPriority;

        targetVC.Priority = ActivePriority;
        currentVC = targetVC;
    }
}
