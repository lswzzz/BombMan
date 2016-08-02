using UnityEngine;
using System.Collections;

public class PlayerBeHurtBox : AABBox {

    private PlayerAnimationController animationController;
    private float leftoff = 15;
    private float rightoff = 15;
    private float downoff = 20;
    private float upoff = 10;

   

    // Use this for initialization
    void Start () {
        leftoff = 15;
        rightoff = 15;
        downoff = 20;
        upoff = 10;
        animationController = GetComponent<PlayerAnimationController>();
       
    }

    public float getTransformPosX(float centerX)
    {
        return (2 * centerX + leftoff - rightoff) / 2;
    }

    public float getTransformPosY(float centerY)
    {
        return (2 * centerY + downoff - upoff) / 2;
    }

    public Vector2 getTransformPos(Vector2 center)
    {
        return new Vector2(getTransformPosX(center.x), getTransformPosY(center.y));
    }

    public void BeHurtBoxReset()
    {
        Vector2 min = new Vector2(transform.position.x - leftoff, transform.position.y - downoff);
        Vector2 max = new Vector2(transform.position.x + rightoff, transform.position.y + upoff);
        resetAABBox(min, max);
    }

	// Update is called once per frame
	void Update () {
        
    }
	
	void FixedUpdate(){
	
	}
	
	void LateUpdate(){
	
	}

    public override void OnDrawGizmos()
    {

        Vector2 min = new Vector2(transform.position.x - leftoff, transform.position.y - downoff);
        Vector2 max = new Vector2(transform.position.x + rightoff, transform.position.y + upoff);
        // 设置颜色
        Color defaultColor = Gizmos.color;
        Gizmos.color = Color.red;

        Vector3 point1 = new Vector3(min.x, min.y, -10);
        Vector3 point2 = new Vector3(max.x, min.y, -10);
        Vector3 point3 = new Vector3(max.x, max.y, -10);
        Vector3 point4 = new Vector3(min.x, max.y, -10);

        // 绘制最后一条线段
        Gizmos.DrawLine(point1, point2);
        Gizmos.DrawLine(point2, point3);
        Gizmos.DrawLine(point3, point4);
        Gizmos.DrawLine(point4, point1);

        // 恢复默认颜色
        Gizmos.color = defaultColor;
    }
}
