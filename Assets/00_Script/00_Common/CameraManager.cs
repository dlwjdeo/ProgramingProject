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
        CinemachineVirtualCamera targetVC;

        switch (area)
        {
            case CameraArea.Floor1:
                targetVC = floor1VC;
                break;
            case CameraArea.Floor2:
                targetVC = floor2VC;
                break;
            default:
                targetVC = null;
                break;
        }

        if (targetVC == null) return;

        if (currentVC != null)
            currentVC.Priority = DefaultPriority;

        targetVC.Priority = ActivePriority;
        currentVC = targetVC;
    }
}
