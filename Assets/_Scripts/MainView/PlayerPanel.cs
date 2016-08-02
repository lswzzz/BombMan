using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour {
    public string playerName;
    public int fd;
    public Text nameText;
    public void setData(string name, int fd)
    {
        Start();
        nameText.text = name;
        this.playerName = name;
        this.fd = fd;
    }

	void Awake(){
	
	}
	
	// Use this for initialization
	void Start () {
        nameText = transform.Find("name").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate(){
	
	}
	
	void LateUpdate(){
	
	}
}
