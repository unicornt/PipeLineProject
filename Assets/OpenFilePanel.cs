using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEditor;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class FileDlg
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileDlg : FileDlg
{

}
public class OpenFileDialog
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileDlg ofd);
}

public class OpenFilePanel : MonoBehaviour
{
    public static string _lastDir = "";
    static public string OpenFile()
    {
        string filepath = "";
        #if UNITY_EDITOR
            filepath = EditorUtility.OpenFilePanel("Open File", _lastDir, "yo");
            _lastDir = filepath;
        #else
            OpenFileDlg pth = new OpenFileDlg();
            pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
            pth.filter = "yo (*.yo)";
            pth.file = new string(new char[256]);
            pth.maxFile = pth.file.Length;
            pth.fileTitle = new string(new char[64]);
            pth.maxFileTitle = pth.fileTitle.Length;
            pth.initialDir = _lastDir;  // default path  
            pth.title = "打开文件";
            pth.defExt = "yo";
            pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
            if (OpenFileDialog.GetOpenFileName(pth))
            {
                filepath = pth.file;//选择的文件路径;  
                _lastDir = filepath;
            }
        #endif
        return filepath;
    }

    private void Awake()
    {
        #if UNITY_EDITOR
        _lastDir = "/Desktop/ics/pj/";
        #else
        _lastDir = Application.dataPath;
        #endif
    }

    public Text text;
    public void ReadFile()
    {
        string filepath = OpenFile();
        for (int i = filepath.Length - 1; i >= 0; i--)
        {
            if (filepath[i] == '/')
            {
                text.text = filepath.Substring(i + 1);
                break;
            }
        }
        conduct.Work.Init();
        load.Work.Gao(filepath);
    }
}