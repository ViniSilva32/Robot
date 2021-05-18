using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player;        // movimentação do player
    public Transform bulletSpawn;   // local de onde a bala sai
    public Slider healthBar;        // seta a barra de vida do bot
    public GameObject bulletPrefab; // seta o prefab dos disparos do player

    NavMeshAgent agent;
    public Vector3 destination; // aponta a direção ao qual o bot deve ir
    public Vector3 target;      // mira 
    float health = 100.0f;      // vida maxima do bot
    float rotSpeed = 5.0f;      //velocidade de rotação do bot

    float visibleRange = 80.0f; //  distancia máxima ao qual é visivel
    float shotRange = 40.0f;    // distancia maxima do disparo    

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5;         //velocidade dos disparos
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;                                                      // faz com que a barra de vida esteja sempre seguindo o BOT
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
    }

    void UpdateHealth()
    {
        if (health < 100)             // O HP volta depois de um tempo fora de combate
            health++;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "bullet")
        {
            health -= 10;           //Quantidade de HP removida pelos tiros do player
        }
    }
    [Task]
    public void PickRandomDestination()     //Escolhe uma direção aleatória para ir
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }
    [Task]
    public void MoveToDestination() // move o bot para a direção escolhida
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
    [Task]
    public void PickDestination(int x, int z) // vai para uma direção pre determinada
    {
        Vector3 dest = new Vector3(x, 0, z);
        agent.SetDestination(dest);
        Task.current.Succeed();

    }
    // --- 17/05/2021 ---
    [Task]
    public void TargetPlayer()  //mira no player
    {
        target = player.transform.position; Task.current.Succeed(); 
    }
    [Task]
    public bool Fire()  // atira no player
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation); 
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        return true;
    }
    [Task]
    public void LookAtTarget()  // olha para o alvo (player), fazendo rotacionar o canhão em sua direção  
    {
        Vector3 direction = target - this.transform.position;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction)); // mostra o algulo em que o canhão está sendo apontado
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();         // se o angulo for menor que o numero X execute a ação
        }
    }
    [Task]
    bool SeePlayer()    // verifica se o player está na mira, ou se é uma parede
    {
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit;
        bool SeePlayer = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);    // cria um traço vermeho entre o player e o Droid
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            if (hit.collider.gameObject.tag == "wall")  // verifica  a tag do objeto
            {
                SeePlayer = true;   
            }
        }
        if (Task.isInspected)   // mostra no console 
            Task.current.debugInfo = string.Format("wall={0}", SeePlayer);  

        if (distance.magnitude < visibleRange && !SeePlayer) 
            return true;
        else
            return false;
    }
    [Task]
    bool Turn (float angle) // leva o droid até uma posição sorteada
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true;
    }

}

