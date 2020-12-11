﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Combat;

[RequireComponent(typeof(Combatant))]
public class HealthUI : MonoBehaviour
{
    private Combatant combatant; 

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;


    // Update is called once per frame
    void Update()
    {
        combatant = GetComponent<Combatant>();

        for (int i = 0; i < hearts.Length; i++)
        {


            //Show full or half heart
            if (i < combatant.Health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            // Show max health on the screen
            if (i < combatant.MaxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

}
