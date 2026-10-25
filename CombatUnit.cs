using UnityEngine;

public enum UnitKind { Tank, Infantry, Artillery, Rocket, Helicopter, Jet }
public class CombatUnit : MonoBehaviour
{
    public int Team {get;private set;} public UnitKind Kind {get;private set;} public float Health {get;private set;}=100;
    public float MoveSpeed=7f, Range=24f, Damage=12f; private Vector3 destination; private bool hasDestination; private float nextShot; private Renderer bodyRenderer; private Color baseColor;
    public void Init(int team, UnitKind kind) { Team=team;Kind=kind;destination=transform.position; MoveSpeed=kind==UnitKind.Infantry?5f:kind==UnitKind.Tank?6f:4.5f;Range=kind==UnitKind.Artillery?38f:kind==UnitKind.Rocket?32f:22f;Damage=kind==UnitKind.Artillery?24f:kind==UnitKind.Rocket?20f:kind==UnitKind.Tank?16f:7f; bodyRenderer=GetComponentInChildren<Renderer>();if(bodyRenderer)baseColor=bodyRenderer.material.color; }
    void Update()
    {
        if(Health<=0)return;
        if(hasDestination) { Vector3 d=destination-transform.position; d.y=0; if(d.magnitude>.8f){transform.position+=d.normalized*MoveSpeed*Time.deltaTime;transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(d),5f*Time.deltaTime);}else hasDestination=false; }
        CombatUnit target=FindTarget(); if(target!=null){Vector3 d=target.transform.position-transform.position;d.y=0;if(d.sqrMagnitude>1)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(d),3f*Time.deltaTime); if(Time.time>=nextShot){nextShot=Time.time+(Kind==UnitKind.Infantry?.7f:1.4f);target.TakeDamage(Damage);if(Random.value<.25f)PlayShot();}}
    }
    CombatUnit FindTarget(){CombatUnit best=null;float bestD=Range;foreach(var u in SteelHorizonBootstrap.Instance.Units){if(!u||u==this||u.Team==Team||u.Health<=0)continue;float d=Vector3.Distance(transform.position,u.transform.position);if(d<bestD){best=u;bestD=d;}}return best;}
    public void SetDestination(Vector3 p){destination=new Vector3(p.x,transform.position.y,p.z);hasDestination=true;}
    public void TakeDamage(float amount){Health-=amount;if(Health<=0){Health=0;var a=GetComponent<AudioSource>();if(a&&SteelHorizonBootstrap.Instance.explosionSound)a.PlayOneShot(SteelHorizonBootstrap.Instance.explosionSound);SteelHorizonBootstrap.Instance.Units.Remove(this);Destroy(gameObject,0.05f);}}
    void PlayShot(){var boot=SteelHorizonBootstrap.Instance;AudioClip clip=Kind==UnitKind.Rocket?boot.rocketSound:Kind==UnitKind.Artillery?boot.explosionSound:Kind==UnitKind.Infantry?boot.rifleSound:boot.tankSound;if(clip)AudioSource.PlayClipAtPoint(clip,transform.position,.18f);}
}
