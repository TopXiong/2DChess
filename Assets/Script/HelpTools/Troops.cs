using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HelpTool
{
    public abstract class Troops
    {
        protected int movement;         //行动力
        protected int maxMovement;      //最大行动力
        protected float combat;         //攻击力
        protected float maxBlood;       //最大血量
        protected float currentBlood;   //当前血量

        protected GameObject troop;     //兵种图片
        protected GameObject direction; //方向图片
        protected TroopType type;       //兵种类型
        protected Color countryColor;   //所属国家
        protected SpriteRenderer countrySprite; //国家颜色
        protected SpriteRenderer troopSprite;   //军队图片
        protected SpriteRenderer countSprite;   //军队图片
        protected Vector2Int point;             //地图点

        public float MaxBlood => maxBlood;
        public float Combat => combat;
        public float Blood => currentBlood;
        public Vector2Int Point => point;
        public GameObject Troop => troop;
        public Color Country => countryColor;
        private static GameObject parent = GameObject.Find("Troops");


        private void SubMovement()
        {
            movement--;
            countSprite.sprite = Resources.Load("Sprites/" + movement.ToString(), typeof(Sprite)) as Sprite;
        }

        public void Recover()
        {
            movement = maxMovement;
            countSprite.sprite = Resources.Load("Sprites/" + movement.ToString(), typeof(Sprite)) as Sprite;
        }

        public Vector2Int getPoint(int direct)
        {
            switch (direct)
            {
                case 0:
                    {
                        if(point.x % 2 == 0)
                        {
                            return new Vector2Int(point.x, point.y + 1);
                        }
                        else
                        {
                            return new Vector2Int(point.x, point.y + 1);
                        }
                    }                    
                case 1:
                    if (point.x % 2 == 0)
                    {
                        return new Vector2Int(point.x + 1, point.y);
                    }
                    else
                    {
                        return new Vector2Int(point.x + 1, point.y + 1);
                    }
                case 2:
                    if (point.x % 2 == 0)
                    {
                        return new Vector2Int(point.x+1, point.y - 1);
                    }
                    else
                    {
                        return new Vector2Int(point.x+1, point.y);
                    }
                case 3:
                    if (point.x % 2 == 0)
                    {
                        return new Vector2Int(point.x, point.y - 1);
                    }
                    else
                    {
                        return new Vector2Int(point.x, point.y - 1);
                    }
                case 4:
                    if (point.x % 2 == 0)
                    {
                        return new Vector2Int(point.x-1, point.y - 1);
                    }
                    else
                    {
                        return new Vector2Int(point.x-1, point.y);
                    }
                case 5:
                    if (point.x % 2 == 0)
                    {
                        return new Vector2Int(point.x-1, point.y);
                    }
                    else
                    {
                        return new Vector2Int(point.x-1, point.y + 1);
                    }
            }
            return new Vector2Int();
        }

        public int getDirect(Vector2Int direct)
        {
            Vector2Int v0;
            Vector2Int v1;
            Vector2Int v2;
            Vector2Int v3;
            Vector2Int v4;
            Vector2Int v5;
            if (point.x % 2 == 0)
            {
                v0 = new Vector2Int(point.x, point.y + 1);
                v1 = new Vector2Int(point.x+1, point.y);
                v2 = new Vector2Int(point.x+1, point.y - 1);
                v3 = new Vector2Int(point.x, point.y - 1);
                v4 = new Vector2Int(point.x-1, point.y - 1);
                v5 = new Vector2Int(point.x-1, point.y);
            }
            else
            {
                v0 = new Vector2Int(point.x, point.y + 1);
                v1 = new Vector2Int(point.x + 1, point.y+1);
                v2 = new Vector2Int(point.x + 1, point.y);
                v3 = new Vector2Int(point.x, point.y - 1);
                v4 = new Vector2Int(point.x - 1, point.y);
                v5 = new Vector2Int(point.x - 1, point.y+1);
            }
            if(direct == v0)
            {
                return 0;
            }
            if (direct == v1)
            {
                return 1;
            }
            if (direct == v2)
            {
                return 2;
            }
            if (direct == v3)
            {
                return 3;
            }
            if (direct == v4)
            {
                return 4;
            }
            if (direct == v5)
            {
                return 5;
            }
            return -1;
        }

        private void Dead()
        {
            GameManager.Instance.units[point.x, point.y].troop = null;
            troop.SetActive(false);
            UnityEngine.Object.Destroy(troop, 1f);
        }

        private void UpDateBlood()
        {
            Vector3 scale = troop.transform.GetChild(3).GetChild(0).localScale;
            scale.x = currentBlood / maxBlood;
            troop.transform.GetChild(2).GetChild(0).localScale = scale;
        }

        public Troops(Color countryColor, GameObject map)
        {
            point = GameManager.Instance.Find(map);
            this.countryColor = countryColor;
            INIParser iniParser = new INIParser();                                                  //读取数据
            iniParser.Open(Application.streamingAssetsPath + "/Sword.ini");
            type = (TroopType)Enum.Parse(typeof(TroopType), GetType().ToString().Split('.')[1]);
            maxBlood = iniParser.ReadValue(GetType().ToString().Split('.')[1], "maxBlood", 0);
            combat = iniParser.ReadValue(GetType().ToString().Split('.')[1], "combat", 0);
            maxMovement = iniParser.ReadValue(GetType().ToString().Split('.')[1], "maxMovement", 0);
            iniParser.Close();

            troop = UnityEngine.Object.Instantiate((GameObject)Resources.Load("Prefabs/Troops"));   //到如预制体并复制一份
            troop.transform.position = map.transform.position;                                      //设定位置
            countrySprite = troop.GetComponent<SpriteRenderer>();                                   //设定国家颜色
            troopSprite = troop.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();     //设定类别
            countSprite = troop.transform.GetChild(2).GetComponentInChildren<SpriteRenderer>(); //行动力

            countSprite.sprite = Resources.Load("Sprites/" + maxMovement.ToString(), typeof(Sprite)) as Sprite;
            countrySprite.color = countryColor;
            string line = "Sprites/" + type;
            troopSprite.sprite = Resources.Load(line, typeof(Sprite)) as Sprite;
            troop.transform.parent = parent.transform;

            GameManager.Instance.units[point.x, point.y].troop = this;
            direction = troop.transform.GetChild(1).gameObject;

            //初始化数据
            currentBlood = maxBlood;
            movement = maxMovement;
        }

        public void Injured(float value)
        {
            currentBlood = currentBlood - value;
            Debug.Log("blood : " + currentBlood);
            if (currentBlood <= 5)
            {
                Dead();
            }
            else
            {
                UpDateBlood();
            }
        }

        public void Attack(Troops Enemy)
        {
            if (movement < 1)
                return;
            SubMovement();
            GameManager.Instance.Effect.GetComponent<ParticleSystem>().Stop(true);
            GameManager.Instance.Effect.transform.position = Enemy.Troop.transform.position;
            GameManager.Instance.Effect.GetComponent<ParticleSystem>().Play(true);
            float mValue = combat * currentBlood / maxBlood;
            float eValue = Enemy.Combat * (Enemy.Blood /Enemy.MaxBlood);
            mValue = mValue < 5 ? 5 : mValue;
            eValue = eValue < 5 ? 5 : eValue;
            Enemy.Injured(mValue);
            Injured(eValue);
        }

        public void HideDirection()
        {
            direction.SetActive(false);
            for (int i = 0; i < 6; i++)
            {
                direction.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        public void ShowDirection()
        {
            if (movement < 1)
                return;
            direction.SetActive(true);
            for(int i = 0; i < 6; i++)
            {
                if (!CheckDirect(i))
                {
                    direction.transform.GetChild(i).gameObject.SetActive(false);
                }
                if (IsEnemy(i))
                {
                    direction.transform.GetChild(i).gameObject.SetActive(true);
                    int k = i % 3 == 0 ? 0 : 1;
                    direction.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/DirectionRed"+k, typeof(Sprite)) as Sprite;
                }
                else
                {
                    int k = i % 3 == 0 ? 0 : 1;
                    direction.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/DirectionGreen" + k, typeof(Sprite)) as Sprite;
                }
            }
        }

        protected bool IsEnemy(int direct)
        {
            try
            {
                switch (direct)
                {
                    case 0:
                        {
                            if (GameManager.Instance.units[point.x, point.y + 1].troop != null && GameManager.Instance.units[point.x, point.y + 1].troop.Country != countryColor)
                            {
                                return true;
                            }
                            break;
                        }
                    case 1:
                        {
                            if (point.x % 2 == 0)
                            {
                                if (GameManager.Instance.units[point.x + 1, point.y].troop != null && GameManager.Instance.units[point.x + 1, point.y].troop.Country != countryColor)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.units[point.x + 1, point.y + 1].troop != null && GameManager.Instance.units[point.x + 1, point.y + 1].troop.Country != countryColor)
                                {
                                    return true;
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            if (point.x % 2 == 0)
                            {
                                if (GameManager.Instance.units[point.x + 1, point.y - 1].troop != null && GameManager.Instance.units[point.x + 1, point.y - 1].troop.Country != countryColor)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.units[point.x + 1, point.y].troop != null && GameManager.Instance.units[point.x + 1, point.y].troop.Country != countryColor)
                                {
                                    return true;
                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            if (GameManager.Instance.units[point.x, point.y - 1].troop != null && GameManager.Instance.units[point.x, point.y - 1].troop.Country != countryColor)
                            {
                                return true;
                            }
                            break;
                        }
                    case 4:
                        {
                            if (point.x % 2 == 0)
                            {
                                if (GameManager.Instance.units[point.x - 1, point.y - 1].troop != null && GameManager.Instance.units[point.x - 1, point.y - 1].troop.Country != countryColor)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.units[point.x - 1, point.y].troop != null && GameManager.Instance.units[point.x - 1, point.y].troop.Country != countryColor)
                                {
                                    return true;
                                }
                            }
                            break;
                        }
                    case 5:
                        {
                            if (point.x % 2 == 0)
                            {
                                if (GameManager.Instance.units[point.x - 1, point.y].troop != null && GameManager.Instance.units[point.x - 1, point.y].troop.Country != countryColor)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.units[point.x - 1, point.y + 1].troop != null && GameManager.Instance.units[point.x - 1, point.y + 1].troop.Country != countryColor)
                                {
                                    return true;
                                }
                            }
                            break;
                        }
                    default:
                        return false;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            return false;
        }

        protected bool CheckDirect(int direct)
        {
            try
            {
                switch (direct)
                {
                    case 0:
                        {
                            if (GameManager.Instance.units[point.x, point.y + 1].troop != null || GameManager.Instance.units[point.x, point.y + 1].unit.tag.Equals("OceanLand"))
                            {
                                return false;
                            }
                            break;
                        }
                    case 1:
                        {
                            if (point.x % 2 == 0)
                            {
                                if (GameManager.Instance.units[point.x + 1, point.y].troop != null || GameManager.Instance.units[point.x + 1, point.y].unit.tag.Equals("OceanLand"))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.units[point.x + 1, point.y + 1].troop != null || GameManager.Instance.units[point.x + 1, point.y + 1].unit.tag.Equals("OceanLand"))
                                {
                                    return false;
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            if (point.x % 2 == 0)
                            {
                                if (GameManager.Instance.units[point.x + 1, point.y - 1].troop != null || GameManager.Instance.units[point.x + 1, point.y - 1].unit.tag.Equals("OceanLand"))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.units[point.x + 1, point.y].troop != null || GameManager.Instance.units[point.x + 1, point.y].unit.tag.Equals("OceanLand"))
                                {
                                    return false;
                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            if (GameManager.Instance.units[point.x, point.y - 1].troop != null || GameManager.Instance.units[point.x, point.y - 1].unit.tag.Equals("OceanLand"))
                            {
                                return false;
                            }
                            break;
                        }
                    case 4:
                        {
                            if (point.x % 2 == 0)
                            {
                                if (GameManager.Instance.units[point.x - 1, point.y - 1].troop != null || GameManager.Instance.units[point.x - 1, point.y - 1].unit.tag.Equals("OceanLand"))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.units[point.x - 1, point.y].troop != null || GameManager.Instance.units[point.x - 1, point.y].unit.tag.Equals("OceanLand"))
                                {
                                    return false;
                                }
                            }
                            break;
                        }
                    case 5:
                        {
                            if (point.x % 2 == 0)
                            {
                                if (GameManager.Instance.units[point.x - 1, point.y].troop != null || GameManager.Instance.units[point.x - 1, point.y].unit.tag.Equals("OceanLand"))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (GameManager.Instance.units[point.x - 1, point.y + 1].troop != null || GameManager.Instance.units[point.x - 1, point.y + 1].unit.tag.Equals("OceanLand"))
                                {
                                    return false;
                                }
                            }
                            break;
                        }
                    default:
                        return false;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            return true;
        }

        public bool Move(int direct)
        {
            if (movement < 1)
                return false;
            SubMovement();
            Vector2Int vector =  getPoint(direct);
            if (!GameManager.Instance.units[vector.x,vector.y].unit.tag.Equals("OceanLand"))
            {
                GameManager.Instance.units[point.x, point.y].troop = null;     //删除自己的位置，并重新注册
                point = vector;
                GameManager.Instance.units[point.x, point.y].troop = this;
                GameManager.Instance.moveTroop = this;
                return true;
            }
            return false;
        }

    }

    class Militia : Troops
    {
        public Militia(GameObject map, Color countryColor) : base(countryColor, map)
        { 
            
        }
    }

    class Infantry : Troops
    {
        public Infantry(GameObject map, Color countryColor) : base(countryColor, map)
        {

        }
    }

    class Artillery : Troops
    {
        public Artillery(GameObject map, Color countryColor) : base(countryColor, map)
        {

        }
    }

    class Armour : Troops
    {
        public Armour(GameObject map, Color countryColor) : base(countryColor, map)
        {

        }
    }

    class Scout : Troops
    {
        public Scout(GameObject map, Color countryColor) : base(countryColor, map)
        {

        }
    }
    //Infantry, Scout, Militia, Artillery, Armour

}
