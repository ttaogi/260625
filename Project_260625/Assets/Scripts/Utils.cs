using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public static class Utils
{
    public static void Log(string msg)
    {
#if UNITY_EDITOR || DEBUG
        Debug.Log("::: " + msg);
#endif
    }

    /// <summary> Enum 의 Description 값 읽어오기. </summary>
    public static string StringValueOfEnum(Enum value)
    {
        try
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return "";
    }

    /// <summary> 하위 오브젝트까지 레이어 변경. </summary>
    public static void ChangeLayer(GameObject go, int layer, params GameObject[] noChangeLayerObjects)
    {
        for (int i = 0; i < noChangeLayerObjects.Length; ++i)
            if (go.Equals(noChangeLayerObjects[i]))
                return;

        go.layer = layer;

        foreach (Transform child in go.transform)
            ChangeLayer(child.gameObject, layer, noChangeLayerObjects);
    }

    public static Camera GetCamera(eLayer layer)
    {
        Camera camera = null;

        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.gameObject.activeInHierarchy)
            {
                var cullingMask = 1 << (int)layer;
                if ((cam.cullingMask & cullingMask) != 0)
                {
                    if (camera == null || camera.depth < cam.depth)
                        camera = cam;
                }
            }
        }

        return camera;
    }
}
