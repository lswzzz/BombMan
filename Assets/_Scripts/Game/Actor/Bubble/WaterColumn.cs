using UnityEngine;
using System.Collections;

public class WaterColumn : MonoBehaviour {

    public enum WaterType {
        UpMid,
        RightMid,
        DownMid,
        LeftMid,
        UpTop,
        RightTop,
        DownTop,
        LeftTop,
        Center,
    }

    private Animator animator;
    public WaterType type;
    public int col, row;
    public float createTime;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        animator.SetInteger("Type", (int)type);
        animator.SetTrigger("Change");
        AABBox box = GetComponent<AABBox>();
        Vector2 min = new Vector2(transform.position.x - 20, transform.position.y - 20);
        Vector2 max = new Vector2(transform.position.x + 20, transform.position.y + 20);
        box.resetAABBox(min, max);
        createTime = TimeUtils.GetTimeStampNow();
    }
	
    public void setType(WaterType type)
    {
        this.type = type;
    }

    public void BeDestory()
    {
        if(GameConst.waterColumns[row, col] != null && GameConst.waterColumns[row, col].createTime == createTime)
        {
            GameConst.waterColumns[row, col] = null;
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
	void FixedUpdate(){
	
	}
	
	void LateUpdate(){
	
	}
}
