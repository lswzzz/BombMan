using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBoxUpdate{

    public enum PopDirection {
        HORIZONTAL = 0,
        VERTICAL = 1,
    }

    public enum BubbleRelative {
        Inline,
        OutLine,
        OutEdge,
    }

    public Transform transform;
    public PlayerColliderBox colliderBox;
    public PlayerBeHurtBox behurtBox;
    public PlayerExploreBox exploreBox;
    protected PopDirection direction;
    protected PlayerMovement movement;
    public Vector3 prePosition;

    public class BubbleNode {
        public BubbleRelative bubbleRelative;
        public int bubbleRow;
        public int bubbleCol;
    }

    public Dictionary<int, BubbleNode> RelativeDitionary;

    public void Init(Transform transform, PlayerColliderBox box1, PlayerBeHurtBox box2, PlayerExploreBox box3)
    {
        this.transform = transform;
        this.colliderBox = box1;
        this.behurtBox = box2;
        this.exploreBox = box3;
        resetColliderBox();
        resetBeHurtBox();
        resetExploreBox();
        movement = transform.GetComponent<PlayerMovement>();
        RelativeDitionary = new Dictionary<int, BubbleNode>();
    }

    public void Update(PopDirection direction)
    {
        this.direction = direction;
        resetColliderBox();
        resetBeHurtBox();
        resetIfOutEdge();
        RollBackPosition();
        UpdateBubbleRelative();
        ResetPositionAndAABBox();
        resetExploreBox();
    }

    public void RobotUpdate(PopDirection direction)
    {
        resetColliderBox();
        resetBeHurtBox();
        resetIfOutEdge();
    }

    public void removeKey(int key)
    {
        RelativeDitionary.Remove(key);
    }

    void UpdateBubbleRelative()
    {
        List<int> newList = new List<int>();
        foreach (KeyValuePair<int, BubbleNode> kv in RelativeDitionary)
        {
            BubbleNode node = kv.Value;
            int key = kv.Key;
            if (GameConst.bubbles[node.bubbleRow, node.bubbleCol] == null)
            {
                newList.Add(key);
                continue;
            }
            BubbleInlineBox inlineBox = GameConst.bubbles[node.bubbleRow, node.bubbleCol].GetComponent<BubbleInlineBox>();
            BubbleOutlineBox outlineBox = GameConst.bubbles[node.bubbleRow, node.bubbleCol].GetComponent<BubbleOutlineBox>();
            switch (node.bubbleRelative) {
                case BubbleRelative.Inline:
                    if (!behurtBox.BoxInline(inlineBox))
                    {
                        node.bubbleRelative = BubbleRelative.OutLine;
                    }
                    break;
                case BubbleRelative.OutLine:
                    if (!colliderBox.BoxInline(outlineBox))
                    {
                        node.bubbleRelative = BubbleRelative.OutEdge;
                        newList.Add(key);
                    }
                    break;
                case BubbleRelative.OutEdge:
                    break;
            }
        }
        foreach(int node in newList)
        {
            RelativeDitionary.Remove(node);
        }
    }

    public bool InBubbleRelative(int row, int col)
    {
        Bubble bubble = GameConst.bubbles[row, col];
        BubbleInlineBox inlineBox = GameConst.bubbles[row, col].GetComponent<BubbleInlineBox>();
        BubbleOutlineBox outlineBox = GameConst.bubbles[row, col].GetComponent<BubbleOutlineBox>();
        if (behurtBox.BoxInline(inlineBox))
        {
            return true;
        }else if (colliderBox.BoxInline(outlineBox))
        {
            return true;
        }
        return false;
    }

    public void AddBubbleRelative(int row, int col)
    {
        Bubble bubble = GameConst.bubbles[row, col];
        BubbleInlineBox inlineBox = GameConst.bubbles[row, col].GetComponent<BubbleInlineBox>();
        BubbleOutlineBox outlineBox = GameConst.bubbles[row, col].GetComponent<BubbleOutlineBox>();
        if (behurtBox.BoxInline(inlineBox))
        {
            BubbleNode node = new BubbleNode();
            node.bubbleRelative = BubbleRelative.Inline;
            node.bubbleRow = row;
            node.bubbleCol = col;
            RelativeDitionary.Add(row* GameConst.width + col, node);
        }
        else if (colliderBox.BoxInline(outlineBox))
        {
            BubbleNode node = new BubbleNode();
            node.bubbleRelative = BubbleRelative.OutLine;
            node.bubbleRow = row;
            node.bubbleCol = col;
            RelativeDitionary.Add(row * GameConst.width + col, node);
        }
        else
        {
            BubbleNode node = new BubbleNode();
            node.bubbleRelative = BubbleRelative.OutEdge;
            node.bubbleRow = row;
            node.bubbleCol = col;
            RelativeDitionary.Add(row * GameConst.width + col, node);
        }
    }

    public bool ContainsInlineBubbleRelative(int row, int col)
    {
        if (!RelativeDitionary.ContainsKey(row * GameConst.width + col)) return false;
        BubbleNode node = RelativeDitionary[row * GameConst.width + col];
        if (node.bubbleRow == row && node.bubbleCol == col && node.bubbleRelative == BubbleRelative.Inline) return true;
        return false;
    }

    public bool ContainsInline()
    {
        foreach (BubbleNode node in RelativeDitionary.Values)
        {
            if (node.bubbleRelative == BubbleRelative.Inline) return true;
        }
        return false;
    }

    public bool ContainsOutlineBubbleRelative(int row, int col)
    {
        if (!RelativeDitionary.ContainsKey(row * GameConst.width + col)) return false;
        BubbleNode node = RelativeDitionary[row * GameConst.width + col];
        if (node.bubbleRow == row && node.bubbleCol == col && node.bubbleRelative == BubbleRelative.OutLine) return true;
        return false;
    }

    void resetPositionInLine()
    {
        ResetPositionAndAABBox();
    }

    public void resetColliderBox()
    {
        colliderBox.ColliderBoxReset();
    }

    public void resetExploreBox()
    {
        exploreBox.ExloreBoxReset();
    }

    public void resetBeHurtBox()
    {
        behurtBox.BeHurtBoxReset();
    }

    void resetIfOutEdge()
    {
        Vector2 offset = Vector2.zero;
        if (colliderBox.min.x < 0)
        {
            offset.x = 0 - colliderBox.min.x;
        }
        if (colliderBox.max.x > GameConst.width * GameConst.tileWidth)
        {
            offset.x = GameConst.width * GameConst.tileWidth - colliderBox.max.x;
        }
        if (colliderBox.min.y < -GameConst.height * GameConst.tileHeight)
        {
            offset.y = -GameConst.height * GameConst.tileHeight - colliderBox.min.y;
        }
        if (colliderBox.max.y > 0)
        {
            offset.y = 0 - colliderBox.max.y;
        }
        if (offset != Vector2.zero)
        {
            resetTransform(offset);
            resetColliderBox();
            resetBeHurtBox();
        }

    }

    public AABBox getGridBox(int row, int col)
    {
        AABBox box = GameConst.sceneObstacles[row, col];
        if (box == null)
        {
            if (GameConst.bubbles[row, col] != null)
            {
                box = GameConst.bubbles[row, col].GetComponent<BubbleOutlineBox>();
            }
        }
        return box;
    }

    Vector2[] ResetPositionPrevious()
    {
        Vector2[] neiBox = GenerateListGrid();
        int row = (int)getCurrentGridPos().y;
        int col = (int)getCurrentGridPos().x;
        neiBox = ResetGrid(row, col, neiBox);
        return neiBox;
    }

    public Vector2 getCurrentGridPos()
    {
        Vector2 center = colliderBox.Center;
        int row = (int)-center.y / GameConst.tileHeight;
        int col = (int)center.x / GameConst.tileWidth;
        return new Vector2(col, row);
    }

    public Vector2 getPreGridPos()
    {
        Vector2 min = new Vector2(prePosition.x - colliderBox.leftoff, prePosition.y - colliderBox.downoff);
        Vector2 max = new Vector2(prePosition.x + colliderBox.rightoff, prePosition.y + colliderBox.upoff);
        Vector2 center = new Vector2((max.x + min.x) / 2, (max.y + min.y) / 2);
        int row = (int)-center.y / GameConst.tileHeight;
        int col = (int)center.x / GameConst.tileWidth;
        return new Vector2(col, row);
    }

    public Vector2 getCenter(int row, int col)
    {
        return new Vector2(col * GameConst.tileWidth + GameConst.tileWidth / 2, -(row * GameConst.tileHeight + GameConst.tileHeight / 2));
    }

    Vector2 checkRoadObstacle()
    {
        int row = (int)getCurrentGridPos().y;
        int col = (int)getCurrentGridPos().x;
        Vector2 preVec = getPreGridPos();
        int preRow = (int)preVec.y;
        int preCol = (int)preVec.x;
        if (row - preRow > 1)
        {
            for (int i = preRow + 1; i <= row; i++)
            {
                if (hasObstacle(i, preCol))
                {
                    return new Vector2(preCol, i-1);
                }
            }
        }
        else if (preRow - row > 1)
        {
            for (int i = row + 1; i <= preRow; i++)
            {
                if (hasObstacle(i, preCol))
                {
                    return new Vector2(preCol, i-1);
                }
            }
        }
        else if (col - preCol > 1)
        {
            for (int i = preCol + 1; i <= col; i++)
            {
                if (hasObstacle(preRow, i))
                {
                    return new Vector2(i-1, preRow);
                }
            }
        }
        else if (preCol - col > 1)
        {
            for (int i = col + 1; i <= preCol; i++)
            {
                if (hasObstacle(preRow, i))
                {
                    return new Vector2(i-1, preRow);
                }
            }
        }
        return Vector2.zero;
    }

    bool hasObstacle(int row, int col)
    {
        BubbleNode node = null;
        if (RelativeDitionary.ContainsKey(row * GameConst.width + col))
        {
            node = RelativeDitionary[row * GameConst.width + col];
        }
        if (GameConst.sceneObstacles[row, col] != null)
        {
            return true;
        }
        else if (node != null && (node.bubbleRow == row && node.bubbleCol == col))
        {
            if (node.bubbleRelative == BubbleRelative.OutLine)
            {
                return true;
            }
            else if (node.bubbleRelative == BubbleRelative.OutEdge)
            {
                return true;
            }
        }
        return false;
    }

    void RollBackPosition()
    {
        int maxCount = Mathf.Max(GameConst.width, GameConst.height);
        while (maxCount > 0)
        {
            maxCount--;
            int row = (int)getCurrentGridPos().y;
            int col = (int)getCurrentGridPos().x;
            Vector3 offset = transform.position - prePosition;
            BubbleNode node = null;
            if (RelativeDitionary.ContainsKey(row * GameConst.width + col))
            {
                node = RelativeDitionary[row * GameConst.width + col];
            }
            if (GameConst.sceneObstacles[row, col] != null)
            {
                AABBox box = getGridBox(row, col);
                RollbackPosition(getRollBackDirection(offset), colliderBox, box);
            }
            else if (node != null && (node.bubbleRow == row && node.bubbleCol == col))
            {
                if (node.bubbleRelative == BubbleRelative.OutLine)
                {
                    AABBox box = GameConst.bubbles[row, col].GetComponent<BubbleInlineBox>();
                    RollbackPosition(getRollBackDirection(offset), behurtBox, box);
                }
                else if (node.bubbleRelative == BubbleRelative.OutEdge)
                {
                    AABBox box = GameConst.bubbles[row, col].GetComponent<BubbleOutlineBox>();
                    RollbackPosition(getRollBackDirection(offset), colliderBox, box);
                }
                else
                {
                    Vector2 grid = checkRoadObstacle();
                    if (grid.Equals(Vector2.zero))
                    {
                        break;
                    }
                    Vector2 center = getCenter((int)grid.y, (int)grid.x);
                    transform.position = new Vector3(center.x, center.y, transform.position.z);
                    resetColliderBox();
                    resetBeHurtBox();
                }
            }
            else
            {
                Vector2 grid = checkRoadObstacle();
                if (grid.Equals(Vector2.zero))
                {
                    break;
                }
                Vector2 center = getCenter((int)grid.y, (int)grid.x);
                transform.position = new Vector3(center.x, center.y, transform.position.z);
                resetColliderBox();
                resetBeHurtBox();
            }
        }
    }

    PlayerDirection getRollBackDirection(Vector3 offset)
    {
        if(Mathf.Abs(offset.x) >= Mathf.Abs(offset.y))
        {
            if(offset.x > 0f)
            {
                return PlayerDirection.Right;
            }
            else
            {
                return PlayerDirection.Left;
            }
        }
        else
        {
            if(offset.y > 0f)
            {
                return PlayerDirection.Down;
            }
            else
            {
                return PlayerDirection.Up;
            }
        }
    }

    void RollbackPosition(PlayerDirection direction, AABBox colliderBox, AABBox box)
    {
        switch (direction)
        {
            case PlayerDirection.Left:
                offsetX(colliderBox, box, Vector2.left);
                break;
            case PlayerDirection.Right:
                offsetX(colliderBox, box, Vector2.right);
                break;
            case PlayerDirection.Down:
                offsetY(colliderBox, box, Vector2.down);
                break;
            case PlayerDirection.Up:
                offsetY(colliderBox, box, Vector2.up);
                break;
        }
    }

    void ResetPositionAndAABBox()
    {
        Vector2[] neiBox = ResetPositionPrevious();
        int row = (int)getCurrentGridPos().y;
        int col = (int)getCurrentGridPos().x;
        for (int i = 0; i < 8; i++)
        {
            if (neiBox[i] != Vector2.zero)
            {
                int row__ = (int)(row + neiBox[i].y);
                int col__ = (int)(col + neiBox[i].x);
                if (ContainsInlineBubbleRelative(row__, col__))
                {
                }
                else if(ContainsOutlineBubbleRelative(row__, col__))
                {
                    OutlineUpdateBoxTransform(row__, col__);
                }
                else
                {
                    NormalUpdateBoxTransform(row__, col__, neiBox[i]);
                }
            }
        }
    }

    void InlineUpdateBoxTransform(int row, int col)
    {

    }

    void OutlineUpdateBoxTransform(int row, int col)
    {
        BubbleInlineBox inlineBox = GameConst.bubbles[row, col].GetComponent<BubbleInlineBox>();
        BubbleOutlineBox outlineBox = GameConst.bubbles[row, col].GetComponent<BubbleOutlineBox>();
        if (behurtBox.BoxInline(inlineBox))
        {
            Vector2 direct = Vector2.zero;
            if (behurtBox.Center.x < inlineBox.Center.x)
            {
                direct.x = 1;
            }
            else if (behurtBox.Center.x > inlineBox.Center.x)
            {
                direct.x = -1;
            }
            if (behurtBox.Center.y > inlineBox.Center.y)
            {
                direct.y = 1;
            }
            else if (behurtBox.Center.y < inlineBox.Center.y)
            {
                direct.y = -1;
            }
            if (direction == PopDirection.HORIZONTAL)
            {
                offsetX(behurtBox, inlineBox, direct);
                if (behurtBox.BoxInline(inlineBox))
                {
                    offsetY(behurtBox, inlineBox, direct);
                }
            }
            else
            {
                offsetY(behurtBox, inlineBox, direct);
                if (behurtBox.BoxInline(inlineBox))
                {
                    offsetX(behurtBox, inlineBox, direct);
                }
            }
        }
    }

    void NormalUpdateBoxTransform(int row, int col, Vector2 direct)
    {
        AABBox box = getGridBox(row, col);
        if (box == null) return;
        if (box.BoxInline(colliderBox))
        {
            if (direction == PopDirection.HORIZONTAL)
            {
                offsetX(colliderBox, box, direct);
                if (box.BoxInline(colliderBox))
                {
                    offsetY(colliderBox, box, direct);
                }
            }
            else
            {
                offsetY(colliderBox, box, direct);
                if (box.BoxInline(colliderBox))
                {
                    offsetX(colliderBox, box, direct);
                }
            }

        }
    }

    public void offsetX(AABBox colliderBox, AABBox box, Vector2 direct)
    {
        Vector2 offset = getOffsetX(colliderBox, box, direct);
        if (offset != Vector2.zero)
        {
            resetTransform(offset);
            resetColliderBox();
            resetBeHurtBox();
        }
    }

    public void offsetY(AABBox colliderBox, AABBox box, Vector2 direct)
    {
        Vector2 offset = getOffsetY(colliderBox, box, direct);
        if (offset != Vector2.zero)
        {
            resetTransform(offset);
            resetColliderBox();
            resetBeHurtBox();
        }
    }

    public void resetTransform(Vector2 offset)
    {
        transform.position = new Vector3(transform.position.x + offset.x,
                        transform.position.y + offset.y, transform.position.z);
    }

    public Vector2 getOffsetX(AABBox box, AABBox neiBox, Vector2 direct)
    {
        Vector2 offset = Vector2.zero;
        if (direct.x < 0)
        {
            if (box.min.x < neiBox.max.x)
            {
                offset.x = neiBox.max.x - box.min.x;
            }
        }
        else if (direct.x > 0)
        {
            if (box.max.x > neiBox.min.x)
            {
                offset.x = neiBox.min.x - box.max.x;
            }
        }
        return offset;
    }

    public Vector2 getOffsetY(AABBox box, AABBox neiBox, Vector2 direct)
    {
        Vector2 offset = Vector2.zero;
        if (direct.y < 0)
        {
            if (box.max.y > neiBox.min.y)
            {
                offset.y = neiBox.min.y - box.max.y;
            }
        }
        else if (direct.y > 0)
        {
            if (box.min.y < neiBox.max.y)
            {
                offset.y = neiBox.max.y - box.min.y;
            }
        }
        return offset;
    }

    public Vector2[] GenerateListGrid()
    {
        Vector2[] neiBox = new Vector2[8];
        neiBox[0] = (new Vector2(-1, -1));
        neiBox[1] = (new Vector2(0, -1));
        neiBox[2] = (new Vector2(1, -1));
        neiBox[3] = (new Vector2(-1, 0));
        neiBox[4] = (new Vector2(1, 0));
        neiBox[5] = (new Vector2(-1, 1));
        neiBox[6] = (new Vector2(0, 1));
        neiBox[7] = (new Vector2(1, 1));
        return neiBox;
    }

    public Vector2[] ResetGrid(int row, int col, Vector2[] neiBox)
    {
        if (row == 0)
        {
            neiBox[0] = Vector2.zero;
            neiBox[1] = Vector2.zero;
            neiBox[2] = Vector2.zero;
        }
        else if (row == GameConst.height - 1)
        {
            neiBox[5] = Vector2.zero;
            neiBox[6] = Vector2.zero;
            neiBox[7] = Vector2.zero;
        }
        if (col == 0)
        {
            neiBox[0] = Vector2.zero;
            neiBox[3] = Vector2.zero;
            neiBox[5] = Vector2.zero;
        }
        else if (col == GameConst.width - 1)
        {
            neiBox[2] = Vector2.zero;
            neiBox[4] = Vector2.zero;
            neiBox[7] = Vector2.zero;
        }
        return neiBox;
    }
}
