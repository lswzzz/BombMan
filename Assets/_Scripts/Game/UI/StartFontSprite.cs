using UnityEngine;
using System.Collections;

public class StartFontSprite : MonoBehaviour {

    public Sprite[] sprites;
    private float perTime;
    private int index;
    private float curTime;
    private float waitTime;
    public bool start;
    private float maxScale;
    private float minScale;
    private SpriteRenderer renderer;
	// Use this for initialization
	void Start () {
        perTime = 1f;
        curTime = 0f;
        waitTime = 1f;
        index = 0;
        start = false;
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprites[index];
        renderer.color = new Color(1f, 1f, 1f, 0f);
        maxScale = 4f;
        minScale = 1;
        StartCoroutine(Show());
    }
	
	// Update is called once per frame
	void Update () {
        if (start)
        {
            curTime += Time.deltaTime;
            if(curTime >= perTime)
            {
                curTime = 0f;
                if(index < sprites.Length - 1)
                {
                    ++index;
                    renderer.sprite = sprites[index];
                }
                else
                {
                    OK();
                }
            }
            FadeOut();
            ScaleSprite();
        }
	}

    IEnumerator Show()
    {
        yield return new WaitForSeconds(waitTime);
        renderer.color = new Color(1f, 1f, 1f, 1f);
        start = true;
    }

    void FadeOut()
    {
        float value = Mathf.Lerp(0, 1f, curTime);
        renderer.color = new Color(1f, 1f, 1f, 1f-value);
    }
	
    void ScaleSprite()
    {
        float value = Mathf.Lerp(minScale, maxScale, curTime);
        transform.localScale = new Vector3(value, value, 1);
    }

    public void OK()
    {
        GameGlobalData.player.GetComponent<PlayerBehaviourCollector>().logicSwap.StartNetSend = true;
        GameGlobalData.player.GetComponent<PlayerInputController>().startAccept = true;
        Destroy();
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
