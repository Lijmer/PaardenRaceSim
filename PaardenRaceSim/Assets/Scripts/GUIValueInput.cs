using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions; //Regex.Replace

public class GUIValueInput : MonoBehaviour
{
  static string[] firstNames = 
  {
    "Guido",
    "Butt Ugly",
    "Tepid Ralphie",
    "Salvatore the",
    "Third Leg",
    "Painfully Shy",
    "Don Jackie",
    "Jumpy Al",
    "Blunt Force",
    "Bullet Riddled",
    "Henk",
    "Green Tony",
    "Tito",
    "Shifty Donato",
    "Stefano",
    "Louie",
  };
  static string[] lastNames = 
  {
    "Gallo",
    "Louie",
    "Costa",
    "Suit",
    "Luigi",
    "Lou",
    "Gravano",
    "Russo",
    "Jackie",
    "Luca",
    "Taart",
    "Mancini",
    "Leatherchest",
    "Milano",
    "Sleepswithfishes",
    "Whatshisnuts",
  };
  static Color[] colors =
  {
    Color.blue,
    Color.cyan,
    Color.red,
    Color.magenta,
    Color.yellow,
    new Color(161, 255, 53, 255) / 255.0f,
    new Color(255, 192, 0, 255) / 255.0f,
    new Color(0, 255, 192, 255) / 255.0f,
    new Color(158, 0, 255, 255) / 255.0f
  };

  private bool m_showGUI;

  //string m_val1 = "";

  public enum GameState
  {
    INPUT,
    PLAYING,
    END
  };
  GameState m_state;
  public GameState state { get { return m_state; } set { m_state = value; } }

  private int m_seed;

  public void StartRace()
  {
    m_state = GameState.PLAYING;
    Horse.s_started = true;

    GameObject[] horses = GameObject.FindGameObjectsWithTag("Horse");
    foreach(GameObject go in horses)
    {
      Horse horse = go.GetComponent<Horse>();
      for(int i = 0; i < horse.steroids; ++i)
        horse.m_targetVelocity += Random.Range(0.0f, .2f);
      int chance = horse.steroids;
      int value = Random.Range(0, 4);
      horse.shouldDie = false;
      horse.deadTimer = 0.0f;
      for(int j = 0; j < chance; ++j)
      {
        if(value == j)
        {
          horse.shouldDie = true;
          horse.deadTimer = Random.Range(5.0f, 60.0f / horse.steroids);
          break;
        }
      }
    }
  }
  public void StopRace()
  {
    m_state = GameState.END;
    Horse.s_started = false;
  }
  public void ResetRace()
  {
    System.DateTime now = System.DateTime.Now;
    Random.seed = now.Second;
    m_state = GameState.INPUT;
    GameObject[] horses = GameObject.FindGameObjectsWithTag("Horse");

    int[] array = new int[horses.Length];
    for(int x = 0; x < array.Length; ++x) array[x] = x + 1;

    System.Random r = new System.Random();
    for(int y = array.Length; y > 0; y--)
    {
      int j = r.Next(y);
      int k = array[j];
      array[j] = array[y - 1];
      array[y - 1] = k;
    }

    int i = 0;
    foreach(GameObject h in horses)
    {
      Horse horse = h.GetComponent<Horse>();
      horse.horseName =
        firstNames[Random.Range(0, firstNames.Length - 1)] + " " +
        lastNames[Random.Range(0, lastNames.Length - 1)];
      horse.m_targetVelocity = Random.Range(.4f, .6f);
      horse.color = colors[i];
      horse.number = array[i++];

    }
    m_seed = (int)(Time.time * 128.0f);
  }

  // Use this for initialization
  void Start()
  {
    ResetRace();
  }

  // Update is called once per frame
  void Update()
  {
    GameObject[] horses = GameObject.FindGameObjectsWithTag("Horse");
    bool allDead = true;
    foreach(GameObject go in horses)
    {
      Horse horse = go.GetComponent<Horse>();
      if(!horse.dead)
      {
        allDead = false;
        break;
      }
    }
    if(allDead)
      StopRace();
  }

