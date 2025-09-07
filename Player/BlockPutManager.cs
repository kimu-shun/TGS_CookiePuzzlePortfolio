using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.Tilemaps;

public class BlockPutManager : MonoBehaviour
{
    [Header("ワッフルのTile"),SerializeField] private Tile _wafflePrefab;
    [Header("バターブロックのタイル"), SerializeField] private Tilemap _groundTileMap;
    [Header("水のタイル"), SerializeField] private Tilemap _waterTileMap;
    [Header("ワッフルのタイル"), SerializeField] private Tilemap _waffleTileMap;
    [Header("チョコブロックのタイル"), SerializeField] private Tilemap _wallTileMap;
    [Header("トランポリンのタイル"), SerializeField] private Tilemap _trampolineMap;

    [Header("ワッフルのタイル"),SerializeField]
    private TileBase _waffleTile;

    [Header("ゲームのメインカメラ"),SerializeField]
    private Camera _mainCam;
    [Header("メインのCanvas"),SerializeField]
    private Canvas _mainCanvas;
    [Header("プレイヤー"), SerializeField]
    private GameObject _player;
    [Header("回収するときに表示するワッフル"), SerializeField]
    private GameObject wafflePrefab;
    [Header("ワッフルが回収されるまでの時間"), SerializeField]
    private float _waffleMoveDuration = 1.0f;


    [Header("頭上UIの表示非表示をコントロールするスクリプト"),SerializeField]
    private TextFadeController textFadeController;

    [Header("設置可能時のスプライト"),SerializeField]
    private Sprite _placement_green;
    [Header("設置不可能時のスプライト"), SerializeField] 
    private Sprite _placement_red;

    // ワッフルを置けるならTrue
    public bool _canPutBlock;

    // プレイヤーの操作をコントロールする関数
    private PlayerControllManager _playerControllManager;

    // ブロックを置ける場所をListで保管
    private List<Vector3Int> _putPosLists = new List<Vector3Int>();
    // _putPosListsの下のブロックがバターか水ならTrueになる
    private List<bool> _putbellowPosIsWall = new List<bool>();
    private List<bool> _putBellowIsWater = new List<bool>();

    private Vector3 _waffleTileAnchorOrigin = Vector3.zero;
    // 置く場所を選択しているIndex
    public int selectPosIndex = 0;

