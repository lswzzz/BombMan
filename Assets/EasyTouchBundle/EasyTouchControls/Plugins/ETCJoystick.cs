/***********************************************
				EasyTouch Controls
	Copyright © 2014-2015 The Hedgehog Team
  http://www.blitz3dfr.com/teamtalk/index.php
		
	  The.Hedgehog.Team@gmail.com
		
**********************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//针对源码作如下改动
//1 dynamic状态下如果没有点击就初始化为原来的位置
//2 dynamic状态下如果有点击就根据点击的区域移动摇杆
//3 dynamic状态下保证摇杆不超出可见范围
[System.Serializable]
public class ETCJoystick : ETCBase,IPointerEnterHandler,IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler {
		
	#region Unity Events
	[System.Serializable] public class OnMoveStartHandler : UnityEvent{}
	[System.Serializable] public class OnMoveSpeedHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveHandler : UnityEvent<Vector2> { }
	[System.Serializable] public class OnMoveEndHandler : UnityEvent{ }

	[System.Serializable] public class OnTouchStartHandler : UnityEvent{}
	[System.Serializable] public class OnTouchUpHandler : UnityEvent{ }

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
	[SerializeField] public OnTouchUpHandler onTouchUp;

	[SerializeField] public OnDownUpHandler OnDownUp;
	[SerializeField] public OnDownDownHandler OnDownDown;
	[SerializeField] public OnDownLeftHandler OnDownLeft;
	[SerializeField] public OnDownRightHandler OnDownRight;

	[SerializeField] public OnDownUpHandler OnPressUp;
	[SerializeField] public OnDownDownHandler OnPressDown;
	[SerializeField] public OnDownLeftHandler OnPressLeft;
	[SerializeField] public OnDownRightHandler OnPressRight;
	#endregion

	#region Enumeration
	public enum JoystickArea { UserDefined,FullScreen, Left,Right,Top, Bottom, TopLeft, TopRight, BottomLeft, BottomRight};
	public enum JoystickType {Dynamic, Static};
	public enum RadiusBase {Width, Height};
	#endregion

	#region Members

	#region Public members
	public JoystickType joystickType;
	public bool allowJoystickOverTouchPad;
	public RadiusBase radiusBase;
	public ETCAxis axisX;
	public ETCAxis axisY;
	public RectTransform thumb;
	
	public JoystickArea joystickArea;
	public RectTransform userArea;
    public Vector2 prePosition;
    #endregion

    #region Private members
    private Vector2 thumbPosition;
	private bool isDynamicActif;
	private Vector2 tmpAxis;
	private Vector2 OldTmpAxis;
	private bool isOnTouch;
    private PlayerDirection tmpDirection;
    private PlayerDirection oldDirection;
    private int curTmpCount;
    private int tmpCount;
    //原始的位置
    private Vector2 sourceAnchoredPosition;
	#endregion

	#region Inspector


	#endregion

	#endregion
	
	#region Constructor
	public ETCJoystick(){
		joystickType = JoystickType.Static;
		allowJoystickOverTouchPad = false;
		radiusBase = RadiusBase.Width;

		axisX = new ETCAxis("Horizontal");
		axisY = new ETCAxis("Vertical");

        _activated = true;

		joystickArea = JoystickArea.FullScreen;

		isDynamicActif = false;
		isOnDrag = false;
		isOnTouch = false;

		axisX.positivekey = KeyCode.RightArrow;
		axisX.negativeKey = KeyCode.LeftArrow;

		axisY.positivekey = KeyCode.UpArrow;
		axisY.negativeKey = KeyCode.DownArrow;

		enableKeySimulation = true;

		showPSInspector = true;
		showAxesInspector = false;
		showEventInspector = false;
		showSpriteInspector = false;
	}
	#endregion

	#region Monobehaviours Callback
	protected override void Awake (){
		base.Awake ();
        if (joystickType == JoystickType.Dynamic){
			this.rectTransform().anchorMin = new Vector2(0.5f,0.5f);
			this.rectTransform().anchorMax = new Vector2(0.5f,0.5f);
			this.rectTransform().SetAsLastSibling();
		}
	}

	void Start(){
        
        tmpAxis = Vector2.zero;
		OldTmpAxis = Vector2.zero;
        curTmpCount = 0;
        tmpCount = 2;
        oldDirection = PlayerDirection.Down;
        tmpDirection = PlayerDirection.Down;

        axisX.InitAxis();
		axisY.InitAxis();
        sourceAnchoredPosition = cachedRectTransform.anchoredPosition;
    }


	protected override void UpdateControlState ()
	{
		UpdateJoystick();
	}

	#endregion
	
	#region UI Callback
	public void OnPointerEnter(PointerEventData eventData){


        if (joystickType == JoystickType.Dynamic && !isDynamicActif && _activated){
			eventData.pointerDrag = gameObject;
			eventData.pointerPress = gameObject;

			isDynamicActif = true;
		}

		if (joystickType == JoystickType.Dynamic &&  !eventData.eligibleForClick){
			OnPointerUp( eventData );
		}

	}

	public void OnPointerDown(PointerEventData eventData){
		onTouchStart.Invoke();
	}

	public void OnBeginDrag(PointerEventData eventData){
        tmpAxis = Vector2.zero;
        prePosition = eventData.position;
    }

	public void OnDrag(PointerEventData eventData){

		isOnDrag = true;
		isOnTouch = true;
        
		float radius =  GetRadius();

        if (joystickType == JoystickType.Dynamic && _activated)
        {
            Vector2 localPosition = Vector2.zero;
            Vector2 screenPosition = Vector2.zero;
            UpdateJoystickPosition(ref localPosition, ref screenPosition);
        }

        Vector2 sourcePosition = GetComponent<RectTransform>().position;
        //touchPosition = eventData.position;
        thumbPosition =  (eventData.position - sourcePosition) / cachedRootCanvas.rectTransform().localScale.x;

        resetTmpAxis(eventData);

        thumbPosition.x = Mathf.FloorToInt( thumbPosition.x);
		thumbPosition.y = Mathf.FloorToInt( thumbPosition.y);
        
		if (!axisX.enable){
			thumbPosition.x=0;
		}

		if (!axisY.enable){
			thumbPosition.y=0;
		}

		if (thumbPosition.magnitude > radius){

			thumbPosition = thumbPosition.normalized * radius;
		}

		thumb.anchoredPosition =  thumbPosition; 

	}

    public void resetTmpAxis(PlayerDirection direction)
    {
        switch (direction)
        {
            case PlayerDirection.Down:
                tmpAxis.x = 0f;
                tmpAxis.y = -1f;
                break;
            case PlayerDirection.Up:
                tmpAxis.x = 0f;
                tmpAxis.y = 1f;
                break;
            case PlayerDirection.Left:
                tmpAxis.x = -1f;
                tmpAxis.y = 0f;
                break;
            case PlayerDirection.Right:
                tmpAxis.x = 1f;
                tmpAxis.y = 0f;
                break;
        }
    }

    public void TryDirection(PlayerDirection direction)
    {
        if(direction == oldDirection)
        {
            resetTmpAxis(direction);
        }
        else if(direction == tmpDirection)
        {
            curTmpCount++;
            if(curTmpCount >= tmpCount)
            {
                oldDirection = tmpDirection;
                resetTmpAxis(oldDirection);
            }
        }
        else
        {
            tmpDirection = direction;
            curTmpCount = 1;
        }
    }

    public void resetTmpAxis(PointerEventData eventData)
    {
        Vector2 deltaDis = eventData.position - prePosition;
        if (Mathf.Abs(deltaDis.x) < 2f && Mathf.Abs(deltaDis.y) < 2f) return;
        //if (deltaDis.magnitude < 2) return;
        deltaDis.Normalize();
        float angle = Mathf.Rad2Deg * Mathf.Atan2(deltaDis.y, deltaDis.x);
        if (angle >= -35f && angle < 35f)
        {
            //right
            tmpAxis.x = 1f;
            tmpAxis.y = 0f;
            //TryDirection(PlayerDirection.Right);
            Vector2 savePosition = prePosition;
            prePosition = eventData.position;
            //Debug.Log("position:" + eventData.position + "   prePosition:" + savePosition);
        }
        else if (angle >= 55f && angle < 125f)
        {
            //up
            tmpAxis.x = 0f;
            tmpAxis.y = 1f;
            //TryDirection(PlayerDirection.Up);
            Vector2 savePosition = prePosition;
            prePosition = eventData.position;
            //Debug.Log("position:" + eventData.position + "   prePosition:" + savePosition);
        }
        else if (angle < -55f && angle >= -125f)
        {
            //down
            tmpAxis.x = 0f;
            tmpAxis.y = -1f;
            //TryDirection(PlayerDirection.Down);
            Vector2 savePosition = prePosition;
            prePosition = eventData.position;
            //Debug.Log("position:" + eventData.position + "   prePosition:" + savePosition);
        }
        else if(angle < -145f || angle > 145f)
        {
            //left
            tmpAxis.x = -1f;
            tmpAxis.y = 0f;
            //TryDirection(PlayerDirection.Left);
            Vector2 savePosition = prePosition;
            prePosition = eventData.position;
            //Debug.Log("position:" + eventData.position + "   prePosition:" + savePosition);
        }
    }

    //public void resetInDirection(int direction)
    //{
    //    Vector2 screenPosition = touchPosition;
    //    Vector2 position = GetComponent<RectTransform>().position;
    //    Vector2 offset = screenPosition - position;
    //    switch (direction)
    //    {
    //        case 0:
    //            //left
    //            screenPosition.x = position.x + (GetRadius() + offset.x);
    //            break;
    //        case 1:
    //            //right
    //            screenPosition.x = position.x - (GetRadius() - offset.x);
    //            break;
    //        case 2:
    //            //Up
    //            screenPosition.y = position.y - (GetRadius() - offset.y);
    //            break;
    //        case 3:
    //            //down
    //            screenPosition.y = position.y + (GetRadius() + offset.y);
    //            break;
    //    }
    //    Vector2 localPosition = Vector2.zero;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(cachedRootCanvas.rectTransform(), screenPosition, cachedRootCanvas.worldCamera, out localPosition);
    //    cachedRectTransform.anchoredPosition = localPosition;
    //    if(localPosition == Vector2.zero)
    //    {
    //        Debug.Log("ZERO");
    //    }
    //}

	public void OnPointerUp (PointerEventData eventData){

		isOnDrag = false;
		isOnTouch = false;
		thumbPosition =  Vector2.zero;
		thumb.anchoredPosition = Vector2.zero;

		axisX.axisState = ETCAxis.AxisState.None;
		axisY.axisState = ETCAxis.AxisState.None;
	
		if (!axisX.isEnertia && !axisY.isEnertia){
			axisX.ResetAxis();
			axisY.ResetAxis();
			tmpAxis = Vector2.zero;
			OldTmpAxis = Vector2.zero;
			onMoveEnd.Invoke();
		}

		if (joystickType == JoystickType.Dynamic){
            resetSourcePosition();
			isDynamicActif = false;
		}

		onTouchUp.Invoke();

	}

    #endregion

    private void resetSourcePosition()
    {
        cachedRectTransform.anchoredPosition = sourceAnchoredPosition;
    }

    private Rect getRect()
    {
        Vector2 position = cachedRectTransform.anchoredPosition;
        Rect rect = new Rect(position.x - cachedRectTransform.rect.width / 2, position.y - cachedRectTransform.rect.height / 2,
                            cachedRectTransform.rect.width, cachedRectTransform.rect.height);
        return rect;
    }

    
    private bool UpdateJoystickPosition(ref Vector2 localPosition, ref Vector2 screenPosition)
    {
        if (isTouchOverJoystickArea(ref localPosition, ref screenPosition))
        {
            Rect rect = getRect();
            Vector2 offset = Vector2.zero;
            if (!rect.Contains(localPosition))
            {
                if(rect.xMin > localPosition.x)
                {
                    offset.x = localPosition.x - rect.xMin;
                }else if(rect.xMax < localPosition.x)
                {
                    offset.x = localPosition.x - rect.xMax;
                }
                if (rect.yMin > localPosition.y)
                {
                    offset.y = localPosition.y - rect.yMin;
                }
                else if (rect.yMax < localPosition.y)
                {
                    offset.y = localPosition.y - rect.yMax;
                }
                cachedRectTransform.anchoredPosition += offset;
            }
        }
        return false;
    }

    
    public void updateSimulation(float h, float v)
    {
        float radius = GetRadius();
        thumbPosition.x = radius * h;
        thumbPosition.y = radius * v;
        thumbPosition.x = Mathf.FloorToInt(thumbPosition.x);
        thumbPosition.y = Mathf.FloorToInt(thumbPosition.y);
        if (thumbPosition.magnitude > radius)
        {
            thumbPosition = thumbPosition.normalized * radius;
        }
        thumb.anchoredPosition = thumbPosition;
    }

	#region Joystick Update
	private void UpdateJoystick(){

		#region dynamic joystick
		if (joystickType == JoystickType.Dynamic && _activated){
			Vector2 localPosition = Vector2.zero;
			Vector2 screenPosition = Vector2.zero;

			if (isTouchOverJoystickArea(ref localPosition, ref screenPosition)){

				GameObject overGO = GetFirstUIElement( screenPosition);
				if (overGO == null || (allowJoystickOverTouchPad && overGO.GetComponent<ETCTouchPad>()) || (overGO != null && overGO.GetComponent<ETCArea>() ) ) {
					cachedRectTransform.anchoredPosition = localPosition;
					visible = true;
				}
			}
		}
		#endregion

		#region Key simulation
		if (enableKeySimulation && !isOnTouch && _activated){
			thumb.localPosition = Vector2.zero;
			isOnDrag = false;

			if (Input.GetKey( axisX.positivekey)){
				isOnDrag = true;
				thumb.localPosition = new Vector2(GetRadius(), thumb.localPosition.y);
			}
			else if (Input.GetKey( axisX.negativeKey)){
				isOnDrag = true;
				thumb.localPosition = new Vector2(-GetRadius(), thumb.localPosition.y);
			}

			if (Input.GetKey( axisY.positivekey)){
				isOnDrag = true;
				thumb.localPosition = new Vector2(thumb.localPosition.x,GetRadius() );
			}
			else if (Input.GetKey( axisY.negativeKey)){
				isOnDrag = true;
				thumb.localPosition = new Vector2(thumb.localPosition.x,-GetRadius());
			}

			thumbPosition = thumb.localPosition;
		}
		#endregion

		// Computejoystick value
		OldTmpAxis.x = axisX.axisValue;
		OldTmpAxis.y = axisY.axisValue;
		//tmpAxis = thumbPosition / GetRadius();

		axisX.UpdateAxis( tmpAxis.x,isOnDrag, ETCBase.ControlType.Joystick,true);
		axisY.UpdateAxis( tmpAxis.y,isOnDrag, ETCBase.ControlType.Joystick,true);

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
		if (Mathf.Abs(OldTmpAxis.x)< axisX.axisThreshold && Mathf.Abs(axisX.axisValue)>=axisX.axisThreshold){

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
		if (Mathf.Abs(OldTmpAxis.y)< axisY.axisThreshold && Mathf.Abs(axisY.axisValue)>=axisY.axisThreshold  ){
			
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

	#region Touch manager
	private bool isTouchOverJoystickArea(ref Vector2 localPosition, ref Vector2 screenPosition){
		
		bool touchOverArea = false;
		bool doTest = false;
		screenPosition = Vector2.zero;
		
		int count = GetTouchCount();

        int i=0;
		while (i<count && !touchOverArea){
            #if ((UNITY_ANDROID || UNITY_IPHONE || UNITY_WINRT || UNITY_BLACKBERRY) && !UNITY_EDITOR)
			if (Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Moved){
				screenPosition = Input.GetTouch(i).position;
				doTest = true;
			}
            #else
            if (Input.GetMouseButton(0)){
				screenPosition = Input.mousePosition;
				doTest = true;
			}
			#endif
			
			if (doTest && isScreenPointOverArea(screenPosition, ref localPosition) ){
				touchOverArea = true;
			}
			
			i++;
		}
		
		return touchOverArea;
	}
	
	private bool isScreenPointOverArea(Vector2 screenPosition, ref Vector2 localPosition){
		
		bool returnValue = false;
		
		if (joystickArea != JoystickArea.UserDefined){
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRootCanvas.rectTransform(),screenPosition,null,out localPosition)){
				
				switch (joystickArea){
				case JoystickArea.Left:
					if (localPosition.x<0){
						returnValue=true;
					}
					break;
					
				case JoystickArea.Right:
					if (localPosition.x>0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.FullScreen:
					returnValue = true;
					break;
					
				case JoystickArea.TopLeft:
					if (localPosition.y>0 && localPosition.x<0){
						returnValue = true;
					}
					break;
				case JoystickArea.Top:
					if (localPosition.y>0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.TopRight:
					if (localPosition.y>0 && localPosition.x>0){
						returnValue=true;
					}
					break;
					
				case JoystickArea.BottomLeft:
					if (localPosition.y<0 && localPosition.x<0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.Bottom:
					if (localPosition.y<0){
						returnValue = true;
					}
					break;
					
				case JoystickArea.BottomRight:
					if (localPosition.y<0 && localPosition.x>0){
						returnValue = true;
					}
					break;
				}
			}
		}
		else{
			if (RectTransformUtility.RectangleContainsScreenPoint( userArea, screenPosition, cachedRootCanvas.worldCamera )){
				RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRootCanvas.rectTransform(),screenPosition,cachedRootCanvas.worldCamera,out localPosition);
				returnValue = true;
			}
		}
		
		return returnValue;
		
	}
	
	private int GetTouchCount(){
		#if ((UNITY_ANDROID || UNITY_IPHONE || UNITY_WINRT || UNITY_BLACKBERRY) && !UNITY_EDITOR) 
		return Input.touchCount;
		#else
		if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)){
			return 1;
		}
		else{
			return 0;
		}
		#endif
	}
	#endregion

	#region Other private method
	private float GetRadius(){
		
		float radius =0;
		
		switch (radiusBase){
		case RadiusBase.Width:
			radius = cachedRectTransform.sizeDelta.x * 0.5f;
			break;
		case RadiusBase.Height:
			radius = cachedRectTransform.sizeDelta.y * 0.5f;
			break;
		}
		
		return radius;
	}

	protected override void SetActivated (){
        GetComponent<CanvasGroup>().blocksRaycasts = _activated;
    }

	protected override void SetVisible (){

		GetComponent<Image>().enabled = true;
		thumb.GetComponent<Image>().enabled = true;
		GetComponent<CanvasGroup>().blocksRaycasts = _activated;
	}
	#endregion
	
}

