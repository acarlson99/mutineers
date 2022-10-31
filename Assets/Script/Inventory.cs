using System;
using System.Collections.Generic;

[Serializable]
public struct STRINT
{
    public EWeaponType s;
    public int i;
}

// negative number means infinity
public class Inventory : Dictionary<EWeaponType, int>
{
    public Inventory(IEnumerable<STRINT> ls)
    {
        //Debug.Log($"Inventory init with {ls}");
        foreach (var item in ls)
        {
            //Debug.Log($"ADD ITEM {item.i} {item.s}");
            if (item.i > 0) Add(item.s, item.i);
            else if (item.i < 0) Add(item.s, -1);
        }
    }

    // mark item as used, return false if unable
    public bool UseItem(EWeaponType name)
    {
        if (CanUseItem(name))
        {
            if (this[name] > 0) this[name] -= 1;
            if (this[name] == 0)
            {
                Remove(name);
            }
            return true;
        }
        return false;
    }

    public void Add(EWeaponType name)
    {
        if (!ContainsKey(name)) Add(name, 1);
        else if (this[name] > 0) Add(name, this[name] + 1);
    }

    public int Get(EWeaponType name)
    {
        if (!CanUseItem(name)) return 0;
        return this[name];
    }

    public bool CanUseItem(EWeaponType name)
    {
        return ContainsKey(name) && this[name] != 0;
    }
}
