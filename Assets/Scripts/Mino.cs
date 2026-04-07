using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Mino : MonoBehaviour
{
    public enum KeyType
    {
        LeftArrow,
        RightArrow,
        DownArrow,
        UpArrow
    }

    [System.Serializable]
    public class MinoConfig
    {
        public KeyType keyType;
        public bool repeatKey = false;
        public Vector3 addVector = new Vector3(0, 0, 0);
        public bool rotate = false;
    }

    public List<MinoConfig> minoConfigList = new List<MinoConfig>();
    private float previousTime = 0f;
    private Dictionary<KeyType, KeyCode> dictKeyMap = new Dictionary<KeyType, KeyCode>
    {
        {KeyType.LeftArrow, KeyCode.LeftArrow},
        {KeyType.RightArrow, KeyCode.RightArrow},
        {KeyType.DownArrow, KeyCode.DownArrow},
        {KeyType.UpArrow, KeyCode.UpArrow},
    };
   // ステージの大きさ
    private const int width = 10;
    private const int height = 20;
    private Vector3 rotationPoint = new Vector3(0.0f, 0.0f, 0.0f); // minoの回転の中心点
    private Vector3 rotationXYZ = new Vector3(0.0f, 0.0f, 1.0f); // minoはZ軸回転
    private const float rotationAngle = 90.0f; // minoは90度回転
    private const float downMoveIntervalTime = 0.05f; // 矢印キーでminoを動かすときのタイム

    // Mino同士が重ならないようにするための配列(全Minoの状態保存のためstatic)
    private static Transform[,] grid = new Transform[width, height];

    private GameManager gameManager;
    enum EnResult
    {
        enSuccess,
        enInvalidLeft ,
        enInvalidRight ,
        enInvalidBottom ,
        enInvalidGrid ,
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    void Update()
    {
        foreach (MinoConfig config in minoConfigList)
        {
            bool moveOn = false;
            Vector3 addVector = config.addVector;
            if (config.repeatKey)
            {
                if (Input.GetKey(dictKeyMap[config.keyType]) && Time.time - previousTime > gameManager.repeatFallTime)
                {
                    moveOn = true;      
                    previousTime = Time.time;
                }
            } 
            else
            {
                if (Input.GetKeyDown(dictKeyMap[config.keyType]))
                {
                    moveOn = true;      
                }
            }
            // 自動落下
            if (Time.time - previousTime > gameManager.autoFallTime)
            {
                addVector = new Vector3(0, -1, 0);
                moveOn = true;
                previousTime = Time.time;
            }

            if (moveOn)
            {
                if (config.rotate) // 回転の処理を優先
                {
                    // minoを回転させる
                    transform.RotateAround(transform.TransformPoint(rotationPoint), rotationXYZ, +rotationAngle);
                    var result = ValidMovement(0, 0);
                    if (result == EnResult.enInvalidBottom || result == EnResult.enInvalidGrid)
                    {
                        // 回転させた後に、もし下に移動できない場合は、回転を元に戻す
                        transform.RotateAround(transform.TransformPoint(rotationPoint), rotationXYZ, -rotationAngle);
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
                    FindObjectOfType<SfxPlayer>().PlaySfx(1); // Mino回転の音を再生
                }

                var resultMove = ValidMovement((int)addVector.x, (int)addVector.y);
                if (resultMove == EnResult.enSuccess)
                {
                    transform.position += addVector;
                }
                else if (addVector.y < 0.0f) // 下に移動できない場合は、Minoを固定する
                {
                    this.enabled = false;
                    if (AddToGrid() == false)
                    {
                        // ゲームオーバー
                        gameManager.SetGameOver();
                        Debug.Log("Game Over");
                        return;
                    }

                    bool isDelete = CheckDeleteLines();
                    if (isDelete == false)
                    {
                        FindObjectOfType<SfxPlayer>().PlaySfx(0); // Mino地面着地の音を再生
                        // 新しいminoを生成
                        FindObjectOfType<SpawnMino>().NewMino();
                    }
                }
                break; // 1回のUpdateで複数のキー入力を処理しないようにするため、キー入力があったらループを抜ける
            }
        }
/*
        // 左矢印キーで左に動く
        if (Input.GetKeyDown(KeyCode.LeftArrow) && ValidMovement(-1, 0) == EnResult.enSuccess)
        {
            transform.position += new Vector3(-1, 0, 0);
        }
        // 右矢印キーで右に動く
        if (Input.GetKeyDown(KeyCode.RightArrow) && ValidMovement(+1, 0) == EnResult.enSuccess)
        {
            transform.position += new Vector3(1, 0, 0);
        }
*/
/*
        // 自動で下に移動させつつ、下矢印キーでも移動する
        if ((Input.GetKey(KeyCode.DownArrow) && Time.time-previousTime > downMoveIntervalTime) || Time.time-previousTime >= 1.0f) 
        {
            if (ValidMovement(0, -1)== EnResult.enSuccess) {
                transform.position += new Vector3(0, -1, 0);
                previousTime = Time.time;
            } 
            else
            {
                this.enabled = false;
                if (AddToGrid() == false)
                {
                    // ゲームオーバー
                    gameManager.SetGameOver();
                    Debug.Log("Game Over");
                    return;
                }

                bool isDelete = CheckDeleteLines();
                if (isDelete == false)
                {
                    FindObjectOfType<SfxPlayer>().PlaySfx(0); // Mino地面着地の音を再生
                    // 新しいminoを生成
                    FindObjectOfType<SpawnMino>().NewMino();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // minoを上矢印キーを押して回転させる
            transform.RotateAround(transform.TransformPoint(rotationPoint), rotationXYZ, +rotationAngle);
            var result = ValidMovement(0, 0);
            if (result == EnResult.enInvalidBottom || result == EnResult.enInvalidGrid)
            {
                // 回転させた後に、もし下に移動できない場合は、回転を元に戻す
                transform.RotateAround(transform.TransformPoint(rotationPoint), rotationXYZ, -rotationAngle);
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
            FindObjectOfType<SfxPlayer>().PlaySfx(1); // Mino回転の音を再生
        }
*/
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
    private bool CheckDeleteLines()
    {
        int addScore = 100;
        List<int> deleteLineList = new List<int>();
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                FindObjectOfType<ParticlePlayer>().Play(new Vector3(0, i - height / 2, 10));
                FindObjectOfType<SfxPlayer>().PlaySfx(2); // Mino消去の音を再生
                DeleteLine(i);
                deleteLineList.Add(i);
                gameManager.AddScore(addScore);
                addScore += 100; // 同時にラインを消すと、スコアがどんどん上がるようにする
            }
        }
        if (deleteLineList.Count > 0)
        {
            // エフェクトを再生してから、ラインを消すために、コルーチンで遅延させる
            StartCoroutine(DelayRowDown(deleteLineList));
            return true;
        }
        return false;
    }

    private IEnumerator DelayRowDown(List<int> deleteLineList)
    {
        // エフェクト再生のために少し待つ（例: 1.0秒）
        yield return new WaitForSeconds(1.0f);
        // 消去したラインを上から順に下げる
        foreach (int i in deleteLineList)
        {
            RowDown(i);
        }
        deleteLineList.Clear();
        // 新しいminoを生成
        FindObjectOfType<SpawnMino>().NewMino();
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
