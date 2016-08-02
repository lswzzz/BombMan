using UnityEngine;
using System.Collections;

public class AABBox : MonoBehaviour{
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public Vector2 min, max;
    
    void Start()
    {

    }

    public void resetAABBox(Vector2 min, Vector2 max)
    {
        this.min = min;
        this.max = max;
    }

    public void resetAABBox(Vector2 center, float width, float height)
    {
        min = new Vector2(center.x - width / 2, center.y - height / 2);
        max = new Vector2(center.x + width / 2, center.y + height / 2);
    }

    public bool intersects(AABBox aabb)
    {
        if (min.x > aabb.max.x || max.x < aabb.min.x) return false;
        if (min.y > aabb.max.y || max.y < aabb.min.y) return false;
        return true;
    }

    public bool BoxInline(AABBox aabb)
    {
        if (min.x >= aabb.max.x || max.x <= aabb.min.x) return false;
        if(min.y >= aabb.max.y || max.y <= aabb.min.y)return false;
        return true;
    }
    public float Width {
        get { return max.x - min.x; }
    }

    public float Height {
        get { return max.y - min.y; }
    }

    public Vector2 Center
    {
        get { return new Vector2((max.x + min.x)/2, (max.y + min.y) /2); }
    }

    public Vector2 Min
    {
        get { return min; }
    }

    public Vector2 Max
    {
        get { return max; }
    }

    //显示的是世界坐标系
    public virtual void OnDrawGizmos()
    {
        
        // 设置颜色
        Color defaultColor = Gizmos.color;
        Gizmos.color = Color.blue;

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
