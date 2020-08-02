using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HelpTool
{
    public class Units
    {
        public GameObject unit = null;
        public Troops troop = null;
        protected Color country = Color.white;
        protected Vector3Int property;
        public Player ConquerPlayer { get; private set; }
        public Color Country => country;
        public Vector3Int Property
        {
            get
            {
                return property;
            }
        }

        public void Conquer(Player player)
        {
            ConquerPlayer = player;
            country = player.Country;
            unit.GetComponent<SpriteRenderer>().color = country;
        }

        public void SetName(string name)
        {
            unit.name = name;
        }

        public Units()
        {
            //this.unit = unit;
            //unit.transform.parent = GameManager.Instance.Mapunits.transform;
        }

    }
}
