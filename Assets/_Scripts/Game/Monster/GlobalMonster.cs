using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalMonster : MonoBehaviour {

    private PlayerColliderBox playerBox;
    private PlayerInputController player;
    private int AttackCount = 3;
    private float AttackDelta = 5f;
    private float waitTime = 5f;
    private int currentAttackCount;
    private bool startAttack = false;
    private float curDeltaTime = 0;
	// Use this for initialization
	void Start () {
        playerBox = GameGlobalData.player.GetComponent<PlayerColliderBox>();
        player = GameGlobalData.player;
        AttackDelta = 5f;
        curDeltaTime = 0f;
        waitTime = 5f;
        StartCoroutine(Wait());
    }
	
	// Update is called once per frame
	void Update () {
        if (startAttack)
        {
            curDeltaTime += Time.deltaTime;
            if (curDeltaTime >= AttackDelta)
            {
                curDeltaTime = 0f;
                Attack();
            }
        }
	}

    IEnumerator Wait() {
        yield return new WaitForSeconds(waitTime);
        startAttack = true;
    }

	void Attack()
    {
        currentAttackCount = 0;
        int row = GameConst.getRow(playerBox.Center.y);
        int col = GameConst.getCol(playerBox.Center.x);
        AttackFirst(row, col);
        if(currentAttackCount < AttackCount)
        {
            AttackSecond(row, col);
            if(currentAttackCount < AttackCount)
            {
                AttackThird(row, col);
            }
        }
    }

    public void AttackFirst(int row, int col)
    {
        var listOne = getGridList(row, col, 1);
        AttackLoop(row, col, 4, 2, ref listOne);
    }

    public void AttackSecond(int row, int col)
    {
        var listOne = getGridList(row, col, 2);
        AttackLoop(row, col, 5, 3, ref listOne);
    }

    public void AttackLoop(int row, int col, int value1, int value2, ref List<Vector2> listOne)
    {
        if (listOne.Count >= value1 && AttackCount - currentAttackCount >= 2)
        {
            List<int> list = new List<int>();
            int rand = Random.Range(0, listOne.Count);
            list.Add(rand);
            while (true)
            {
                rand = Random.Range(0, listOne.Count);
                if (!containsInIntList(ref list, rand))
                {
                    list.Add(rand);
                    break;
                }
            }
            foreach(int val in list)
            {
                Vector2 grid = listOne[val];
                int row_ = (int)grid.y;
                int col_ = (int)grid.x;
                player.GlobalAttack(row_, col_);
            }
            currentAttackCount += 2;
        }
        else if (listOne.Count >= value2 && AttackCount - currentAttackCount >= 1)
        {
            List<int> list = new List<int>();
            int rand = Random.Range(0, listOne.Count);
            list.Add(rand);
            Vector2 grid = listOne[rand];
            int row_ = (int)grid.y;
            int col_ = (int)grid.x;
            player.GlobalAttack(row_, col_);
            currentAttackCount += 1;
        }
    }

    public void AttackThird(int row, int col)
    {
        var list3 = getGridList(row, col, 3);
        var list4 = getGridList(row, col, 4);
        var list5 = getGridList(row, col, 5);
        List<Vector2> total = new List<Vector2>();
        foreach(Vector2 val in list3)
        {
            total.Add(val);
        }
        foreach (Vector2 val in list4)
        {
            total.Add(val);
        }
        foreach (Vector2 val in list5)
        {
            total.Add(val);
        }
        AttackLoop(row, col, 15, 6, ref total);
    }

    public bool containsInIntList(ref List<int> list, int value)
    {
        foreach(int val in list)
        {
            if (val == value) return true;
        }
        return false;
    }

    List<Vector2> getGridList(int row, int col, int layer)
    {
        List<Vector2> list = new List<Vector2>();
        int count = 8 * layer;
        int curRow = row - layer;
        int curCol = col - layer;
        int maxCol = col + layer;
        int maxRow = row + layer;
        int minRow = row - layer;
        int currentCount = 0;
        while(currentCount < count)
        {
            addToList(ref list, curRow, curCol);
            if(curCol >= maxCol)
            {
                curRow++;
                curCol = col - layer;
            }
            else if (curRow != maxRow && curRow != minRow)
            {
                curCol += layer * 2;
            }
            else
            {
                curCol++;
            }
            currentCount++;
        }
        return list;
    }

    public bool addToList(ref List<Vector2> list, int row, int col)
    {
        if (row < 0 || row >= GameConst.height) return false;
        if (col < 0 || col >= GameConst.width) return false;
        if(GameConst.hasObstacleOrBubble(row, col))
        {
            return false;
        }
        list.Add(new Vector2(col, row));
        return true;
    }
}
