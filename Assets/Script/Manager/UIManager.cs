using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HelpTool;
using UnityEngine.EventSystems;
using System.Configuration;
using System;

public class UIManager : MonoBehaviour
{
    public static bool Clicked = false;
    public static int Opened = 0;
    public GameObject panel;
    public GameObject buildPanel;
    public GameObject List;
    private string currentClick = "Sword";
    private Units hitMap;

    private void Awake()
    {
        GameManager.Instance.uIManager = this;
    }

    public void Close()
    {
        hitMap = null;
        Opened = 1;
        buildPanel.SetActive(false);
        for (int i = 0; i < List.transform.GetChild(0).childCount; i++)
        {
            Destroy(List.transform.GetChild(0).GetChild(i).gameObject);
        }
        GameManager.Instance.CanelSelect();
    }

    public void BuildClick()
    {
        if (GameManager.Instance.hitMap != null)
        {
            hitMap = GameManager.Instance.hitMap;
        }
        else
        {
            return;
        }
        Opened = 2;
        ShowList();
        buildPanel.SetActive(true);
    }

    public void NextClick()
    {
        Clicked = true;        
        GameManager.Instance.NextTime();             
        GameManager.Instance.CanelSelect();
        ShowPanel(GameManager.Instance.currentPlayer.PlayerName + "的回合",1);
    }

    public void ShowPanel(string text,int time)
    {
        panel.SetActive(true);
        panel.GetComponentInChildren<Text>().text = text;
        Invoke("DestoryText", time);
    }

    private void DestoryText()
    {
        panel.SetActive(false);
    }

    public void SwordClick()
    {
        List.GetComponent<Image>().color = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color;      //更改背景颜色
        for (int i = 0; i < List.transform.GetChild(0).childCount; i++)                                                     //删除物体
        {
            Destroy(List.transform.GetChild(0).GetChild(i).gameObject);
        }
        currentClick = EventSystem.current.currentSelectedGameObject.name;
        ShowList();
    }

    private void ShowList()
    {
        INIParser iniParser = new INIParser();        
        iniParser.Open(Application.streamingAssetsPath + "/" + EventSystem.current.currentSelectedGameObject.name + ".ini");
        foreach (int i in Enum.GetValues(GetTypeByString(currentClick)))
        {
            string strName = Enum.GetName(GetTypeByString(currentClick), i);
            int agriculture = iniParser.ReadValue(strName, "Agriculture", 0);
            int industry = iniParser.ReadValue(strName, "Industry", 0);
            int science = iniParser.ReadValue(strName, "Science", 0);
            GameObject item = Instantiate((GameObject)Resources.Load("Prefabs/Item"));
            item.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load("Sprites/" + strName, typeof(Sprite)) as Sprite;
            item.transform.GetChild(1).GetComponent<Text>().text = strName;
            item.GetComponent<Button>().onClick.AddListener(ItemClick);
            GameObject info = item.transform.GetChild(2).gameObject;                                    //设置价格
            info.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = agriculture.ToString();
            info.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = industry.ToString();
            info.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = science.ToString();
            item.transform.SetParent(List.transform.GetChild(0));
        }
        iniParser.Close();
    }

    public void ItemClick()
    {
        string name = EventSystem.current.currentSelectedGameObject.gameObject.transform.GetChild(1).GetComponent<Text>().text;    //获取名字
        int x = int.Parse(EventSystem.current.currentSelectedGameObject.gameObject.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text);
        int y = int.Parse(EventSystem.current.currentSelectedGameObject.gameObject.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text);
        int z = int.Parse(EventSystem.current.currentSelectedGameObject.gameObject.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Text>().text);
        Vector3Int property = new Vector3Int(x,y,z);
        if (CanPay(property))
        {
            switch (currentClick)
            {
                case "Sword":
                    {
                        GameManager.Instance.currentPlayer.BuildTroops(hitMap, (TroopType)Enum.Parse(typeof(TroopType),name));
                        break;
                    }
                case "Shovel":
                    {
                        GameManager.Instance.currentPlayer.AddCity((City)Enum.Parse(typeof(City),name),hitMap);
                        break;
                    }
            }
            Close();            
        }
    }

    private bool CanPay(Vector3Int property)
    {
        Vector3Int current = GameManager.Instance.currentPlayer.Property;
        if(property.x<current.x || property.y < current.y || property.z < current.z)
        {
            return true;
        }
        return false;
    }

    private Type GetTypeByString(string t)
    {
        switch (t)
        {
            case "Sword":
                return typeof(TroopType);
            case "Shovel":
                return typeof(City);
            case "Flag":
                return typeof(TroopType);
        }
        return typeof(TroopType);
    }
}
