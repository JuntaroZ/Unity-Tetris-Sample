using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino : MonoBehaviour
{
    // 
    public float previousTime;
    // minoが落ちるタイム
    public float fallTime = 1f;

    // ステージの大きさ
    private const int width = 10;
    private const int height = 20;
    public Vector3 rotationPoint;
    // Mino同士が重ならないようにするための配列(全Minoの状態保存のためstatic)
    private static Transform[,] grid = new Transform[width, height];

    enum EnResult
    {
        enSuccess,
        enInvalidLeft ,
        enInvalidRight ,
        enInvalidBottom ,
        enInvalidGrid ,
    }

    void Update()
    {
        MinoMovememt();
    }

    private void MinoMovememt()
    {
        // 左矢印キーで左に動く
        if (Input.GetKeyDown(KeyCode.LeftArrow) && ValidMovement(-1, 0) == EnResult.enSuccess)
        {
            transform.position += new Vector3(-1, 0, 0);
        }
        // 右矢印キーで右に動く
        else if (Input.GetKeyDown(KeyCode.RightArrow) && ValidMovement(+1, 0) == EnResult.enSuccess)
        {
            transform.position += new Vector3(1, 0, 0);
        }
        // 自動で下に移動させつつ、下矢印キーでも移動する
        else if ((Input.GetKey(KeyCode.DownArrow) && Time.time-previousTime > 0.05f) || Time.time-previousTime >= fallTime) 
        {
            if (ValidMovement(0, -1)== EnResult.enSuccess) {
                transform.position += new Vector3(0, -1, 0);
                previousTime = Time.time;
            } 
            else
            {
                if (AddToGrid() == false)
                {
                    // ゲームオーバー
                    Debug.Log("Game Over");
                } else
                {
                    CheckLines();
                    // 新しいminoを生成
                    FindObjectOfType<SpawnMino>().NewMino();
                }
                this.enabled = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // minoを上矢印キーを押して回転させる
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            var result = ValidMovement(0, 0);
            if (result == EnResult.enInvalidBottom || result == EnResult.enInvalidGrid)
            {
                // 回転させた後に、もし下に移動できない場合は、回転を元に戻す
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
                return;
            }
            // 両側の壁に埋まってたら修正を繰り返す
            foreach (Transform children in transform)
            {
                if (ValidMovement(0, 0) == EnResult.enInvalidLeft)
                {
                    transform.position += new Vector3(1, 0, 0);
                }
                if (ValidMovement(0, 0) == EnResult.enInvalidRight)
                {
                    transform.position += new Vector3(-1, 0, 0);
                }
            }
        }
    }
    EnResult ValidMovement(int x, int y)
    {
        // minoの子オブジェクトを一つずつ見ていく
        foreach (Transform children in transform)
        {
            int roundX = x + Mathf.RoundToInt(children.transform.position.x);
            int roundY = y + Mathf.RoundToInt(children.transform.position.y);

            // minoがステージよりはみ出さないように制御
            if (roundX < -width/2)
            {
                return EnResult.enInvalidLeft;
            }
            if (roundX > +width/2 - 1)
            {
                return EnResult.enInvalidRight;
            }
            if (roundY < -height/2)
            {
                return EnResult.enInvalidBottom;
            }

            int gridX = roundX + width / 2;
            int gridY = roundY + height / 2;
            if (gridY >= height) // Mino形状により、ステージの上にはみ出す場合は無視
            {
                continue;
            }
            if (grid[gridX, gridY] != null)
            {
                return EnResult.enInvalidGrid;
            }
        }
        return EnResult.enSuccess;
    }
    bool AddToGrid() 
    {
        foreach (Transform children in transform) 
        {
            int roundX = Mathf.RoundToInt(children.transform.position.x);
            int roundY = Mathf.RoundToInt(children.transform.position.y);
            int gridX = roundX + width / 2;
            int gridY = roundY + height / 2;
            if (gridY >= height) // Mino形状により、ステージの上にはみ出す場合はゲームオーバー
            {
                return false;
            }
            grid[gridX, gridY] = children;
        }
        return true;
    }
    // 今回の追加 ラインがあるか？確認
    public void CheckLines()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
                FindObjectOfType<GameManager>().AddScore(100);
            }
        }
    }

    bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null)
                return false;
        }
        return true;
    }

    // 今回の追加 ラインを消す
    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }

    }

    // 今回の追加 列を下げる
    public void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }
}
