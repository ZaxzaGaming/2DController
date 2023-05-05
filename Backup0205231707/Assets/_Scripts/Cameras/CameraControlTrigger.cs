using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;
    private Collider2D coll;
    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           
            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - coll.bounds.center).normalized;
            if (customInspectorObjects.swapCameras && !customInspectorObjects.XAxisOn && (customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null) || (customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null))
            {
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, customInspectorObjects.cameraOnTop,  customInspectorObjects.cameraOnBottom, exitDirection);
            }
            if (customInspectorObjects.swapCameras && customInspectorObjects.XAxisOn && (customInspectorObjects.cameraOnTop != null && customInspectorObjects.cameraOnBottom != null) || (customInspectorObjects.cameraOnTop != null && customInspectorObjects.cameraOnBottom != null))
            {
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnBottom, customInspectorObjects.cameraOnTop, customInspectorObjects.cameraOnBottom, customInspectorObjects.cameraOnTop, exitDirection);
            }
            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    }
}
[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;
    [HideInInspector] public bool XAxisOn;
    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;
    [HideInInspector] public CinemachineVirtualCamera cameraOnTop;
    [HideInInspector] public CinemachineVirtualCamera cameraOnBottom;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}
public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}
[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;
    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (cameraControlTrigger.customInspectorObjects.swapCameras)
        {
            cameraControlTrigger.customInspectorObjects.XAxisOn = EditorGUILayout.Toggle("X Axis (ON)", cameraControlTrigger.customInspectorObjects.XAxisOn);
            if (!cameraControlTrigger.customInspectorObjects.XAxisOn)
            {
                cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraControlTrigger.customInspectorObjects.cameraOnLeft,
                    typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
                cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControlTrigger.customInspectorObjects.cameraOnRight,
                    typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            }
            else
            {
                cameraControlTrigger.customInspectorObjects.cameraOnTop = EditorGUILayout.ObjectField("Camera on Top", cameraControlTrigger.customInspectorObjects.cameraOnTop,
                    typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
                cameraControlTrigger.customInspectorObjects.cameraOnBottom = EditorGUILayout.ObjectField("Camera on Bottom", cameraControlTrigger.customInspectorObjects.cameraOnBottom,
                    typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
            }
        }
        if(cameraControlTrigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                cameraControlTrigger.customInspectorObjects.panDirection);
            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectorObjects.panDistance);
            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectorObjects.panTime);
        }
        if(GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}
