using UnityEngine;
using Terresquall;

public class PlayerBenchSave : PersistentObject
{
    private const string DefaultSaveID = "PlayerData";

    private Stats stats;
    private Rigidbody2D rb;

    [System.Serializable]
    public class PlayerSaveData : PersistentObject.SaveData
    {
        public float positionX;
        public float positionY;
        public float positionZ;

        public int health;
        public int power;
        public int attackDamage;
        public int defense;
    }

    private void Awake()
    {
        // Make sure this object always has the same ID.
        // Bench uses saveID to know which saved data belongs to which object.
        if (string.IsNullOrEmpty(saveID))
        {
            saveID = DefaultSaveID;
        }

        stats = GetComponentInChildren<Stats>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override PersistentObject.SaveData Save()
    {
        if (!CanSave())
        {
            return null;
        }

        if (stats == null)
        {
            stats = GetComponentInChildren<Stats>();
        }

        if (stats == null)
        {
            Debug.LogWarning("PlayerBenchSave could not find Stats component.");
            return null;
        }

        PlayerSaveData data = new PlayerSaveData();

        data.saveID = saveID;

        data.positionX = transform.position.x;
        data.positionY = transform.position.y;
        data.positionZ = transform.position.z;

        data.health = stats.health;
        data.power = stats.power;
        data.attackDamage = stats.attackDamage;
        data.defense = stats.defense;

        return data;
    }

    public override bool Load(PersistentObject.SaveData data)
    {
        PlayerSaveData playerData = data as PlayerSaveData;

        if (playerData == null)
        {
            return false;
        }

        if (stats == null)
        {
            stats = GetComponentInChildren<Stats>();
        }

        transform.position = new Vector3(
            playerData.positionX,
            playerData.positionY,
            playerData.positionZ
        );

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (stats != null)
        {
            stats.health = playerData.health;
            stats.power = playerData.power;
            stats.attackDamage = playerData.attackDamage;
            stats.defense = playerData.defense;
        }

        Debug.Log("Player data loaded with Bench Save System.");

        return true;
    }
}