  float ViewportToScreenX(float x) { return x * Screen.width; }
  float ViewportToScreenY(float y) { return y * Screen.height; }
  Rect ViewportToScreen(Rect r, float scrWidth, float scrHeight)
  {
    return new Rect(
      r.x * scrWidth,
      r.y * scrHeight,
      r.height * scrWidth,
      r.width * scrHeight
    );
  }

  void OnGUI()
  {
    switch(m_state)
    {
      case GameState.INPUT: OnGUIInput(); break;
      case GameState.PLAYING: OnGUIPlaying(); break;
      case GameState.END: OnGUIEnd(); break;
      default:
        Debug.Log("missing game state in OnGUI in GUIValueInput.cs");
        break;
    }
  }

  void OnGUIInput()
  {
    Rect boxSize = ViewportToScreen(new Rect(.05f, .05f, .9f, .9f), Screen.width, Screen.height);
    GUI.BeginGroup(boxSize);
    GUI.Box(ViewportToScreen(new Rect(0.0f, 0.0f, 1.0f, 1.0f),
      boxSize.width, boxSize.height), new GUIContent());

    GameObject[] horses = GameObject.FindGameObjectsWithTag("Horse");
    Random.seed = m_seed;
    GUI.skin.label.fontSize = 20;
    GUI.color = Color.white;
    GUI.backgroundColor = Color.white;
    int i = 0;
    foreach(GameObject h in horses)
    {
      Horse horse = h.GetComponent<Horse>();
      GUI.contentColor = horse.color;
      GUI.Label(ViewportToScreen(new Rect(0.05f, 0.05f + .1f * i, 1.0f, 1.0f), boxSize.width, boxSize.height),
        (horse.number) + ". " + horse.horseName
        );
      GUI.Label(ViewportToScreen(new Rect(0.35f, 0.05f + .1f * i, 1.0f, 1.0f), boxSize.width, boxSize.height),
        "Rank: " + Mathf.Clamp(Mathf.RoundToInt((horse.m_targetVelocity - .4f) * 50) + Random.Range(-2, 2), 0, 10)
        );
      string value = horse.steroids.ToString();
      value = GUI.TextField(ViewportToScreen(new Rect(.55f, 0.05f + .1f * i, .05f, .1f), boxSize.width, boxSize.height),
        value
        );
      int ivalue;
      if(int.TryParse(value, out ivalue))
        horse.steroids = ivalue;
      ++i;
    }
    GUI.contentColor = Color.white;
    bool playButton = GUI.Button(
      ViewportToScreen(new Rect(.45f, .85f, .1f, .1f), boxSize.width, boxSize.height),
      "play"
      );
    if(playButton)
      StartRace();
    GUI.skin.label.fontSize = 12;
    GUI.EndGroup();
  }

