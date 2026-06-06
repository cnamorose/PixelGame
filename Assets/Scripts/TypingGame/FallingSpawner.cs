using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FallingSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public Transform lettersParent;
    public TypingGameManager manager;
    public RectTransform spawnArea;

    private Queue<float> recentXs = new Queue<float>();

    public float spawnInterval = 2f;
    private float timer;

    // 최근에 생성된 단어들을 기억할 큐 (중복 방지용 족보)
    private Queue<string> recentWords = new Queue<string>();

    // 몇 개 전까지 나왔던 단어를 금지할 것인가? (500개 중 15개 정도면 적당히 가끔 중복이 나옵니다)
    private const int MaxRecentCount = 15;

    // private readonly를 사용하여 인스펙터를 깔끔하게 숨김
    private readonly string[] wordList =
    {
        // === [1] 컴퓨터 과학 (Computer Science) ===
        "algorithm", "array", "binary", "buffer", "cache", "compiler", "database", "debugging", "encryption", "framework",
        "hardware", "interface", "kernel", "network", "object", "parameter", "protocol", "recursion", "server", "software",
        "variable", "syntax", "runtime", "pointer", "memory", "function", "argument", "boolean", "string", "integer",
        "matrix", "vector", "cluster", "node", "packet", "router", "gateway", "firewall", "domain", "hosting",
        "storage", "backup", "recovery", "security", "exploit", "malware", "phishing", "thread", "process", "deadlock",
        "console", "terminal", "script", "library", "module", "package", "repository", "commit", "branch", "merge",
        "deployment", "pipeline", "automation", "abstract", "inheritance", "instance", "class", "method", "constructor",
        "virtual", "container", "sandbox", "firmware", "driver", "peripheral", "bandwidth", "latency", "throughput",
        "request", "response", "session", "cookie", "token", "payload", "endpoint", "routing", "schema", "query",
        "index", "transaction", "trigger", "concurrency", "parallel", "distributed", "cloning", "parsing", "compiling",

        // === [2] 과학 (Science) ===
        "atom", "molecule", "electron", "proton", "neutron", "nucleus", "isotope", "element", "compound", "mixture",
        "catalyst", "reaction", "acid", "base", "solution", "solvent", "evolution", "mutation", "organism", "species",
        "genus", "cell", "tissue", "organ", "enzyme", "protein", "genome", "chromosome", "gravity", "velocity",
        "friction", "inertia", "momentum", "energy", "entropy", "spectrum", "wavelength", "frequency", "resonance", "vacuum",
        "galaxy", "nebula", "planet", "asteroid", "comet", "orbit", "eclipse", "telescope", "microscope", "laboratory",
        "hypothesis", "control", "analysis", "synthesis", "theory", "phenomenon", "mechanism", "organelle", "membrane",
        "photosynthesis", "respiration", "metabolism", "heredity", "phenotype", "genotype", "ecosystem", "biosphere", "habitat", "symbiosis",
        "parasite", "bacteria", "virus", "fungus", "algae", "radiation", "conduction", "convection", "thermodynamics", "kinetics",
        "equilibrium", "solubility", "precipitation", "distillation", "filtration", "diffusion", "osmosis", "viscosity", "density", "volume",
        "mass", "acceleration", "force", "pressure", "temperature", "voltage", "current", "resistance", "magnetism", "optics",

        // === [3] 인문학 (Humanities) ===
        "philosophy", "ethics", "morality", "logic", "dialectic", "rhetoric", "aesthetic", "metaphysics", "epistemology", "existentialism",
        "dualism", "idealism", "realism", "pragmatism", "nihilism", "stoicism", "humanism", "skepticism", "rationalism", "empiricism",
        "literature", "poetry", "prose", "drama", "narrative", "metaphor", "allegory", "paradox", "irony", "satire",
        "chronology", "history", "archive", "artifact", "culture", "heritage", "tradition", "folklore", "mythology", "ritual",
        "linguistics", "phonetics", "semantics", "pragmatics", "etymology", "dialect", "discourse", "hermeneutics", "critique",
        "theology", "deity", "sacred", "secular", "doctrine", "dogma", "orthodoxy", "paradigm", "enlightenment", "renaissance",
        "modernism", "classicism", "romanticism", "structuralism", "text", "context", "author", "audience", "genre",
        "biography", "memoir", "chronicle", "sculpture", "architecture", "perspective", "symmetry", "harmony", "melody", "rhythm",
        "identity", "subjectivity", "agency", "consciousness", "perception", "intuition", "cognition", "wisdom", "virtue", "justice",
        "altruism", "empathy", "sympathy", "catharsis", "tragedy", "comedy", "epic", "lyric", "monologue", "dialogue",

        // === [4] 사회과학 (Social Sciences) ===
        "society", "community", "institution", "structure", "stratification", "mobility", "demography", "population", "urbanization", "migration",
        "globalization", "bureaucracy", "hierarchy", "authority", "legitimacy", "sovereignty", "democracy", "republic", "monarchy", "oligarchy",
        "autocracy", "ideology", "liberalism", "conservatism", "socialism", "capitalism", "nationalism", "citizenship", "suffrage", "elections",
        "parliament", "congress", "judiciary", "executive", "legislative", "constitution", "statute", "regulation", "policy", "governance",
        "economy", "market", "finance", "currency", "inflation", "deflation", "recession", "commodity", "capital", "labor",
        "employment", "unemployment", "poverty", "welfare", "subsidy", "tariff", "trade", "commerce", "consumer", "producer",
        "psychology", "behavior", "emotion", "personality", "motivation", "attitude", "stereotype", "prejudice",
        "discrimination", "segregation", "integration", "assimilation", "acculturation", "socialization", "norm", "deviance", "sanction", "crime",
        "rehabilitation", "deterrence", "conflict", "cooperation", "negotiation", "diplomacy", "treaty", "alliance",
        "survey", "statistics", "census", "correlation", "causality", "empirical", "qualitative", "quantitative",

        // === [5] 지리학 (Geography) ===
        "latitude", "longitude", "equator", "hemisphere", "continent", "peninsula", "archipelago", "island", "ocean", "sea",
        "gulf", "bay", "strait", "channel", "river", "tributary", "estuary", "delta", "lake", "lagoon",
        "mountain", "valley", "canyon", "plateau", "plain", "desert", "oasis", "tundra", "savanna", "forest",
        "jungle", "swamp", "marsh", "glacier", "iceberg", "volcano", "crater", "earthquake", "fault", "seismic",
        "atmosphere", "climate", "weather", "monsoon", "hurricane", "typhoon", "tornado", "precipitation", "humidity",
        "erosion", "weathering", "sediment", "soil", "topography", "elevation", "altitude", "relief", "contour", "map",
        "atlas", "globe", "compass", "horizon", "terrain", "landscape", "biome", "environment", "conservation",
        "resource", "mineral", "agriculture", "irrigation", "cultivation", "pasture", "livestock", "settlement", "village", "town",
        "city", "metropolis", "megalopolis", "suburb", "boundary", "frontier", "border", "territory", "region", "province",
        "state", "nation", "capital", "district", "zone", "corridor", "transit", "navigation", "meridian", "toponym"
    };

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnWord();
            timer = 0f;
        }
    }

    void SpawnWord()
    {
        GameObject obj = Instantiate(wordPrefab, lettersParent);

        RectTransform wordRT = obj.GetComponent<RectTransform>();
        RectTransform areaRT = lettersParent.GetComponent<RectTransform>();

        float areaWidth = areaRT.rect.width;

        // 1. 최근에 안 나온 단어 뽑기 (안전장치)
        string w = "";
        int safetyNet = 0;

        while (safetyNet < 100) // 무한루프 방지용 카운트
        {
            w = wordList[Random.Range(0, wordList.Length)];

            // 최근 15개 안에 포함되지 않은 단어라면 합격!
            if (!recentWords.Contains(w))
            {
                break;
            }
            safetyNet++;
        }

        // 2. 최근 단어 족보 리스트에 추가하고 관리
        recentWords.Enqueue(w);
        if (recentWords.Count > MaxRecentCount)
        {
            recentWords.Dequeue(); // 가장 오래된 단어 제거
        }

        // 3. 단어 길이에 따른 양옆 짤림 방지 X 좌표 계산
        float wordEstimatedWidth = w.Length * 22f;
        float halfWordWidth = wordEstimatedWidth * 0.5f;

        float minX = -areaWidth * 0.5f + halfWordWidth;
        float maxX = areaWidth * 0.5f - halfWordWidth;

        if (minX > maxX) minX = maxX = 0f;


        // 4. ⭐ [추가] 위치 랜덤 분산 시스템 (비슷한 위치 스폰 방지) ⭐
        float x = 0f;
        int positionSafetyNet = 0;
        float minDistance = areaWidth * 0.2f; // 최소 이 정도 거리(화면 너비의 20%)는 떨어져서 나오도록 설정

        while (positionSafetyNet < 50)
        {
            x = Random.Range(minX, maxX); // 우선 랜덤 좌표를 뽑음
            bool isTooClose = false;

            // 최근에 생성된 X 좌표들과 비교해서 너무 가까운지 체크
            foreach (float rx in recentXs)
            {
                if (Mathf.Abs(x - rx) < minDistance)
                {
                    isTooClose = true;
                    break;
                }
            }

            // 다른 단어들과 충분히 떨어져 있다면 루프 탈출!
            if (!isTooClose)
            {
                break;
            }
            positionSafetyNet++;
        }

        // 결정된 X 좌표를 기록하고 기억장치(큐) 관리 (최근 3개 위치만 기억)
        recentXs.Enqueue(x);
        if (recentXs.Count > 3)
        {
            recentXs.Dequeue();
        }


        // 5. 최종 위치 및 단어 설정
        float y = 0f;

        wordRT.anchoredPosition = new Vector2(x, y);

        WordMovement wm = obj.GetComponent<WordMovement>();
        wm.SetWord(w);

        manager.RegisterWord(wm);
    }
}