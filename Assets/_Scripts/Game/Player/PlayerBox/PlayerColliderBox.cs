using UnityEngine;
using System.Collections;

public class PlayerColliderBox : AABBox {

    private PlayerAnimationController animationController;

    public float leftoff = 20;
    public float rightoff = 20;
    public float downoff = 25;
    public float upoff = 15;
	// Use this for initialization
	void Start () {
        leftoff = 20;
        rightoff = 20;
        downoff = 25;
        upoff = 15;
        animationController = GetComponent<PlayerAnimationController>();
	}
	
    public void ColliderBoxReset()
    {
        Vector2 min = new Vector2(transform.position.x - leftoff, transform.position.y - downoff);
        Vector2 max = new Vector2(transform.position.x + rightoff, transform.position.y + upoff);
        resetAABBox(min, max);
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

	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate(){
	
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
