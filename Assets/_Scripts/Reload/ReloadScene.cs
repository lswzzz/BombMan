using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
        StartCoroutine(waitSecond());
    }
	
    IEnumerator waitSecond()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("scene1");
    }

	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate(){
	
	}
	
	void LateUpdate(){
	
	}
}
