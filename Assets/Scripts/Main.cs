using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public Text uiTextScore;
    public Text uiHighScore;
    public Text uiDifficalty;
    [Space(10)]
    public GameObject[] prefubEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    [Space(10)]
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield
    };
    [Space(10)]
    public TextAsset diffXML;
    public Level level;
    [System.NonSerialized]
    public int score = 0;

    private BoundsCheck _bndCheck;
    private int _highScore = 2500;
    private const int _maxScore = 1000000;

    private string[] _difficulties = { "easy", "normal", "hard" };

    // Start is called before the first frame update
    void Awake()
    {
        S = this;
        _bndCheck = GetComponent<BoundsCheck>();
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
            WEAP_DICT[def.type] = def;

        if (PlayerPrefs.HasKey("HighScore"))
        {
            _highScore = PlayerPrefs.GetInt("HighScore");
        }
        PlayerPrefs.SetInt("HighScore", _highScore);

        int currDifficulty = PlayerPrefs.GetInt("Difficulty");
        LoadEnemyPresset(_difficulties[currDifficulty]);
        uiDifficalty.text = _difficulties[currDifficulty];


        if (!AudioManager.instance.IsSoundOn("Theme"))
            AudioManager.instance.Play("Theme");

        Invoke("SpawnWave", 0);
    }

    void Start()
    {
        UpdateGUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnWave()
    {
        int ndx = Random.Range(0, level.waves.Length);
        StartCoroutine(SpawnEnemy(level.waves[ndx]));
    }

    IEnumerator SpawnEnemy(Wave wave)
    {
        //Оптимизировать
        while (!wave.IsEnd)
        {
            var go = Instantiate(wave.GetShipPrefub());
            float enemyPadding = enemyDefaultPadding;
            if (go.GetComponent<BoundsCheck>() != null)
            {
                enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
            }

            var pos = Vector3.zero;
            float xMin = -_bndCheck.camWidth + enemyPadding;
            float xMax = _bndCheck.camWidth - enemyPadding;
            pos.x = Random.Range(xMin, xMax);
            pos.y = _bndCheck.camHeight + enemyPadding;
            go.transform.position = pos;
            yield return new WaitForSeconds(1f / enemySpawnPerSecond);
        }
        wave.IsEnd = false;
        if (wave.delayNextWave)
            Invoke("SpawnWave", 1f / wave.delayBeforeWave);
        else
            Invoke("SpawnWave", 0);
    }

    public void ShipDestroyed(Enemy e)
    {
        if (Random.value <= e.poweUpDropChance)
        {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            var go = Instantiate(prefabPowerUp);
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);
            pu.transform.position = e.transform.position;
        }
        AudioManager.instance.Play("EnemyDestroyed");
        score += e.score;
        UpdateGUI();
    }

    public void DelayedRestart(float delay)
    {
        //Invoke("Restart", delay);
        Invoke("ToMainMenu", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
            return WEAP_DICT[wt];

        return new WeaponDefinition();
    }

    void UpdateGUI()
    {
        if (score > _maxScore) score = _maxScore;

        if (score > _highScore)
        {
            _highScore = score;
            PlayerPrefs.SetInt("HighScore", _highScore);
        }
        uiTextScore.text = $"{score}";
        uiHighScore.text = $"{_highScore}";
    }

    void LoadEnemyPresset(string difficulty)
    {
        level = LevelXmlSaver.Load(diffXML.name).Levels.Where(l => l.name == difficulty).Single();
        foreach (var wave in level.waves)
        {
            wave.InitShipsPrefub();
            for (int i = 0; i < wave.ships.Length; i++)
            {
                wave.SetShipPrefub(prefubEnemies.Where(e => e.name == wave.ships[i]).Single());
            }
        }
    }
}
