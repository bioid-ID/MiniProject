using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    private readonly List<BuffBase> buffs = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddBuff(BuffBase buff)
    {
        if (buff == null)
            return;

        buffs.Add(buff);

        buff.Initialize(PlayerStat.Instance);

        buff.Apply();
    }

    public void RemoveBuff(BuffBase buff)
    {
        if (!buffs.Contains(buff))
            return;

        buff.Remove();

        buffs.Remove(buff);
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            buffs[i].UpdateBuff(dt);

            if (!buffs[i].Finished)
                continue;

            buffs[i].Remove();

            buffs.RemoveAt(i);
        }
    }
}