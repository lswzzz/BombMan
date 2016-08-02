using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DelayText : MonoBehaviour {

    public Text text;

	void Awake(){
	
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        text.text = (TimeUtils.UdpSingleDelay * 1000).ToString();
	}
	
	void FixedUpdate(){
	
	}
	
	void LateUpdate(){
	
	}
}
