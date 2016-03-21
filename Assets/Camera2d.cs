using UnityEngine;
using System.Collections;

public class Camera2d : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Camera>().orthographicSize = Screen.height * 0.5f;	
	}
}