  void OnGUIPlaying()
  {
    Rect boxSize = ViewportToScreen(new Rect(.65f, .05f, .25f, .25f), Screen.width, Screen.height);
    GUI.BeginGroup(boxSize);
    GUI.Box(ViewportToScreen(new Rect(0.0f, 0.0f, 1.0f, 1.0f),
      boxSize.width, boxSize.height), new GUIContent());

    GameObject[] horses = GameObject.FindGameObjectsWithTag("Horse");
    foreach(GameObject go in horses)
    {
      Horse h = go.GetComponent<Horse>();
      h.score = 0.0f;
      h.score += h.lap * 65536.0f;
      h.score += h.indexPoint * 128.0f;
      h.score += 0.0f;
      Vector3 v0;
      Vector3 v1;
      if(h.indexPoint == 0)
      {
        v0 = Horse.s_path.m_points[h.indexPoint - 0];
        v1 = Horse.s_path.m_points[Horse.s_path.m_points.Length - 1];
      }
      else
      {
        v0 = Horse.s_path.m_points[h.indexPoint - 0];
        v1 = Horse.s_path.m_points[h.indexPoint - 1];
      }
      Vector3 line = (v0 - v1);
      float travelled = (h.transform.position - v1).magnitude / line.magnitude;
      h.score += travelled;
    }

    for(int i = 0; i < horses.Length; ++i)
    {
      bool sorted = true;
      for(int j = 0; j < horses.Length - 1; ++j)
      {
        Horse h0 = horses[j + 0].GetComponent<Horse>();
        Horse h1 = horses[j + 1].GetComponent<Horse>();
        if(h0.score < h1.score)
        {
          sorted = false;
          //swap
          GameObject temp = horses[j + 0];
          horses[j + 0] = horses[j + 1];
          horses[j + 1] = temp;
        }
      }
      if(sorted)
        break;
    }
    horses[0].GetComponent<Horse>().SetSlowDown(.2f);
    //horses[0].GetComponent<Horse>().OrderSlowDown(1.0f);
    for(int i = 0; i < horses.Length; ++i)
    {
      Horse horse = horses[i].GetComponent<Horse>();
      //horse.SetSlowDown(.2f - (i * .02f));
      GUI.skin.label.normal.textColor = horse.color;
      GUI.skin.label.fontSize = 18;
      GUI.Label(ViewportToScreen(new Rect(0.05f, 0.05f + .1f * i, 1.0f, 1.0f), boxSize.width, boxSize.height),
        (horse.number) + ". " + horse.horseName
        );
      GUI.skin.label.fontSize = 12;
    }
    GUI.skin.label.normal.textColor = Color.white;

    GUI.EndGroup();
  }
  void OnGUIEnd()
  {

    Rect boxSize = ViewportToScreen(new Rect(.05f, .05f, .9f, .9f), Screen.width, Screen.height);
    GUI.BeginGroup(boxSize);
    GUI.Box(ViewportToScreen(new Rect(0.0f, 0.0f, 1.0f, 1.0f),
      boxSize.width, boxSize.height), new GUIContent());

    GameObject[] horses = GameObject.FindGameObjectsWithTag("Horse");
    foreach(GameObject go in horses)
    {
      Horse h = go.GetComponent<Horse>();
      h.score = 0.0f;
      h.score += h.lap * 65536.0f;
      h.score += h.indexPoint * 128.0f;
      h.score += 0.0f;
      Vector3 v0;
      Vector3 v1;
      if(h.indexPoint == 0)
      {
        v0 = Horse.s_path.m_points[h.indexPoint - 0];
        v1 = Horse.s_path.m_points[Horse.s_path.m_points.Length - 1];
      }
      else
      {
        v0 = Horse.s_path.m_points[h.indexPoint - 0];
        v1 = Horse.s_path.m_points[h.indexPoint - 1];
      }
      Vector3 line = (v0 - v1);
      float travelled = (h.transform.position - v1).magnitude / line.magnitude;
      h.score += travelled;
    }

    for(int i = 0; i < horses.Length; ++i)
    {
      bool sorted = true;
      for(int j = 0; j < horses.Length - 1; ++j)
      {
        Horse h0 = horses[j + 0].GetComponent<Horse>();
        Horse h1 = horses[j + 1].GetComponent<Horse>();
        if(h0.score < h1.score)
        {
          sorted = false;
          //swap
          GameObject temp = horses[j + 0];
          horses[j + 0] = horses[j + 1];
          horses[j + 1] = temp;
        }
      }
      if(sorted)
        break;
    }

    for(int i = 0; i < horses.Length; ++i)
    {
      Horse horse = horses[i].GetComponent<Horse>();
      GUI.skin.label.normal.textColor = horse.color;
      GUI.skin.label.fontSize = 18;
      GUI.Label(ViewportToScreen(new Rect(0.05f, 0.05f + .1f * i, 1.0f, 1.0f), boxSize.width, boxSize.height),
        (horse.number) + ". " + horse.horseName
        );
      if(horse.dead)
      {
        GUI.skin.label.normal.textColor = Color.red;
        GUI.Label(ViewportToScreen(new Rect(0.5f, 0.05f + .1f * i, 1.0f, 1.0f), boxSize.width, boxSize.height),
          "DOOD");
      }
    }
    bool resetButton = GUI.Button(
      ViewportToScreen(new Rect(.45f, .85f, .1f, .1f), boxSize.width, boxSize.height),
      "Reset"
      );
    if(resetButton)
      Application.LoadLevel("scene");
    GUI.skin.label.fontSize = 12;
    GUI.EndGroup();
  }
}
