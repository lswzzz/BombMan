﻿/***********************************************
				EasyTouch Controls
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ETCDPad : ETCBase, IDragHandler,  IPointerDownHandler, IPointerUpHandler { 

	#region Unity Events
	[System.Serializable] public class OnMoveStartHandler : UnityEvent{}
	[System.Serializable] public class OnMoveHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveSpeedHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveEndHandler : UnityEvent{ }

	[System.Serializable] public class OnTouchStartHandler : UnityEvent{}
	[System.Serializable] public class OnTouchUPHandler : UnityEvent{}

	[System.Serializable] public class OnDownUpHandler : UnityEvent{ }
	[System.Serializable] public class OnDownDownHandler : UnityEvent{ }
	[System.Serializable] public class OnDownLeftHandler : UnityEvent{ }
	[System.Serializable] public class OnDownRightHandler : UnityEvent{ }
	
	[System.Serializable] public class OnPressUpHandler : UnityEvent{ }
	[System.Serializable] public class OnPressDownHandler : UnityEvent{ }
	[System.Serializable] public class OnPressLeftHandler : UnityEvent{ }
	[System.Serializable] public class OnPressRightHandler : UnityEvent{ }

	[SerializeField] public OnMoveStartHandler onMoveStart;
	[SerializeField] public OnMoveHandler onMove;
	[SerializeField] public OnMoveSpeedHandler onMoveSpeed;
	[SerializeField] public OnMoveEndHandler onMoveEnd;

	[SerializeField] public OnTouchStartHandler onTouchStart;
	[SerializeField] public OnTouchUPHandler onTouchUp;


	[SerializeField] public OnDownUpHandler OnDownUp;
	[SerializeField] public OnDownDownHandler OnDownDown;
	[SerializeField] public OnDownLeftHandler OnDownLeft;
	[SerializeField] public OnDownRightHandler OnDownRight;
	
	[SerializeField] public OnDownUpHandler OnPressUp;
	[SerializeField] public OnDownDownHandler OnPressDown;
	[SerializeField] public OnDownLeftHandler OnPressLeft;
	[SerializeField] public OnDownRightHandler OnPressRight;
	#endregion
	
	#region Members
	#region Public members
	public ETCAxis axisX;
	public ETCAxis axisY;
    public ETCAxis axisCenter;

	public Sprite normalSprite;
	public Color normalColor;

	public Sprite pressedSprite;
	public Color pressedColor;

    #endregion

    private class TouchInfo {
        public int touchId;
        public Vector2 point;
    }

	#region Private Member
	private Vector2 tmpAxis;
	private Vector2 OldTmpAxis;
	private bool isOnTouch;
    private List<TouchInfo> touchIds;
	#endregion

	#endregion
	
	#region Private members
	private Image cachedImage; 
	#endregion

	#region Constructor
	public ETCDPad(){
		
		axisX = new ETCAxis( "Horizontal");
		axisY = new ETCAxis( "Vertical");
        axisCenter = new ETCAxis("Center");

        _visible = true;
		_activated = true;

		dPadAxisCount = DPadAxis.Two_Axis;
		tmpAxis = Vector2.zero;

		showPSInspector = true; 
		showSpriteInspector = false;
		showBehaviourInspector = false;
		showEventInspector = false;

		isOnDrag = false;
		isOnTouch = false;
		
		axisX.positivekey = KeyCode.RightArrow;
		axisX.negativeKey = KeyCode.LeftArrow;

        axisCenter.positivekey = KeyCode.Space;

		axisY.positivekey = KeyCode.UpArrow;
		axisY.negativeKey = KeyCode.DownArrow;
		
		enableKeySimulation = true;
		#if !UNITY_EDITOR
		enableKeySimulation = false;
		#endif
	}
	#endregion

	#region Monobehaviour Callback
	void Start(){
		tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;
        touchIds = new List<TouchInfo>();

        axisX.InitAxis();
		axisY.InitAxis();
        axisCenter.InitAxis();
	}

    void Awake()
    {
        base.Awake();
        ETCInput.instance.RegisterAxis(axisCenter);
    }

    void OnDestroy()
    {
        base.OnDestroy();
        if (!isShuttingDown && !Application.isLoadingLevel)
        {
            ETCInput.instance.UnRegisterAxis(axisCenter);
        }
    }
	/*
	void Update(){
		
		if (!useFixedUpdate){
			UpdateDPad();
		}
	}
	
	void FixedUpdate(){
		if (useFixedUpdate){
			UpdateDPad();
		}
	}*/

	protected override void UpdateControlState ()
	{
		UpdateDPad();
        UpdateButton();

    }
	#endregion

    bool cantainsIntList(int id)
    {
        foreach(TouchInfo touchInfo in touchIds)
        {
            if (touchInfo.touchId == id)
            {
                return true;
            }
        }
        return false;
    }

    void removeId(int id)
    {
        foreach (TouchInfo touchInfo in touchIds)
        {
            if (touchInfo.touchId == id)
            {
                touchIds.Remove(touchInfo);
                return;
            }
        }
    }

    void addId(PointerEventData eventData)
    {
        TouchInfo info = new TouchInfo();
        info.touchId = eventData.pointerId;
        info.point = eventData.position;
        touchIds.Add(info);
    }

    void updateId(PointerEventData eventData)
    {
        foreach (TouchInfo touchInfo in touchIds)
        {
            if (touchInfo.touchId == eventData.pointerId)
            {
                touchInfo.point = eventData.position;
            }
        }
    }

	#region UI Callback
	public void OnPointerDown(PointerEventData eventData){
		if (_activated){
            if (!cantainsIntList(eventData.pointerId))
            {
                addId(eventData);
            }
			onTouchStart.Invoke();
            GetTouchDirectionDown( eventData.position,eventData.pressEventCamera);
			isOnTouch = true;
			isOnDrag = true;
		}
	}

	public void OnDrag(PointerEventData eventData){
		if (_activated){
			isOnTouch = true;
			isOnDrag = true;
            updateId(eventData);
            GetTouchDirectionDown( eventData.position,eventData.pressEventCamera);
		}
	}

	public void OnPointerUp(PointerEventData eventData){

		isOnTouch = false;
		isOnDrag = false;

		tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;

        removeId(eventData.pointerId);

        if(touchIds.Count > 0)
        {
            GetTouchDirectionDown(touchIds[touchIds.Count - 1].point, eventData.pressEventCamera);
        }

        GetTouchDirectionUp(eventData.position, eventData.pressEventCamera);

        if (touchIds.Count == 0)
        {
            axisX.axisState = ETCAxis.AxisState.None;
            axisY.axisState = ETCAxis.AxisState.None;

            if (!axisX.isEnertia && !axisY.isEnertia)
            {
                axisX.ResetAxis();
                axisY.ResetAxis();
                onMoveEnd.Invoke();
            }
        }

		onTouchUp.Invoke();

	}

	#endregion

	#region DPad Update
	void UpdateDPad(){

		#region Key simulation

		if (enableKeySimulation && !isOnTouch && _activated && _visible){
			isOnDrag = false;
			tmpAxis = Vector2.zero;

			if (Input.GetKey( axisX.positivekey)){
				isOnDrag = true;
				tmpAxis = new Vector2(1,tmpAxis.y);
			}
			else if (Input.GetKey( axisX.negativeKey)){
				isOnDrag = true;
				tmpAxis = new Vector2(-1,tmpAxis.y);
			}
			
			if (Input.GetKey( axisY.positivekey)){
				isOnDrag = true;
				tmpAxis = new Vector2(tmpAxis.x,1);

			}
			else if (Input.GetKey( axisY.negativeKey)){
				isOnDrag = true;
				tmpAxis = new Vector2(tmpAxis.x,-1);
			}
		}
		#endregion

		OldTmpAxis.x = axisX.axisValue;
		OldTmpAxis.y = axisY.axisValue;

		axisX.UpdateAxis( tmpAxis.x,isOnDrag,ETCBase.ControlType.DPad);
		axisY.UpdateAxis( tmpAxis.y,isOnDrag, ETCBase.ControlType.DPad);

		axisX.DoGravity();
		axisY.DoGravity();

		#region Move event
		if ((axisX.axisValue!=0 ||  axisY.axisValue!=0 ) && OldTmpAxis == Vector2.zero){
			onMoveStart.Invoke();
		}

		if (axisX.axisValue!=0 ||  axisY.axisValue!=0 ){
			
			// X axis
			if( axisX.actionOn == ETCAxis.ActionOn.Down && (axisX.axisState == ETCAxis.AxisState.DownLeft || axisX.axisState == ETCAxis.AxisState.DownRight)){
				axisX.DoDirectAction();
			}
			else if (axisX.actionOn == ETCAxis.ActionOn.Press){
				axisX.DoDirectAction();
			}
			
			// Y axis
			if( axisY.actionOn == ETCAxis.ActionOn.Down && (axisY.axisState == ETCAxis.AxisState.DownUp || axisY.axisState == ETCAxis.AxisState.DownDown)){
				axisY.DoDirectAction();
			}
			else if (axisY.actionOn == ETCAxis.ActionOn.Press){
				axisY.DoDirectAction();
			}
			onMove.Invoke( new Vector2(axisX.axisValue,axisY.axisValue));
			onMoveSpeed.Invoke( new Vector2(axisX.axisSpeedValue,axisY.axisSpeedValue));
		}
		else if (axisX.axisValue==0 &&  axisY.axisValue==0  && OldTmpAxis!=Vector2.zero) {
			onMoveEnd.Invoke();
		}
		#endregion
		
		#region Down & press event
		float coef =1;
		if (axisX.invertedAxis) coef = -1;
		if (OldTmpAxis.x == 0 && Mathf.Abs(axisX.axisValue)>0){


			if (axisX.axisValue*coef >0){
				axisX.axisState = ETCAxis.AxisState.DownRight;
				OnDownRight.Invoke();
			}
			else if (axisX.axisValue*coef<0){
				axisX.axisState = ETCAxis.AxisState.DownLeft;
				OnDownLeft.Invoke();
			}
			else{
				axisX.axisState = ETCAxis.AxisState.None;
			}
		}
		else if (axisX.axisState!= ETCAxis.AxisState.None) {
			if (axisX.axisValue*coef>0){
				axisX.axisState = ETCAxis.AxisState.PressRight;
				OnPressRight.Invoke();
			}
			else if (axisX.axisValue*coef<0){
				axisX.axisState = ETCAxis.AxisState.PressLeft;
				OnPressLeft.Invoke();
			}
			else{
				axisX.axisState = ETCAxis.AxisState.None;
			}
		}


		coef =1;
		if (axisY.invertedAxis) coef = -1;
		if (OldTmpAxis.y==0 && Mathf.Abs(axisY.axisValue)>0 ){
			
			if (axisY.axisValue*coef>0){
				axisY.axisState = ETCAxis.AxisState.DownUp;
				OnDownUp.Invoke();
			}
			else if (axisY.axisValue*coef<0){
				axisY.axisState = ETCAxis.AxisState.DownDown;
				OnDownDown.Invoke();
			}
			else{
				axisY.axisState = ETCAxis.AxisState.None;
			}
		}
		else if (axisY.axisState!= ETCAxis.AxisState.None) {
			if (axisY.axisValue*coef>0){
				axisY.axisState = ETCAxis.AxisState.PressUp;
				OnPressUp.Invoke();
			}
			else if (axisY.axisValue*coef<0){
				axisY.axisState = ETCAxis.AxisState.PressDown;
				OnPressDown.Invoke();
			}
			else{
				axisY.axisState = ETCAxis.AxisState.None;
			}
		}

        #endregion

    }

    #endregion

    #region Button Update
    private void UpdateButton()
    {

        if (axisCenter.axisState == ETCAxis.AxisState.Down)
        {
            axisCenter.axisState = ETCAxis.AxisState.Press;
            axisCenter.UpdateButton();
        }

        if (axisCenter.axisState == ETCAxis.AxisState.Up)
        {
            axisCenter.axisState = ETCAxis.AxisState.None;
        }


        //if (enableKeySimulation && _activated && _visible && !isOnTouch)
        //{


        //    if (Input.GetKey(axis.positivekey) && axis.axisState == ETCAxis.AxisState.None)
        //    {
        //        axis.axisState = ETCAxis.AxisState.Down;
        //    }

        //    if (!Input.GetKey(axis.positivekey) && axis.axisState == ETCAxis.AxisState.Press)
        //    {
        //        axis.axisState = ETCAxis.AxisState.Up;
        //        onUp.Invoke();
        //    }
        //}

    }
    #endregion

    #region Private methods
    protected override void SetVisible (){
		GetComponent<Image>().enabled = _visible;
	}

    private void GetTouchDirectionUp(Vector2 position, Camera cam)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedRectTransform, position, cam, out localPoint);

        Vector2 buttonSize = this.rectTransform().sizeDelta / 3f;

        if (localPoint.y >= -buttonSize.y / 2f && localPoint.y <= buttonSize.y / 2f && localPoint.x >= -buttonSize.x / 2f && localPoint.x <= buttonSize.x / 2f)
        {
            axisCenter.axisState = ETCAxis.AxisState.Up;
            axisCenter.axisValue = 0;
        }
    }

	private void GetTouchDirectionDown(Vector2 position, Camera cam){

		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRectTransform,position,cam,out localPoint);

		Vector2 buttonSize = this.rectTransform().sizeDelta / 3f;
	    
		tmpAxis = Vector2.zero;

        if (localPoint.y >= -buttonSize.y / 2f && localPoint.y <= buttonSize.y / 2f && localPoint.x >= -buttonSize.x / 2f && localPoint.x <= buttonSize.x / 2f)
        {
            axisCenter.ResetAxis();
            axisCenter.axisState = ETCAxis.AxisState.Down;
        }

        Vector2 LeftUp = new Vector2(-this.rectTransform().sizeDelta.x / 2f, this.rectTransform().sizeDelta.y / 2f);
        Vector2 LeftDown = new Vector2(-this.rectTransform().sizeDelta.x / 2f, -this.rectTransform().sizeDelta.y / 2f);
        Vector2 RightUp = new Vector2(this.rectTransform().sizeDelta.x / 2f, this.rectTransform().sizeDelta.y / 2f);
        Vector2 RightDown = new Vector2(this.rectTransform().sizeDelta.x / 2f, -this.rectTransform().sizeDelta.y / 2f);

        if ((localPoint.x < -buttonSize.x / 2f && Vector2.Dot(localPoint, LeftUp)>0 && Vector2.Dot(localPoint, LeftDown) > 0 && dPadAxisCount == DPadAxis.Two_Axis)
                || (dPadAxisCount == DPadAxis.Four_Axis && localPoint.x < -buttonSize.x / 2f))
        {
            tmpAxis.x = -1;
        }

        // right
        if ((localPoint.x > buttonSize.x / 2f && Vector2.Dot(localPoint, RightUp) > 0 && Vector2.Dot(localPoint, RightDown) > 0 && dPadAxisCount == DPadAxis.Two_Axis)
            || (dPadAxisCount == DPadAxis.Four_Axis && localPoint.x > buttonSize.x / 2f))
        {
            tmpAxis.x = 1;
        }


        // Up
        if ((localPoint.y > buttonSize.y / 2f && Vector2.Dot(localPoint, LeftUp) > 0 && Vector2.Dot(localPoint, RightUp) > 0 && dPadAxisCount == DPadAxis.Two_Axis)
            || (dPadAxisCount == DPadAxis.Four_Axis && localPoint.y > buttonSize.y / 2f))
        {
            tmpAxis.y = 1;
        }


        // Down
        if ((localPoint.y < -buttonSize.y / 2f && Vector2.Dot(localPoint, LeftDown) > 0 && Vector2.Dot(localPoint, RightDown) > 0 && dPadAxisCount == DPadAxis.Two_Axis)
            || (dPadAxisCount == DPadAxis.Four_Axis && localPoint.y < -buttonSize.y / 2f))
        {
            tmpAxis.y = -1;
        }

        // Left
        //      if ( (localPoint.x < -buttonSize.x/2f && localPoint.y > -buttonSize.y/2f && localPoint.y< buttonSize.y/2f && dPadAxisCount== DPadAxis.Two_Axis) 
        //    || (dPadAxisCount== DPadAxis.Four_Axis &&  localPoint.x < -buttonSize.x/2f) ){
        //	tmpAxis.x = -1;
        //}

        //// right
        //if ( (localPoint.x > buttonSize.x/2f && localPoint.y> -buttonSize.y/2f && localPoint.y< buttonSize.y/2f && dPadAxisCount== DPadAxis.Two_Axis) 
        //	|| (dPadAxisCount== DPadAxis.Four_Axis &&  localPoint.x > buttonSize.x/2f) ){
        //	tmpAxis.x = 1;
        //}


        //// Up
        //if ( (localPoint.y > buttonSize.y/2f && localPoint.x>-buttonSize.x/2f && localPoint.x<buttonSize.x/2f && dPadAxisCount == DPadAxis.Two_Axis)
        //	|| (dPadAxisCount== DPadAxis.Four_Axis &&  localPoint.y > buttonSize.y/2f) ){
        //	tmpAxis.y = 1;
        //}


        //// Down
        //if ( (localPoint.y < -buttonSize.y/2f && localPoint.x>-buttonSize.x/2f && localPoint.x<buttonSize.x/2f && dPadAxisCount == DPadAxis.Two_Axis)
        //	|| (dPadAxisCount== DPadAxis.Four_Axis &&  localPoint.y <- buttonSize.y/2f) ){
        //	tmpAxis.y = -1;
        //}



    }
    #endregion
}
