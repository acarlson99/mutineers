using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Linq;

public class TeamHealthBar : HealthBar
{
    public int teamNum = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (teamNum == 0) bar.color = Color.red;
        else bar.color = Color.blue;
    }

    public TeamHealthBar()
    {
        maxHealth = 0;
        currentHealth = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        float h = (from bar in healthBars
                   select bar.currentHealth).Sum();
        float mh = (from bar in healthBars
                    select bar.maxHealth).Sum();
        currentHealth = h;
        maxHealth = mh;
        base.Update();
    }

    private List<HealthBar> healthBars = new();

    public void AddToPool(HealthBar h)
    {
        healthBars.Add(h);
    }
}
