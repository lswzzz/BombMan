using UnityEngine;
using System.Collections;

//为了兼容大小不等于一个块的图片将锚点的位置修改为bottom left
public class Block : MonoBehaviour {

    private AABBox aabbox;
    private int width, height;
    private float fade;
    private SpriteRenderer m_renderer;
    
    void Awake(){
	
	}
	
	// Use this for initialization
	void Start () {
        m_renderer = GetComponent<SpriteRenderer>();
        fade = 1.0f;
        aabbox = GetComponent<AABBox>();
        width = (int)aabbox.Center.x / GameConst.tileWidth;
        height = (int)-aabbox.Center.y / GameConst.tileHeight;
    }
	
    public void InitBlockTransform(Vector3 pos, int width, int height)
    {

        transform.position = pos;
        this.width = width;
        this.height = height;
        aabbox = GetComponent<AABBox>();
        aabbox.resetAABBox(new Vector2(pos.x, pos.y), new Vector2(pos.x + width, pos.y + height));
    }

	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate(){
	
	}
	
    public void BeDestory()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        fade = fade - Time.deltaTime;
        m_renderer.color = new Color(m_renderer.color.r, m_renderer.color.g, m_renderer.color.b, fade);
        yield return null;
        if(fade > 0f)
        {
            yield return StartCoroutine(FadeOut());
        }
        else
        {
            GameConst.sceneObstacles[height, width] = null;
            Destroy(gameObject);
        }

    }

}
