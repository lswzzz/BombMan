using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterBoxUpdate : PlayerBoxUpdate
{
    public void Init(Transform transform, MonsterColliderBox box1, MonsterBeHurtBox box2, MonsterExploreBox box3)
    {
        base.transform = transform;
        base.colliderBox = box1;
        base.behurtBox = box2;
        base.exploreBox = box3;
        resetColliderBox();
        resetBeHurtBox();
        resetExploreBox();
        movement = transform.GetComponent<MonsterMovement>();
        RelativeDitionary = new Dictionary<int, BubbleNode>();
    }


}
