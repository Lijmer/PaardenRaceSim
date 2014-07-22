using UnityEngine;
using System.Collections;

public class Horse : MonoBehaviour
{
  public static RaceTrackPath s_path;
  public static GUIValueInput s_guiValueInput;

  public static bool s_started = false;


  public string horseName { get { return m_name; } set { m_name = value; } }
  public Color color { get { return m_color; } set { m_color = value; } }
  public int number { get { return m_number; } set { m_number = value; } }
  public int steroids { get { return m_steroids; } set { m_steroids = value; } }
  public float score { get { return m_score; } set { m_score = value; } }
  public int lap { get { return m_lap; } set { m_lap = value; } }
  public int indexPoint { get { return m_indexPoint; } set { m_indexPoint = value; } }
  public bool shouldDie { get { return m_shouldDie; } set { m_shouldDie = value; } }
  public float deadTimer { get { return m_deadTimer; } set { m_deadTimer = value; } }
  public bool dead { get { return m_dead; } set { m_dead = value; } }

  string m_name;
  Color m_color;
  int m_number;
  int m_steroids;
  float m_score;

  public Vector3 m_targetPos;
  Vector3 m_lastPos;
  int m_indexPoint = 0;
  int m_lap = 0;
  public float m_targetVelocity;
  float m_lastVelChangeTime;

  float m_slowDownCountDown;
  bool m_slowDownActive;
  public float m_slowdown = 0.0f;

  bool m_shouldDie;
  float m_deadTimer = 0.0f;
  bool m_dead;

  Animator m_animator;

  void Start()
  {
    if(s_path == null)
      s_path = GameObject.Find("Path").GetComponent<RaceTrackPath>();
    if(s_guiValueInput == null)
      s_guiValueInput = GameObject.FindGameObjectWithTag("GUIValueInput").GetComponent<GUIValueInput>();

    m_lastPos = transform.position;
    m_targetPos = s_path.m_points[++m_indexPoint];
    //m_targetVelocity += Random.Range(-.01f, .01f);
    m_animator = GetComponentInChildren<Animator>();
    m_lastVelChangeTime = Time.time;
    m_animator.speed = 0.0f;
  }

  public void OrderSlowDown(float time)
  {
    m_slowDownActive = true;
    if(m_slowDownCountDown <= time)
      m_slowDownCountDown = time;
  }
  public void SetSlowDown(float slowDown)
  {
    m_slowdown = slowDown;
  }

  void FixedUpdate()
  {
    if(!s_started)
    {
      m_animator.speed = 0;
      return;
    }

    if(m_deadTimer >= 0.0f)
      m_deadTimer -= Time.fixedDeltaTime;
    else if(m_shouldDie)
      m_dead = true;

    if(m_dead)
    {
      Quaternion targetRot = Quaternion.Euler(new Vector3(
        transform.rotation.eulerAngles.x,
        transform.rotation.eulerAngles.y,
        90.0f
      ));
      transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime);
      m_animator.speed = 0;
      return;
    }

    m_slowDownCountDown -= Time.fixedDeltaTime;
    if(m_slowDownCountDown < 0.0f)
      m_slowDownActive = false;

    //calculate new velocity
    Vector3 vel = (rigidbody.transform.position - m_lastPos);
    float slowDown = m_slowDownActive ? .1f : .0f;
    float velMagnitude = Mathf.Lerp(vel.magnitude, m_targetVelocity - m_slowdown, Time.fixedDeltaTime);
    //vel = vel.normalized * velMagnitude;


    //adjust target velocity every 10 seconds
    if(Time.time - m_lastVelChangeTime > 3.0f)
    {
      m_targetVelocity += Random.Range(-.1f, .1f);
      m_targetVelocity = Mathf.Clamp(m_targetVelocity, .4f, .6f + .1f * steroids);
      //m_targetVelocity += Random.Range(-.05f, .05f);
      m_lastVelChangeTime = Time.time;
    }

    //vel += vel.normalized * m_acceleration * Time.fixedDeltaTime;

    //calculate direction to the next point
    if((m_targetPos - transform.position).sqrMagnitude < 5f)
    {
      if(++m_indexPoint >= s_path.m_points.Length)
      {
        m_indexPoint = 0;
        ++m_lap;
      }
      m_targetPos = s_path.m_points[m_indexPoint];
    }

    //Adjust direction if something is blocking it's path.
    Vector3 dir = (m_targetPos - transform.position).normalized;
    RaycastHit rayHit;
    if(Physics.Raycast(transform.position, transform.forward * 2.0f, out rayHit))
    {
      if(rayHit.collider.gameObject && rayHit.collider.tag == "Horse")
      {
        //rayHit.collider.gameObject.GetComponent<Horse>().OrderSlowDown(1.0f);
        dir += Quaternion.AngleAxis(-30f, Vector3.up) * transform.forward;
      }

    }
    //Debug.DrawRay(transform.position, transform.forward * 2f);
    //Debug.DrawRay(transform.position, dir * 2f, Color.blue);

    //Set the forward vector.
    if(dir != Vector3.zero)
      transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.fixedDeltaTime * 10f);

    //Actually move the horse
    m_lastPos = transform.position;
    m_animator.speed = velMagnitude * 2.5f;
    rigidbody.MovePosition(transform.position + transform.forward * velMagnitude);

    if(m_lap >= 3)
      s_guiValueInput.StopRace();

  }

  void OnGUI()
  {
    if(s_guiValueInput.GetComponent<GUIValueInput>().state == GUIValueInput.GameState.INPUT)
      return;

    Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.0f, 2.0f, 0.0f));
    GUI.contentColor = this.color;
    GUI.skin.label.fontSize = 20;

    GUI.Label(new Rect(pos.x, Screen.height - pos.y, 200, 100), m_number + ". " + m_name);

    GUI.contentColor = Color.white;
    GUI.skin.label.fontSize = 12;
  }
}
