using System.Collections.Generic;
using System.Linq;

public class TimerMux<T> : Dictionary<T, float>
{
    public void Update(float deltaTime)
    {
        foreach (T key in Keys.ToList())
        {
            this[key] += deltaTime;
        }
    }

    public void Add(T key)
    {
        if (!ContainsKey(key)) Add(key, 0);
    }
}
