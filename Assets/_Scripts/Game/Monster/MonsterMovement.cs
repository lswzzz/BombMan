using UnityEngine;
using System.Collections;

public class MonsterMovement : PlayerMovement
{
	
	// Use this for initialization
	void Start () {
        speed = 200f;
        maxSpeed = 300f;
        boxUpdate = new MonsterBoxUpdate();
        boxUpdate.Init(transform, GetComponent<MonsterColliderBox>(), GetComponent<MonsterBeHurtBox>(), GetComponent<MonsterExploreBox>());
        colliderBox = GetComponent<PlayerColliderBox>();
        behurtBox = GetComponent<PlayerBeHurtBox>();
        animatoController = GetComponent<PlayerAnimationController>();
        resetPlayerZ();
    }
	
	// Update is called once per frame
}
