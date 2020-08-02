using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HelpTool
{
    public class GrassLand : Units
    {
        private static GameObject gressLand = (GameObject)Resources.Load("Prefabs/GrassLand");
        private Vector3Int level;   //每个城市的等级
        private GameObject[] citys = new GameObject[3];

        public Vector3Int Level => level;

        public bool AddCity(City city)
        {
            Vector3Int temp =  level;
            temp[(int)city]++;
            Debug.Log(temp.magnitude);
            for (int i = 0; i < 3; i++)
            {
                if (temp[i] > 5 - i)
                {
                    return false;
                }
            }
            INIParser iniParser = new INIParser();                                                  //读取数据
            iniParser.Open(Application.streamingAssetsPath + "/Shovel.ini");
            int value = iniParser.ReadValue(city.ToString(), "Value", 0);
            iniParser.Close();
            property[(int)city] += value;
            level = temp;
            ShowCityLevel();
            return true;
        }

        private void ShowCityLevel()
        {
            for (int i = 0; i < citys.Length; i++)
            {
                citys[i].SetActive(level[i] == 0 ? false : true);
                if (citys[i].activeSelf)
                {
                    citys[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/" + level[i].ToString(), typeof(Sprite)) as Sprite;
                }
            }
        }

        public GrassLand(Vector2 vector)
        {
            System.Random random = new System.Random();
            property = new Vector3Int(random.Next(1,3), 1, 1);            
            unit = GameObject.Instantiate(gressLand, vector, new Quaternion());
            unit.transform.parent = GameManager.Instance.Mapunits.transform;
            for(int i = 0; i < citys.Length; i++)
            {
                citys[i] = unit.transform.GetChild(i).gameObject;
            }
            ShowCityLevel();
        }
    }

    public class OceanLand : Units
    {
        private static GameObject oceanLand = (GameObject)Resources.Load("Prefabs/OceanLand");
        public OceanLand(Vector2 vector)
        {
            unit = GameObject.Instantiate(oceanLand, vector, new Quaternion());
            unit.transform.parent = GameManager.Instance.Mapunits.transform;
            property = new Vector3Int(0, 0, 0);
        }
    }
}
