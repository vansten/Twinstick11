using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RenderOnDemand : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Render")]
    void Render()
    {
        Camera c = GetComponent<Camera>();
        if(c == null || c.targetTexture == null)
        {
            Debug.LogError("There is no camera or render target texture set");
            return;
        }
        RenderTexture.active = c.targetTexture;
        c.Render();

        Texture2D textureToWrite = new Texture2D(c.targetTexture.width, c.targetTexture.height);
        textureToWrite.ReadPixels(new Rect(0, 0, c.targetTexture.width, c.targetTexture.height), 0, 0);
        textureToWrite.Apply();
        RenderTexture.active = null;

        byte[] bytes = textureToWrite.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/p.png", bytes);
        Debug.Log("Written to: " + Application.persistentDataPath + "/p.png");
    }
#endif
}
