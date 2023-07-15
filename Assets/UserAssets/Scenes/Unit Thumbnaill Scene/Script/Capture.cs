using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Capture : MonoBehaviour
{
    public Color m_BackColor;
    public Camera m_Camera;
    public RenderTexture m_RenderTexture;
    public Image m_Back;

    [SerializeField] private GameObject[] m_CapturingGameObject;
    private int cnt;

    private Vector3 m_ObjPos = new Vector3(0, 0.5f, -8);
    private Vector3 m_ObjRot = new Vector3(0,180,0);
    public void Start()
    {
        m_Camera = Camera.main;
        ColorSetting();
    }

    public void SingleCapture()
    {
        StartCoroutine(CaptureImage());
    }

    public void MultiCapture()
    {
        StartCoroutine(AllCapture());
    }

    private IEnumerator CaptureImage()
    {
        yield return null;

        Texture2D tex = new Texture2D(m_RenderTexture.width, m_RenderTexture.height,TextureFormat.ARGB32,false,transform);
        RenderTexture.active = m_RenderTexture;
        tex.ReadPixels(new Rect(0, 0, m_RenderTexture.width, m_RenderTexture.height),0,0);

        yield return null;

        byte[] data = tex.EncodeToPNG();
        string name = "Thumbnail";
        string extention = ".png";
        string path = Application.persistentDataPath + "/Thumbnail/";

        Debug.Log(path);

        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        File.WriteAllBytes(path + name + extention, data);

        yield return null;
    }

    private IEnumerator AllCapture()
    {
        GameObject currentObj;
        while (cnt < m_CapturingGameObject.Length)
        {
            currentObj = Instantiate(m_CapturingGameObject[cnt].gameObject);
            currentObj.transform.position = m_ObjPos;
            currentObj.transform.rotation = Quaternion.Euler(m_ObjRot);

            yield return null;

            Texture2D tex = new Texture2D(m_RenderTexture.width, m_RenderTexture.height, TextureFormat.ARGB32, false, transform);
            RenderTexture.active = m_RenderTexture;
            tex.ReadPixels(new Rect(0, 0, m_RenderTexture.width, m_RenderTexture.height), 0, 0);

            yield return null;

            byte[] data = tex.EncodeToPNG();
            string name = $"Thumbnail_{m_CapturingGameObject[cnt].name}";
            string extention = ".png";
            string path = Application.persistentDataPath + "/Thumbnail/";

            Debug.Log(path);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            File.WriteAllBytes(path + name + extention, data);

            yield return null;
            DestroyImmediate(currentObj);

            cnt++;
            yield return null;
        }
    }

    public void ColorSetting()
    {
        m_Camera.backgroundColor = m_BackColor;
    }
}
