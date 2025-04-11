using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Top,
    Bottom,
    Right,
    Left
}

public class Room : MonoBehaviour
{
    public GameObject wallTop;
    public GameObject wallBottom;
    public GameObject wallRight;
    public GameObject wallLeft;

    /// <summary>
    /// 방향에 맞는 벽을 비활성화 하는 함수
    /// </summary>
    /// <param name="dir"></param>
    public void RemoveWall(Direction dir)
    {
        switch (dir)
        {
            case Direction.Top:
                if (wallTop != null)
                {
                    wallTop.SetActive(false);
                }
                break;
            case Direction.Bottom:
                if (wallBottom != null)
                {
                    wallBottom.SetActive(false);
                }
                break;            
            case Direction.Right:
                if (wallRight != null)
                {
                    wallRight.SetActive(false);
                }
                break;
            case Direction.Left:
                if (wallLeft != null)
                {
                    wallLeft.SetActive(false);
                }
                break;
        }
    }
}
