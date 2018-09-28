using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
public class ButtonLikeLabel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/

public class TestLabelLikeButton : EditorWindow
{
    [MenuItem("Test/Label Like Button")]
    public static void St()
    {
        var win = GetWindow<TestLabelLikeButton>();
        win.Show();
    }
    private void OnGUI()
    {
        GUILayout.Button("Btn1", GetBtnStyle());
    }
    GUIStyle GetBtnStyle()
    {
        var s = new GUIStyle();
        var b = s.border;
        b.left = 10;
        b.top = 0;
        b.right = 0;
        b.bottom = 0;
        return s;
    }
}