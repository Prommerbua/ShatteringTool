using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DelaunayControl))]
public class DelaunayControlInspector : Editor {
    DelaunayControl script;
	// Use this for initialization
	void Awake () {
        script = (DelaunayControl)target;
	}

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (GUILayout.Button("Draw triangulation"))
        {
            script.drawTriangulation();
        }

        if (GUILayout.Button("Draw voronoi"))
        {
            script.drawVoronoi();
        }

        if (GUILayout.Button("Create objects"))
        {
            script.createCells();
        }
    }
}
