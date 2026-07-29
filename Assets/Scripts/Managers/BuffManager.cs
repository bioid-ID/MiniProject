using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance { get; private set; }

    private readonly List<BuffBase> buffs = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddBuff(BuffBase buff)
    {
        if (buff == null)
            return;

        BuffBase runtime = Instantiate(buff);

        runtime.OnApply(PlayerStat.Instance);

        buffs.Add(runtime);
    }

    public void RemoveBuff(BuffBase buff)
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            if (buffs[i].name != buff.name)
                continue;

            buffs[i].OnRemove(PlayerStat.Instance);

            Destroy(buffs[i]);

            buffs.RemoveAt(i);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            buffs[i].Tick(PlayerStat.Instance, dt);

            if (!buffs[i].IsFinished)
                continue;

            buffs[i].OnRemove(PlayerStat.Instance);

            Destroy(buffs[i]);

            buffs.RemoveAt(i);
        }
    }
}