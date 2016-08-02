using UnityEngine;
using System.Collections;

public class UICamera : MonoBehaviour {

    float devHeight = 9.6f;
    float devWidth = 6.4f;

    //http://www.cnblogs.com/flyFreeZn/p/4073655.html
    // Use this for initialization
    void Start () {
        float aaspectRotio = 2.0f / 3.0f;
        Camera camera = GetComponent<Camera>();
        float orthographicSize = camera.orthographicSize;
        Debug.Log(Screen.width + "   " + Screen.height); 
        float aspectRotio = Screen.width * 1.0f / Screen.height;
        float cameraWidth = orthographicSize * 2 * aspectRotio;
        Debug.Log(cameraWidth + "   " + devWidth);
        if(cameraWidth < devWidth)
        {
            orthographicSize = devWidth / (2 * aspectRotio);
            camera.orthographicSize = orthographicSize;
            Debug.Log(orthographicSize);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
