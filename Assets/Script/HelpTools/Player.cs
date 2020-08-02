using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HelpTool
{
    public class Player
    {
        public string PlayerName{ get; private set; }
        private List<Troops> allTroops = new List<Troops>();
        private List<Units> allUnit = new List<Units>();

        public Color Country { get; }
        public Vector3Int Property { get; private set; }
        public Vector3 Center { get; private set; }

        public Player(Color color,Vector2Int point,string name)
        {
            PlayerName = name;
            Country = color;
            Center = GameManager.Instance.units[point.x, point.y].unit.transform.position;
            Conquer(GameManager.Instance.units[point.x, point.y]);
            Property = new Vector3Int(300,200,100);
        }

        public void AddCity(City city,Units unit)
        {
            INIParser iniParser = new INIParser();                                                  //读取数据
            iniParser.Open(Application.streamingAssetsPath + "/Shovel.ini");
            int agriculture = iniParser.ReadValue(city.ToString(), "Agriculture", 0);
            int industry = iniParser.ReadValue(city.ToString(), "Industry", 0);
            int science = iniParser.ReadValue(city.ToString(), "Science", 0);
            iniParser.Close();
            GrassLand land = unit as GrassLand;
            if (IsBig(new Vector3Int(agriculture, industry, science), Property) && allUnit.Contains(unit) && land.AddCity(city))
            {
                Pay(new Vector3Int(agriculture, industry, science));
            }

        }

        public Troops BuildTroops(Units hitMap,TroopType troopType)
        {
            if(allUnit.Contains(hitMap) && !hitMap.unit.tag.Equals("OceanLand"))
            {
                INIParser iniParser = new INIParser();                                                  //读取数据
                iniParser.Open(Application.streamingAssetsPath + "/Sword.ini");
                int agriculture = iniParser.ReadValue(troopType.ToString(), "Agriculture", 0);
                int industry = iniParser.ReadValue(troopType.ToString(), "Industry", 0);
                int science = iniParser.ReadValue(troopType.ToString(), "Science", 0);
                int copper = iniParser.ReadValue(troopType.ToString(), "Copper", 0);
                int silver = iniParser.ReadValue(troopType.ToString(), "Silver", 0);
                int blue = iniParser.ReadValue(troopType.ToString(), "Blue", 0);
                iniParser.Close();
                GrassLand grassLand = hitMap as GrassLand;
                if(IsBig(new Vector3Int(agriculture, industry, science),Property) && IsBig(new Vector3Int(copper, silver, blue),grassLand.Level))
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    object[] parameters = new object[2];
                    parameters[0] = hitMap.unit;
                    parameters[1] = GameManager.Instance.CurrentCountry;
                    object o = assembly.CreateInstance("HelpTool." + troopType, true, BindingFlags.Default, null, parameters, null, null);
                    Troops troops = o as Troops;
                    Debug.Log(o.ToString());
                    Pay(new Vector3Int(agriculture, industry, science));
                    allTroops.Add(troops);
                    return troops;
                }
            }
            return null;
        }

        private bool IsBig(Vector3Int small, Vector3Int big)
        {
            if (big.x < small.x || big.y < small.y || big.z < small.z)
            {
                Debug.Log("not Enough" + big  + "--"+ small);
                return false;
            }
            return true;
        }

        private void Pay(Vector3Int property)
        {
            Property -= property;
            GameManager.Instance.cameraManager.ShowHead(Property);
        }

        private void Dead()
        {
            Debug.Log(PlayerName + "Dead");
            GameManager.Instance.PlayerDead(this);
        }

        public Vector3Int Next()
        {
            foreach(Units unit in allUnit)
            {
                Property += unit.Property;
            }
            if (allTroops.Count > 0)
            {
                foreach (Troops troops in allTroops)
                {
                    troops.Recover();
                }
            }
            return Property;
        }

        public void Conquer(Units unit)
        {
            if (!unit.unit.tag.Equals("OceanLand"))
            {
                if (unit.ConquerPlayer != this)
                {
                    allUnit.Add(unit);
                    if(unit.ConquerPlayer != null)
                    {
                        unit.ConquerPlayer.allUnit.Remove(unit);
                        if (unit.ConquerPlayer.allUnit.Count == 0)
                        {
                            unit.ConquerPlayer.Dead();
                        }
                    }
                    unit.Conquer(this);
                }
            }
            
        }
    }
}
