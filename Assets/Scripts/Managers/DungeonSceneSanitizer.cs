using UnityEngine;

public static class DungeonSceneSanitizer
{
    public static GameObject PreparePlayer()
    {
        RemoveBrokenSceneEnemies();
        RemoveDuplicatePoolManagers();
        return ConsolidatePlayers();
    }

    private static void RemoveBrokenSceneEnemies()
    {
        Enemy[] sceneEnemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in sceneEnemies)
        {
            if (enemy == null)
                continue;

            Object.Destroy(enemy.gameObject);
        }

        GameObject legacyEnemy = GameObject.Find("Enemy");
        if (legacyEnemy != null && legacyEnemy.GetComponent<Enemy>() == null)
            Object.Destroy(legacyEnemy);
    }

    private static void RemoveDuplicatePoolManagers()
    {
        PoolManager[] poolManagers = Object.FindObjectsByType<PoolManager>(FindObjectsSortMode.None);
        if (poolManagers.Length <= 1)
            return;

        for (int i = 1; i < poolManagers.Length; i++)
        {
            if (poolManagers[i] != null)
                Object.Destroy(poolManagers[i].gameObject);
        }
    }

    private static GameObject ConsolidatePlayers()
    {
        GameObject[] taggedPlayers = GameObject.FindGameObjectsWithTag("Player");
        GameObject bestPlayer = SelectBestPlayer(taggedPlayers);

        if (bestPlayer == null)
        {
            GameObject namedPlayer = GameObject.Find("Player");
            if (namedPlayer != null)
            {
                namedPlayer.tag = "Player";
                bestPlayer = namedPlayer;
            }
        }

        if (bestPlayer == null)
            return null;

        foreach (GameObject candidate in taggedPlayers)
        {
            if (candidate == null || candidate == bestPlayer)
                continue;

            Debug.LogWarning($"DungeonSceneSanitizer: Removing duplicate Player '{candidate.name}'.");
            Object.Destroy(candidate);
        }

        CleanupBrokenMeleeChild(bestPlayer);
        return bestPlayer;
    }

    private static GameObject SelectBestPlayer(GameObject[] candidates)
    {
        GameObject best = null;
        int bestScore = int.MinValue;

        foreach (GameObject candidate in candidates)
        {
            if (candidate == null)
                continue;

            int score = ScorePlayer(candidate);
            if (score <= bestScore)
                continue;

            bestScore = score;
            best = candidate;
        }

        return best;
    }

    private static int ScorePlayer(GameObject playerObject)
    {
        int score = 0;

        if (playerObject.GetComponent<PlayerStat>() != null)
            score += 20;

        if (playerObject.GetComponent<SpriteRenderer>() != null)
            score += 10;

        if (playerObject.GetComponent<CircleCollider2D>() != null)
            score += 10;

        if (playerObject.GetComponent<Rigidbody2D>() != null)
            score += 8;

        if (playerObject.GetComponent<PlayerMovement>() != null)
            score += 5;

        if (playerObject.GetComponent<PlayerHealth>() != null)
            score += 5;

        return score;
    }

    private static void CleanupBrokenMeleeChild(GameObject playerObject)
    {
        Transform meleeTransform = playerObject.transform.Find("MeleeHitbox");
        if (meleeTransform == null)
            return;

        MeleeAttack misplacedMeleeAttack = meleeTransform.GetComponent<MeleeAttack>();
        if (misplacedMeleeAttack != null)
            Object.Destroy(misplacedMeleeAttack);

        if (meleeTransform.GetComponent<Hitbox>() == null)
            meleeTransform.gameObject.SetActive(false);
    }
}
