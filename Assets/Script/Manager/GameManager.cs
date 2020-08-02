using HelpTool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    //游戏人数
    public int PlayerCount = 3;
    private List<Player> playerList = new List<Player>();

    public UIManager uIManager;

    public Player currentPlayer
    {
        get
        {
            return playerList[currentCountry];
        }
    }
    public CameraManager cameraManager;

    /// <summary>
    /// 选中的地图
    /// </summary>
    public Units hitMap = null;
    //有没有棋子移动
    public Troops moveTroop = null;

    private Color colorStart;
    private Color colorEnd = Color.clear;
    private Color rawColor = new Color((float)0.7, (float)0.6, (float)0.3, (float)0.7);
    //特效
    public GameObject Effect { get; private set; }
    //unit 的父组件
    public GameObject mapUnits;
    public GameObject Mapunits => mapUnits;
    public GameObject grassland;
    public GameObject oceanland;
    public const float landwidth = 1.85F;
    public const float landheight = 1.6F;
    public const int width = 100;
    public const int height = 100;

    private int currentCountry =0;
    private Color[] allCountry = { Color.yellow, Color.red, Color.gray };
    
    public Units[,] units = new Units[width, height];
    private int[,] map = new int[width, height];

    private static GameManager _instance;

    public GameObject Maps;

    public Vector2Int Find(GameObject unit)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (unit.Equals(units[i, j].unit))
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    private int getCount(int x, int y)
    {
        int count = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                try
                {
                    if (map[i, j] == 1)
                    {
                        count++;
                    }
                }
                catch (Exception)
                {
                    if (j == 2)
                    {
                        j++;
                    }
                }
            }
        }
        return count;
    }

    private void InitMap()
    {
        Random random = new Random();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (random.Next(10) < 3)
                {
                    map[i, j] = 1;
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int r = random.Next(9, 15);
                map[i, j] = 1;
                Vector2 vector;
                if (i % 2 == 0)
                {
                    vector = new Vector2(landwidth / 4 * 3 * i, landheight * j);
                }
                else
                {
                    vector = new Vector2(landwidth / 4 * 3 * i, landheight * j + landheight / 2);
                }
                if (getCount(i, j) * 2 > r)
                {
                    GrassLand grassLand = new GrassLand(vector);
                    grassLand.SetName("grassland " + i + " : " + j);
                    units[i, j] = grassLand;
                }
                else
                {                   
                    OceanLand oceanLand = new OceanLand(vector);
                    oceanLand.SetName("oceanLand " + i + " : " + j);
                    units[i, j] = oceanLand;
                }                
                if (i == 0 && j == 0)
                {
                    units[i, j].SetName("start");
                }
                if (i == width - 1 && j == height - 1)
                {
                    units[i, j].SetName("end");
                }
            }
        }
    }

    public Color CurrentCountry
    {
        get
        {
            return playerList[currentCountry].Country;
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                //则创建一个
                _instance = GameObject.Find("Map").GetComponent<GameManager>();
            //返回这个实例
            return _instance;
        }
    }

    public void PlayerDead(Player player)
    {
        playerList.Remove(player);
        if (playerList.Count == 1)
        {
            uIManager.ShowPanel(player.PlayerName + "胜利", 2);
        }
        else
        {
            uIManager.ShowPanel(player.PlayerName + "落败", 2);
        }
    }

    public void NextTime()
    {
        currentCountry++;
        if(currentCountry >= allCountry.Length)
        {
            currentCountry = 0;
        }
        cameraManager.MoveToPoint(playerList[currentCountry].Center);
        cameraManager.ShowHead(playerList[currentCountry].Next());
    }

    private void initPlayer()
    {
        Random random = new Random();
        for (int i = 0;i< PlayerCount; i++)
        {
            int rWidth = random.Next(width), rHeight = random.Next(height);
            for (; units[rWidth, rHeight].unit.tag.Equals("OceanLand"); rWidth = random.Next(width), rHeight = random.Next(height));
            playerList.Add(new Player(allCountry[i], new Vector2Int(rWidth, rHeight),"玩家"+i));
        }
    }

    private void Awake()
    {
        if (!Instance)
            _instance = this;
        Maps = GameObject.Find("Map");
        mapUnits = GameObject.Find("MapUnits");
        Effect = GameObject.Find("Boom");
        Effect.GetComponent<ParticleSystem>().Stop(true);
        InitMap();
        initPlayer();
    }

    public void CanelSelect()
    {
        if (hitMap != null)
        {
            if(hitMap.unit.tag.Equals("GrassLand"))
                hitMap.unit.GetComponent<SpriteRenderer>().color = hitMap.Country;
            else
                hitMap.unit.GetComponent<SpriteRenderer>().color = Color.white;
            if (hitMap.troop != null)
            hitMap.troop.HideDirection();
        }
        hitMap = null;
    }

    void Start()
    {
        cameraManager.MoveToPoint(playerList[currentCountry].Center);
    }

    void Update()
    {
        if (hitMap != null)          //地图闪烁效果
        {
            colorStart = hitMap.unit.GetComponent<SpriteRenderer>().color;
            if (colorStart == hitMap.Country || (hitMap.unit.tag.Equals("OceanLand") && colorStart == Color.white))
            {
                colorEnd = rawColor;
            }
            if (colorStart == rawColor)
            {
                if (hitMap.unit.tag.Equals("GrassLand"))
                    colorEnd = hitMap.Country;
                else
                    colorEnd = Color.white;
            }
            hitMap.unit.GetComponent<SpriteRenderer>().color = Color.Lerp(colorStart, colorEnd, (float)0.3);
        }
        //棋子移动
        if (moveTroop != null)
        {
            if (units[moveTroop.Point.x, moveTroop.Point.y].unit.transform.position != moveTroop.Troop.transform.position)
            {
                moveTroop.Troop.transform.position = Vector3.Lerp(moveTroop.Troop.transform.position, units[moveTroop.Point.x, moveTroop.Point.y].unit.transform.position, 0.1f);
            }
            else
            {
                moveTroop = null;
            }
        }

        if (Input.GetMouseButtonUp(1))      //右键取消
        {
            CanelSelect();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (UIManager.Clicked)          //button拦截后续效果
            {
                UIManager.Clicked = false;
                return;
            }
            if(UIManager.Opened != 0)
            {
                if(UIManager.Opened == 1)
                    UIManager.Opened--;
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hitMap != null)      //回复原色
                {
                    hitMap.unit.GetComponent<SpriteRenderer>().color = hitMap.Country;
                    if (hitMap.troop != null)   //隐藏方向
                    {
                        hitMap.troop.HideDirection();
                    }
                }
                Vector2Int point = Find(hit.collider.gameObject);    //选中的位置
                
                if(hitMap == null || hitMap.troop == null)
                {
                    hitMap = units[point.x, point.y];           //重新选中
                    cameraManager.ShowInfo(hitMap.Property);
                }
                if (units[point.x, point.y].troop == null && hitMap.troop != null && hitMap.troop.Country == CurrentCountry)     //棋子移动
                {
                    int direct = hitMap.troop.getDirect(point);
                    if (direct != -1)
                    {                        
                        if(hitMap.troop.Move(direct) && units[point.x, point.y].Country != CurrentCountry)
                        {
                            playerList[currentCountry].Conquer(units[point.x, point.y]);
                        }
                        hitMap = units[point.x, point.y];
                        if(hitMap.troop != null)                        //移动之后重新计算方向
                            hitMap.troop.HideDirection();
                    }
                    else
                    {
                        hitMap = units[point.x, point.y];
                        cameraManager.ShowInfo(hitMap.Property);
                    }
                }
                if (units[point.x, point.y].troop != null && hitMap.troop != null)      //重新选择棋子或攻击
                {
                    if (units[point.x, point.y].troop.Country == CurrentCountry)
                    {
                        hitMap = units[point.x, point.y];                   //重新选择地图
                        cameraManager.ShowInfo(hitMap.Property);
                    }
                    else if(hitMap.troop.Country==CurrentCountry && hitMap.troop.getDirect(point) != -1)                      
                    {
                        hitMap.troop.Attack(units[point.x, point.y].troop);     //攻击
                    }
                }
                if (hitMap.troop != null && hitMap.troop.Country == CurrentCountry)         //显示方向
                {
                    hitMap.troop.ShowDirection();
                }
            }
            else
            {
                CanelSelect();
            }
        }
    }
}