    private void Start()
    {
        _playerControllManager = GetComponent<PlayerControllManager>();
        _waffleTileAnchorOrigin = _waffleTileMap.tileAnchor;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _putPosLists = GetPutPos();
        }
    }

    /// <summary>
    /// ブロックを置ける場所を表示する関数
    /// </summary>
    /// <param name="putPos">置く位置</param>
    /// <param name="posView">置く位置を表示するオブジェクト</param>
    public void BlockPutPosView(GameObject posView)
    {

        // 置ける設置できる場所をリセット
        _putPosLists.Clear();
        _putbellowPosIsWall.Clear();
        _putBellowIsWater.Clear();
        // 置ける場所を検索してリストに入れる
        _putPosLists = GetPutPos();

        if (_putBellowIsWater[selectPosIndex])
        {
            _waffleTileMap.tileAnchor = new Vector3(_waffleTileMap.tileAnchor.x, _waterTileMap.tileAnchor.y, _waffleTileMap.tileAnchor.z);
        }
        else
        {
            _waffleTileMap.tileAnchor = _waffleTileAnchorOrigin;
        }

        Vector3 spawnWorldPos = _waffleTileMap.GetCellCenterWorld(_putPosLists[selectPosIndex]); // マスの中心座標



        posView.transform.position = spawnWorldPos;
        if (!_putbellowPosIsWall[selectPosIndex])
        {
            posView.GetComponent<SpriteRenderer>().sprite = _placement_green;
            _canPutBlock = true;

        }
        else
        {
            posView.GetComponent<SpriteRenderer>().sprite= _placement_red;
            _canPutBlock = false;
        }
        posView.gameObject.SetActive(true);
    }

    /// <summary>
    /// ブロックを置く関数
    /// </summary>
    /// <param name="putPos">設置する位置</param>
    public void BlockPut()
    {
        // 選択位置の下が壁でなく、プレイヤーがワッフルを持っている場合のみ処理を実行
        if (!_putbellowPosIsWall[selectPosIndex] && _playerControllManager._haveWaffle)
        {
            if (_putBellowIsWater[selectPosIndex])
            {
                // 下が水タイルの場合、ワッフルのアンカー位置を水タイルに合わせる
                _waffleTileMap.tileAnchor = new Vector3(_waffleTileMap.tileAnchor.x, _waterTileMap.tileAnchor.y, _waffleTileMap.tileAnchor.z);
            }
            else
            {
                // 通常のアンカー位置に戻す
                _waffleTileMap.tileAnchor = _waffleTileAnchorOrigin;
            }
            // 選択位置にワッフルタイルを配置
            _waffleTileMap.SetTile(_putPosLists[selectPosIndex], _wafflePrefab);
            // ワッフル所持をfalse
            _playerControllManager._haveWaffle = false;
            // 頭上のテキストを回収にする
            textFadeController.DestroyTextView();
        }
        else
        {
            Debug.Log("クッキーないか置けない");
        }
    }

    /// <summary>
    /// ワッフルを回収するときの関数
    /// </summary>
    public void WaffleCatcher()
    {
        foreach (var pos in _waffleTileMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = _waffleTileMap.GetTile(pos);
            if (tile == _waffleTile)
            {
                // タイルの中心座標を取得
                Vector3 worldPos = _waffleTileMap.CellToWorld(pos) + _waffleTileMap.cellSize / 2f;
                Vector3 screenPos = _mainCam.WorldToScreenPoint(worldPos);

                // プレイヤー位置をスクリーン座標に変換
                Vector3 playerScreenPos = _mainCam.WorldToScreenPoint(_player.transform.position);
                Camera uiCam = _mainCanvas.worldCamera;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _mainCanvas.GetComponent<RectTransform>(),
                    screenPos,
                    uiCam,
                    out Vector2 localPoint
                    );

                // UIオブジェクト生成
                GameObject obj = Instantiate(wafflePrefab, _mainCanvas.transform);
                RectTransform objRect = obj.GetComponent<RectTransform>();
                objRect.anchoredPosition = localPoint;

                // プレイヤー位置に向かって移動
                StartCoroutine(MoveToPlayer(objRect, this.gameObject, playerScreenPos));

                break;
            }
        }
    }

    /// <summary>
    /// 回収した時にPlayerのいる位置までオブジェクトを移動させる
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    IEnumerator MoveToPlayer(RectTransform obj, GameObject targetObj,Vector2 playrScreenPos)
    {
        Camera uiCam = _mainCanvas.worldCamera;
        float time = 0f;
        float duration = _waffleMoveDuration;
        Vector3 startPos = obj.anchoredPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _mainCanvas.GetComponent<RectTransform>(),
            playrScreenPos,
            uiCam,
            out Vector2 targetPos
        );
        Debug.Log($"targetPos{targetPos}");
        Vector3 startScale = obj.localScale;
        Vector3 endScale = Vector3.zero;

        while (time < duration)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _mainCanvas.GetComponent<RectTransform>(),
            playrScreenPos,
            uiCam,
            out targetPos
        );
            time += Time.deltaTime;
            float t = Mathf.Pow(time / duration, 2);

            obj.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);

            obj.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        Destroy(obj.gameObject);
    }

    /// <summary>
    /// ワッフルを消す関数
    /// </summary>
    public void DestroyPutObject()
    {
        BoundsInt bounds = _waffleTileMap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = _waffleTileMap.GetTile(pos);
            if (tile == _waffleTile)
            {
                _playerControllManager._haveWaffle = true;
                textFadeController.PutHintView();
                WaffleCatcher();
                _waffleTileMap.SetTile(pos, null); // タイルを削除
            }
        }
    }


    /// <summary>
    /// ワッフルブロックを置ける場所をすべて確認する関数
    /// </summary>
    /// <returns>Vector3Intのリストで返す</returns>
    private List<Vector3Int> GetPutPos()
    {
        // 
        List<Vector3Int> placeablePositions = new List<Vector3Int>();
        // プレイヤーの位置取得
        Vector3 playerWorldPos = transform.position;
        Vector3Int playerCell = _waffleTileMap.WorldToCell(playerWorldPos);

        _putbellowPosIsWall.Clear();
        _putBellowIsWater.Clear();

        // プレイヤーを中心として上下左右2マス取得
        for (int dx = -2;dx <= 2;dx++)
        {
            for(int dy = -2;dy <= 2; dy++)
            {
                //設置する場所のPos
                Vector3Int checkPos = playerCell + new Vector3Int(dx,dy);
                // 設置する場所の１マス下
                Vector3Int checkPosBellow = new Vector3Int(checkPos.x, checkPos.y - 1);

                // 条件の場所
                // 置く場所の下のマスの指定
                Vector3Int groundBellowCheckCell = _groundTileMap.WorldToCell(_waffleTileMap.GetCellCenterWorld(checkPosBellow)); 
                Vector3Int waterBellowCheckCell = _waterTileMap.WorldToCell(_waffleTileMap.GetCellCenterWorld(checkPosBellow) );
                Vector3Int wallBellowCheckCell = _wallTileMap.WorldToCell(_waffleTileMap.GetCellCenterWorld(checkPosBellow));
                // 置く場所のマスの指定
                Vector3Int groundCenterCheckCell = _groundTileMap.WorldToCell(_waffleTileMap.GetCellCenterWorld(checkPos));
                Vector3Int waterCenterCheckCell = _waterTileMap.WorldToCell(_waffleTileMap.GetCellCenterWorld(checkPos));
                Vector3Int wallCenterCheckCell = _wallTileMap.WorldToCell(_wallTileMap.GetCellCenterWorld(checkPos) );

                // 置く場所の一つ下のマスに GroundMapのタイルがあるか確認　あったらTrueなかったらFalse
                bool canPlaceBelowGroundTile = _groundTileMap.GetTile(groundBellowCheckCell) != null;
                // 置く場所の一つ下のマスに waterMapのタイルがあるか確認　あったらTrueなかったらFalse
                bool canPlaceBellowWaterTile = _waterTileMap.GetTile(waterBellowCheckCell) != null;
                // 置く場所の1つ下のマスに　WallTileMapのタイルがあるか確認　あったらTrueなかったらFalse
                bool canPlaceBellowWallTile = _wallTileMap.GetTile(wallBellowCheckCell) != null;


                // 置く場所にそれぞれのタイルがあるか確認　　あったらTrueなかったらFalse
                bool canPlaceByCookieTile = _groundTileMap.GetTile(groundCenterCheckCell) != null;
                bool canPlaceByWaterTile = _waterTileMap.GetTile(waterCenterCheckCell) != null;
                bool canPlaceByWallTile = _wallTileMap.GetTile(waterCenterCheckCell) != null;


                // 設置するマスにTileがないかつ下のマスが地面 もしくは　設置するマスにTileがないかつ下のマスが水なら実行            
                if ((canPlaceBellowWaterTile && !canPlaceByCookieTile && !canPlaceByWaterTile && !canPlaceByWallTile) ||
                    (canPlaceBelowGroundTile && !canPlaceByCookieTile && !canPlaceByWaterTile && !canPlaceByWallTile) ||
                    (canPlaceBellowWallTile && !canPlaceByCookieTile && !canPlaceByWaterTile && !canPlaceByWallTile))
                {
                    if (checkPos != playerCell)
                    {
                        if ((canPlaceBellowWallTile && canPlaceBelowGroundTile) || (canPlaceBellowWallTile && canPlaceBelowGroundTile))
                        {
                            _putbellowPosIsWall.Add(false);
                        }
                        else if (canPlaceBellowWallTile)
                        {
                            _putbellowPosIsWall.Add(true);
                        }
                        else
                        {
                            _putbellowPosIsWall.Add(false);
                        }

                        _putBellowIsWater.Add(canPlaceBellowWaterTile);
                        placeablePositions.Add(checkPos);
                    }
                }
            }
        }
        return placeablePositions;
    }

    public void OnLeftDpad(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(selectPosIndex > 0)
            {
                selectPosIndex--;
            }
            else
            {
                //Debug.Log($"現在のselectPosIndexは{selectPosIndex}なのでこれ以上小さくできない");
            }
        }
    }

    public void OnRightDpad(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selectPosIndex < _putPosLists.Count - 1)
            {
                selectPosIndex++;
            }
            else
            {
                //Debug.Log($"現在のselectPosIndexは{selectPosIndex}なのでこれ以上大きくできない");
            }
        }
    }


}
