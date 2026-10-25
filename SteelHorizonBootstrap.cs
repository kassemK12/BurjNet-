using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SteelHorizonBootstrap : MonoBehaviour
{
    public static int TeamSize = 1;
    public static SteelHorizonBootstrap Instance;
    public readonly List<CombatUnit> Units = new List<CombatUnit>();
    public CombatUnit Selected;
    public Text statusText;
    public AudioClip tankSound, rifleSound, rocketSound, explosionSound, jetSound, heliSound;
    private Material groundMat, roadMat, blueMat, redMat, buildingMat, greenMat;
    private Camera cam;
    private float nextSpawn;

    void Awake() { Instance = this; }
    void Start()
    {
        Application.targetFrameRate = 60;
        MakeMaterials(); CreateWorld(); CreateCamera(); CreateAudio(); SpawnBattle(TeamSize); CreateUI();
    }
    void MakeMaterials()
    {
        groundMat = Mat(new Color(.20f,.27f,.19f)); roadMat = Mat(new Color(.16f,.17f,.17f));
        blueMat = Mat(new Color(.12f,.52f,.82f)); redMat = Mat(new Color(.78f,.16f,.12f));
        buildingMat = Mat(new Color(.48f,.43f,.36f)); greenMat = Mat(new Color(.16f,.32f,.17f));
    }
    Material Mat(Color c) { var m = new Material(Shader.Find("Standard")); m.color = c; return m; }
    GameObject Cube(string name, Vector3 pos, Vector3 scale, Material mat)
    { var g=GameObject.CreatePrimitive(PrimitiveType.Cube); g.name=name; g.transform.position=pos; g.transform.localScale=scale; g.GetComponent<Renderer>().material=mat; return g; }
    void CreateWorld()
    {
        var ground=GameObject.CreatePrimitive(PrimitiveType.Plane); ground.name="Terrain"; ground.transform.localScale=new Vector3(24,1,24); ground.GetComponent<Renderer>().material=groundMat;
        Cube("Main road",new Vector3(0,.015f,0),new Vector3(7,.04f,220),roadMat);
        Cube("Cross road",new Vector3(0,.02f,0),new Vector3(220,.04f,6),roadMat);
        Random.InitState(47);
        for(int i=0;i<70;i++) { float x=Random.Range(-95,95), z=Random.Range(-95,95); if(Mathf.Abs(x)<12||Mathf.Abs(z)<12) continue; float h=Random.Range(3,10); Cube("Apartment block",new Vector3(x,h/2,z),new Vector3(Random.Range(4,9),h,Random.Range(4,8)),buildingMat); }
        for(int i=0;i<95;i++) { float x=Random.Range(-110,110), z=Random.Range(-110,110); if(Mathf.Abs(x)<10||Mathf.Abs(z)<10) continue; var t=GameObject.CreatePrimitive(PrimitiveType.Cylinder); t.name="Tree"; t.transform.position=new Vector3(x,1.4f,z); t.transform.localScale=new Vector3(.7f,1.4f,.7f); t.GetComponent<Renderer>().material=greenMat; }
        Cube("Blue objective",new Vector3(-42,.15f,0),new Vector3(18,.25f,18),blueMat); Cube("Red objective",new Vector3(42,.15f,0),new Vector3(18,.25f,18),redMat);
    }
    void CreateCamera()
    {
        var go=new GameObject("RTS Camera"); cam=go.AddComponent<Camera>(); cam.fieldOfView=55; cam.transform.position=new Vector3(0,95,-100); cam.transform.rotation=Quaternion.Euler(42,0,0); cam.tag="MainCamera";
        go.AddComponent<AudioListener>();
    }
    void CreateAudio()
    {
        tankSound=Load("sfx_tank"); rifleSound=Load("sfx_rifle"); rocketSound=Load("sfx_rocket"); explosionSound=Load("sfx_explosion"); jetSound=Load("sfx_jet"); heliSound=Load("sfx_heli");
    }
    AudioClip Load(string n) { return Resources.Load<AudioClip>("Audio/"+n); }
    void SpawnBattle(int teamSize)
    {
        foreach(var u in Units) if(u) Destroy(u.gameObject); Units.Clear(); Selected=null;
        int perTeam=teamSize*3;
        for(int i=0;i<perTeam;i++) { SpawnUnit(0,new Vector3(-55+(i%3)*8,1,-25+(i/3)*9), i%3==0?UnitKind.Tank:(i%3==1?UnitKind.Infantry:UnitKind.Artillery)); }
        for(int i=0;i<perTeam;i++) { SpawnUnit(1,new Vector3(55-(i%3)*8,1,25-(i/3)*9), i%3==0?UnitKind.Tank:(i%3==1?UnitKind.Infantry:UnitKind.Rocket)); }
    }
    void SpawnUnit(int team, Vector3 pos, UnitKind kind)
    {
        var root=new GameObject((team==0?"Blue ":"Red ")+kind); root.transform.position=pos;
        Material m=team==0?blueMat:redMat;
        GameObject body=GameObject.CreatePrimitive(kind==UnitKind.Infantry?PrimitiveType.Capsule:PrimitiveType.Cube); body.transform.SetParent(root.transform,false); body.transform.localPosition=Vector3.zero;
        body.transform.localScale=kind==UnitKind.Tank?new Vector3(2.8f,.9f,4.1f):kind==UnitKind.Artillery?new Vector3(2.5f,1.1f,4.5f):kind==UnitKind.Rocket?new Vector3(2.4f,1.4f,3.5f):new Vector3(.7f,1.8f,.7f);
        body.GetComponent<Renderer>().material=m; // Keep the body collider so touch selection and raycasts work.
        if(kind==UnitKind.Tank||kind==UnitKind.Artillery||kind==UnitKind.Rocket) { var turret=GameObject.CreatePrimitive(PrimitiveType.Cylinder); turret.transform.SetParent(root.transform,false); turret.transform.localPosition=new Vector3(0,.65f,0); turret.transform.localScale=new Vector3(.65f,.22f,.65f); turret.GetComponent<Renderer>().material=m; Destroy(turret.GetComponent<Collider>()); var barrel=Cube("Barrel",Vector3.zero,new Vector3(.22f,.22f,1.7f),Mat(Color.gray)); barrel.transform.SetParent(root.transform,false); barrel.transform.localPosition=new Vector3(0,.72f,1.1f); }
        var unit=root.AddComponent<CombatUnit>(); unit.Init(team,kind); Units.Add(unit);
    }
    void CreateUI()
    {
        var canvasGO=new GameObject("HUD Canvas"); var canvas=canvasGO.AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay; canvasGO.AddComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; canvasGO.AddComponent<GraphicRaycaster>();
        var es=FindObjectOfType<EventSystem>(); if(!es){var e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();}
        Panel(canvas.transform,"Top HUD",new Vector2(0,1),new Vector2(1,1),new Vector2(0,-38),new Vector2(0,76),new Color(.03f,.06f,.08f,.86f));
        var title=Label(canvas.transform,"STEEL HORIZON  •  TACTICAL SKIRMISH",22,new Vector2(.02f,.96f),new Vector2(.65f,.99f),TextAnchor.MiddleLeft); title.color=Color.white;
        statusText=Label(canvas.transform,"Tap a friendly unit, then tap terrain to move. Units engage enemies automatically.",16,new Vector2(.02f,.89f),new Vector2(.96f,.94f),TextAnchor.MiddleLeft); statusText.color=Color.white;
        for(int i=1;i<=4;i++) { int size=i; var b=Button(canvas.transform,i+"v"+i,new Vector2(.66f+(i-1)*.082f,.96f),new Vector2(.735f+(i-1)*.082f,.995f)); b.onClick.AddListener(()=>{TeamSize=size;SpawnBattle(size);if(statusText)statusText.text="Mode "+size+"v"+size+" — local AI skirmish";}); }
        var help=Label(canvas.transform,"SELECT UNIT  •  TAP GROUND TO MOVE  •  AI COMBAT",15,new Vector2(.02f,.02f),new Vector2(.98f,.07f),TextAnchor.MiddleCenter); help.color=Color.white;
    }
    void Panel(Transform parent,string name,Vector2 amin,Vector2 amax,Vector2 pos,Vector2 size,Color color) { var g=new GameObject(name);g.transform.SetParent(parent,false);var r=g.AddComponent<RectTransform>();r.anchorMin=amin;r.anchorMax=amax;r.anchoredPosition=pos;r.sizeDelta=size;var im=g.AddComponent<Image>();im.color=color; }
    Text Label(Transform p,string s,int size,Vector2 amin,Vector2 amax,TextAnchor align) { var g=new GameObject("Text");g.transform.SetParent(p,false);var r=g.AddComponent<RectTransform>();r.anchorMin=amin;r.anchorMax=amax;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;var t=g.AddComponent<Text>();t.text=s;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=size;t.alignment=align;t.color=Color.white;t.horizontalOverflow=HorizontalWrapMode.Wrap;return t; }
    Button Button(Transform p,string s,Vector2 amin,Vector2 amax) { var g=new GameObject("Mode "+s);g.transform.SetParent(p,false);var r=g.AddComponent<RectTransform>();r.anchorMin=amin;r.anchorMax=amax;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;var im=g.AddComponent<Image>();im.color=new Color(.12f,.25f,.34f,.95f);var b=g.AddComponent<Button>();b.targetGraphic=im;var t=Label(g.transform,s,18,new Vector2(0,0),new Vector2(1,1),TextAnchor.MiddleCenter);t.fontStyle=FontStyle.Bold;return b; }
    void Update()
    {
        if(cam==null)return;
        float scroll=Input.mouseScrollDelta.y; if(Mathf.Abs(scroll)>.01f) cam.transform.position+=cam.transform.forward*scroll*4f;
        if(Input.touchCount>0 && Input.GetTouch(0).phase==TouchPhase.Began) HandleTap(Input.GetTouch(0).position);
        else if(Input.GetMouseButtonDown(0)) HandleTap(Input.mousePosition);
        if(Input.GetMouseButton(1)) MoveSelected(Input.mousePosition);
        if(Input.touchCount==1 && Input.GetTouch(0).phase==TouchPhase.Ended && Selected!=null) MoveSelected(Input.GetTouch(0).position);
    }
    void HandleTap(Vector2 screen)
    {
        Ray ray=cam.ScreenPointToRay(screen); if(Physics.Raycast(ray,out RaycastHit hit,500)) { var u=hit.collider.GetComponentInParent<CombatUnit>(); if(u!=null&&u.Team==0){Selected=u;if(statusText)statusText.text="Selected: "+u.Kind+" | HP "+Mathf.CeilToInt(u.Health)+". Tap terrain to order movement.";} else if(Selected!=null) MoveSelected(screen); }
    }
    void MoveSelected(Vector2 screen) { Ray r=cam.ScreenPointToRay(screen); if(Physics.Raycast(r,out RaycastHit h,500)&&Selected!=null&&h.collider.GetComponentInParent<CombatUnit>()==null) Selected.SetDestination(h.point); }
}
