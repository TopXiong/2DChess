using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorDropdown : MonoBehaviour
{
    public Dropdown dropdown;
    public GameObject Image;

    private void Start()
    {
        dropdown.options.Clear();
        Dropdown.OptionData optionData = new Dropdown.OptionData();
        Texture2D texture2D = new Texture2D(256, 128);
        for(int i = 0; i < texture2D.width; i++)
        {
            for(int j = 0; j < texture2D.height; j++)
            {
                texture2D.SetPixel(i, j, Color.yellow);
            }
        }
        texture2D.Apply();
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(128,64));
        optionData.text = "asd";
        Image.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/build");
        optionData.image = Resources.Load<Sprite>("Sprites/build");
        //Image.GetComponent<Image>().sprite = sprite;
        dropdown.options.Add(optionData);
        
    }

    public void OnDropDown()
    {
        Debug.Log("0");
    }
}
