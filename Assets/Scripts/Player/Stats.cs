using Terresquall;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : PersistentObject {

    private const string DEFAULT_SAVE_ID = "PlayerStatsData";
    // Create the SaveData object, which specifies what data needs to be saved.
    [System.Serializable]
    public new class SaveData : PersistentObject.SaveData {
        public int health = 200, power = 0, attackDamage = 100, defense = 0;
        protected float x, y;

        // Shorthand to allow us to get / set the position instead of
        // manually setting the x and y in the save data.
        public Vector2 position {
            get { return new Vector2(x, y); }
            set { x = value.x; y = value.y; }
        }
    }

    // This variable allows us to access health, power, attackDamage and defense.
    public SaveData data = new SaveData();

    // We only need to record the position before saving.
    // The rest of the attributes are updated live by wrappers.
    public override PersistentObject.SaveData Save() {
        if(CanSave()) {
            data.saveID = saveID;
            data.position = transform.position;
            return data;
        }
        return null;
    }

    // Since wrappers get values from <data>, we only need to set it
    // and the position variable.
    public override bool Load(PersistentObject.SaveData loadedData) {
        data = loadedData as SaveData;
        if(data != null) {
            transform.position = data.position;
            return true;
        }
        return false;
    }
    
    // Wrappers that route the getting / setting to the data variable.
    public int health { 
        get { return data.health; }
        set { data.health = value; }
    }
    public int power { 
        get { return data.power; }
        set { data.power = value; }
    }
    public int attackDamage { 
        get { return data.attackDamage; } 
        set { data.attackDamage = value; }
    }
    public int defense { 
        get { return data.defense; }
        set { data.defense = value; }
    }

    public GameObject camera;
    public GameObject stats;


    public Image hearts;
    public Sprite fullHeart;
    public Sprite heart190;
    public Sprite heart180;
    public Sprite heart170;
    public Sprite heart160;
    public Sprite heart150;
    public Sprite heart140;
    public Sprite heart130;
    public Sprite heart120;
    public Sprite heart110;
    public Sprite heart100;
    public Sprite heart90;
    public Sprite heart80;
    public Sprite heart70;
    public Sprite heart60;
    public Sprite heart50;
    public Sprite heart40;
    public Sprite heart30;
    public Sprite heart20;
    public Sprite heart10;
    public Sprite emptyHeart;

    public Image powers;
    public Sprite fullPower;
    public Sprite power75;
    public Sprite power50;
    public Sprite power25;
    public Sprite emptyPower;

    public static Stats instance;
    public TextMeshProUGUI textDamage;
    public TextMeshProUGUI textDefense;
    public GameObject sonidoMuerte;
    public GameObject sonidoDaño;

    private bool once = false;

    // Start is called before the first frame update
    void Start() {
        if (string.IsNullOrEmpty(saveID)) {
            saveID = DEFAULT_SAVE_ID;
        }
        if (instance == null) {
            instance = this;
        }

        // Cogemos los datos guardados de las otras escenas
        if ((int)PlayerPrefs.GetInt("AttackDamage", 0) >= 100) {
            this.attackDamage = (int)PlayerPrefs.GetInt("AttackDamage", 0);
        }
        this.defense = (int)PlayerPrefs.GetInt("Defense", 0);
        // Actualiza los contadores 
        this.textDamage.text = "+" + attackDamage.ToString();
        this.textDefense.text = "+" + defense.ToString();
    }

    // Update is called once per frame
    void Update() {
        // Actualiza los contadores 
        this.textDamage.text = "+" + attackDamage.ToString();
        this.textDefense.text = "+" + defense.ToString();

        if (health <= 0) {
            gameObject.GetComponent<Animator>().SetBool("die", true);
            //gameObject.GetComponentInParent<PlayerController>().destroy();
            gameObject.GetComponentInParent<PlayerController>().isDead();
            camera.SetActive(true);
            stats.SetActive(false);
            if (!once) {
                Instantiate(sonidoMuerte);
                once = true;
            }
        }


        switch (health) {
            case int n when (n >= 200):            hearts.sprite = fullHeart; break;
            case int n when (n >= 190 && n < 200): hearts.sprite = heart190; break;
            case int n when (n >= 180 && n < 190): hearts.sprite = heart180; break;
            case int n when (n >= 170 && n < 180): hearts.sprite = heart170; break;
            case int n when (n >= 160 && n < 170): hearts.sprite = heart160; break;
            case int n when (n >= 150 && n < 160): hearts.sprite = heart150; break;
            case int n when (n >= 140 && n < 150): hearts.sprite = heart140; break;
            case int n when (n >= 130 && n < 140): hearts.sprite = heart130; break;
            case int n when (n >= 120 && n < 130): hearts.sprite = heart120; break;
            case int n when (n >= 110 && n < 120): hearts.sprite = heart110; break;
            case int n when (n >= 100 && n < 110): hearts.sprite = heart100; break;
            case int n when (n >= 90 && n < 100):  hearts.sprite = heart90; break;
            case int n when (n >= 80 && n < 90):   hearts.sprite = heart80; break;
            case int n when (n >= 70 && n <= 80):  hearts.sprite = heart70; break;
            case int n when (n >= 60 && n < 70):   hearts.sprite = heart60; break;
            case int n when (n >= 50 && n < 60):   hearts.sprite = heart50; break;
            case int n when (n >= 40 && n < 50):   hearts.sprite = heart40; break;
            case int n when (n >= 30 && n < 40):   hearts.sprite = heart30; break;
            case int n when (n >= 20 && n < 30):   hearts.sprite = heart20; break;
            case int n when (n >= 10 && n < 20):   hearts.sprite = heart10; break;
            case int n when (n < 10):              hearts.sprite = emptyHeart; break;
        }

        switch (power) {
            case 4: powers.sprite = fullPower; break;
            case 3: powers.sprite = power75; break;
            case 2: powers.sprite = power50; break;
            case 1: powers.sprite = power25; break;
            case 0: powers.sprite = emptyPower; break;
        }

    }

    public void takeDamage(int value) {
        if((value-defense) > 0) { 
            this.health -= (value-defense);
        }
        Instantiate(sonidoDaño);
    }

    public void takeTrueDamage(int value) {
        this.health -= value;
    }

    public void takePower(int value) {
        this.power += value;
    }

    public int getHealth() {
        return this.health;
    }

    public int getPower() {
        return this.power;
    }

    public void setHealth(int value) {
        this.health += value;
        if(this.health >= 200) {
            this.health = 200;
        }
    }

    public void addAttackDamage(int value) {
        this.attackDamage += value;
        PlayerPrefs.SetInt("AttackDamage", attackDamage);
    }

    public int getAttackDamage() {
        return this.attackDamage;
    }

    public void addDefense(int value) {
        this.defense += value;
        PlayerPrefs.SetInt("Defense", defense);
    }
}
