using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyDropDown : Dropdown
{
    public new class OptionData
    {       
        public string text { get; set; }
        
        public Image image { get; set; }
    }
}
