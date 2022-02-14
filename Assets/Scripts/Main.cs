using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public GameObject[] prefubEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyDefaultPadding = 1.5f;
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {
        WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield
    };
    public TextAsset diffXML;
    public Level level;

    private BoundsCheck bndCheck;
    private int _score = 0;
    private int _highScore = 1000;
    private const int _maxScore = 1000000;
    private LevelXmlSaver _levelSettings;

    private string[] _difficulties = { "easy", "normal", "hard" };
    private int _currWave = 0;

    // Start is called before the first frame update
    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
            WEAP_DICT[def.type] = def;

        if (PlayerPrefs.HasKey("HighScore"))
        {
            _highScore = PlayerPrefs.GetInt("HighScore");
        }
        PlayerPrefs.SetInt("HighScore", _highScore);

        int _difficulty = PlayerPrefs.GetInt("Difficulty");
        _levelSettings = new LevelXmlSaver();
        LoadEnemyPresset(_difficulties[_difficulty]);
        Invoke("SpawnWave", 1f / level.waves[_currWave].delayBeforeWave);
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
        StartCoroutine(SpawnEnemy(level.waves[_currWave]));
    }

    IEnumerator SpawnEnemy(Wave wave)
    {
        for (; ; )
        {
            if (!wave.IsEnd())
            {
                var go = Instantiate<GameObject>(wave.GetShipPrefub());
                float enemyPadding = enemyDefaultPadding;
                if (go.GetComponent<BoundsCheck>() != null)
                {
                    enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
                }

                var pos = Vector3.zero;
                float xMin = -bndCheck.camWidth + enemyPadding;
                float xMax = bndCheck.camWidth - enemyPadding;
                pos.x = Random.Range(xMin, xMax);
                pos.y = bndCheck.camHeight + enemyPadding;
                go.transform.position = pos;
                yield return new WaitForSeconds(1f / enemySpawnPerSecond);
            }
            else
            {
                _currWave++;
                if (_currWave == level.waves.Length)
                {
                    _currWave = 0;
                    print("CONGRATULATION");
                    break;
                }
                else
                {
                    Invoke("SpawnWave", 1f / level.waves[_currWave].delayBeforeWave);
                    break;
                }
            }
        }
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
        _score += e.score;
        UpdateGUI();
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
            return WEAP_DICT[wt];

        return new WeaponDefinition();
    }

    void UpdateGUI()
    {
        if (_score > _maxScore) _score = _maxScore;

        if (_score > _highScore)
        {
            _highScore = _score;
            PlayerPrefs.SetInt("HighScore", _highScore);
        }
        uiTextScore.text = $"Score: {_score}";
        uiHighScore.text = $"HighScore: {_highScore}";
    }

    void LoadEnemyPresset(string difficulty)
    {
        level = LevelXmlSaver.Load(Path.Combine(Application.dataPath, "Resources/difficalty.xml")).Levels.Where(l => l.name == difficulty).Single();
        foreach (var wave in level.waves)
        {
            wave.InitShipsPrefub(wave.ships.Length);
            for (int i = 0; i < wave.ships.Length; i++)
            {
                wave.SetShipPrefub(prefubEnemies.Where(e => e.name == wave.ships[i]).Single());
            }
        }
    }
}
