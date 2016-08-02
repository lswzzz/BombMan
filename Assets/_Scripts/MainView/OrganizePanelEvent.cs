using UnityEngine;
using System.Collections;
using proto.serverproto;
using System.Collections.Generic;

public class OrganizePanelEvent : MonoBehaviour {

    public GameObject roomPrefab;

    public Transform content;
	void Awake(){
	
	}
	
	// Use this for initialization
	void Start () {
        
	}
	
    public void GenerateRoomList(GetRoomListResponse resp)
    {
        int childCount = resp.roomCount;
        ResetChild(childCount);
        List<SimpleRoomInfo> roomInfoList = resp.roomInfo;
        int index = 0;
        foreach(SimpleRoomInfo info in roomInfoList)
        {
            setRoomData(info, index);
            index++;
        }
    }

    void ResetChild(int count)
    {
        for(int i=0; i<content.childCount; i++)
        {
            content.GetChild(i).gameObject.SetActive(true);
        }
        int curChildCount = content.childCount;
        if(curChildCount < count)
        {
            int num = count - curChildCount;
            for(int i=0; i< num; i++)
            {
                Instantiate(roomPrefab).transform.parent = content;
            }
        }else if(curChildCount > count)
        {
            int num = curChildCount - count;
            for(int i=curChildCount-1; i > count - 1; i--)
            {
                content.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    void setRoomData(SimpleRoomInfo roomInfo, int index)
    {
        Transform child = content.GetChild(index);
        RoomData roomData = child.GetComponent<RoomData>();
        roomData.SetData(roomInfo.roomOwner, roomInfo.roomName, roomInfo.maxManCount, roomInfo.currentManCount, roomInfo.roomOwnerFd);
    }

    // Update is called once per frame
    void Update () {
	
	}
	
	void FixedUpdate(){
	
	}
	
	void LateUpdate(){
	
	}
}
