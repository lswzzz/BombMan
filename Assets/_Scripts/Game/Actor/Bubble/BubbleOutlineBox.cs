using UnityEngine;
using System.Collections;

public class BubbleOutlineBox : AABBox {

    private float leftoff = 20;
    private float rightoff = 20;
    private float downoff = 20;
    private float upoff = 20;

	void Start () {
        leftoff = 20;
        rightoff = 20;
        downoff = 20;
        upoff = 20;
    }
	
    public void resetBox()
    {
        Vector2 min = new Vector2(transform.position.x - leftoff, transform.position.y - downoff);
        Vector2 max = new Vector2(transform.position.x + rightoff, transform.position.y + upoff);
        resetAABBox(min, max);
    }

	// Update is called once per frame
	void Update () {
	
	}

    public override void OnDrawGizmos()
    {

        Vector2 min = new Vector2(transform.position.x - leftoff, transform.position.y - downoff);
        Vector2 max = new Vector2(transform.position.x + rightoff, transform.position.y + upoff);
        // 设置颜色
        Color defaultColor = Gizmos.color;
        Gizmos.color = Color.cyan;

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
