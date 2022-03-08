using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshMakerNamespace;

public class RuntimeCSG : MonoBehaviour {
		
	// Use this for initialization
	void Start () {
		CSG.EPSILON = 1e-5f;
		CSG csg = new CSG();
		csg.Brush = GameObject.Find("Sphere");
		csg.Target = GameObject.Find("Cube");
		csg.OperationType = CSG.Operation.Subtract;		
		csg.customMaterial = new Material(Shader.Find("Standard"));
		csg.useCustomMaterial = false; 
		csg.hideGameObjects = true;
		csg.keepSubmeshes = true;
		GameObject newObject = csg.PerformCSG();
		Debug.Log(newObject.name);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}