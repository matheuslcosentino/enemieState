using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Importa a biblioteca para trabalhar com UI

public class EnemyController : MonoBehaviour
{
    public Transform[] waypoints; // Waypoints configuráveis no Inspector
    public GameObject enemyPrefab; // Prefab do inimigo a ser spawnado
    public float speed = 3.0f; // Velocidade de movimento do inimigo
    private int currentWaypointIndex = 0; // Índice do waypoint atual

    public float actionRadius = 10.0f; // Raio da área de ação para seguir o player
    public float checkRadius = 5.0f; // Raio da área de checagem para parar o inimigo
    private Transform player; // Referência ao jogador
    private bool isChasingPlayer = false; // Controle se o inimigo está perseguindo o player
    private bool isInCheckArea = false; // Controle se o player está na área de checagem

    public Text statusText; // Referência ao texto da UI para mostrar o estado do inimigo

    // Atributo de vida
    public int life = 10;

    // Referência ao texto da UI para a vida do inimigo
    public Text lifeText;

    private float attackCooldown = 3.0f; // Tempo de espera entre ataques
    private float lastAttackTime = 0.0f; // Marca o tempo do último ataque

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Identifica o jogador pela tag
        UpdateStatusText("Patrulhando"); // Inicializa com o estado "Patrulhando"
        UpdateLifeText(); // Atualiza o texto da vida no início
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Verifica se o player está dentro da área de checagem
        if (distanceToPlayer <= checkRadius)
        {
            isInCheckArea = true; // O jogador está na área de checagem
            UpdateStatusText("Atacando"); // Atualiza o estado para "Atacando"
            AttackPlayer(); // Inicia o ataque ao jogador
        }
        else
        {
            isInCheckArea = false; // O jogador saiu da área de checagem
        }

        // Se o jogador está na área de checagem, o inimigo para de se mover
        if (isInCheckArea)
        {
            StopMovement();
        }
        else
        {
            // Verifica se o player está dentro da área de ação
            if (distanceToPlayer <= actionRadius)
            {
                isChasingPlayer = true; // Persegue o jogador
                UpdateStatusText("Perseguindo"); // Atualiza o estado para "Perseguindo"
                FollowPlayer();
            }
            else
            {
                isChasingPlayer = false; // Volta para o caminho dos waypoints
                UpdateStatusText("Patrulhando"); // Atualiza o estado para "Patrulhando"
                FollowWaypoints();
            }
        }
    }

    // Função para seguir o jogador
    void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    // Função para seguir os waypoints
    void FollowWaypoints()
    {
        if (waypoints.Length == 0)
            return; // Verifica se há waypoints configurados

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Verifica se o inimigo alcançou o waypoint atual
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Muda para o próximo waypoint
        }
    }

    // Função para parar o movimento do inimigo
    void StopMovement()
    {
        // Apenas interrompe o movimento ao não alterar a posição do inimigo
        // O inimigo ficará parado
    }

    // Função para atualizar o texto do estado na UI
    void UpdateStatusText(string newState)
    {
        if (statusText != null)
        {
            statusText.text = "Estado: " + newState; // Atualiza o texto da UI com o estado atual
        }
    }

    // Função para atacar o jogador
    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown) // Verifica o tempo de espera entre ataques
        {
            lastAttackTime = Time.time; // Atualiza o tempo do último ataque
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.TakeDamage(1); // Dano de 1 ao jogador
        }
    }

    // Método para receber dano
    public void TakeDamage(int damage)
    {
        life -= damage; // Reduz a vida do inimigo
        UpdateLifeText(); // Atualiza o texto da vida

        // Verifica se a vida do inimigo chegou a 0
        if (life <= 0)
        {
            SpawnNewEnemy(); // Chama a função para spawnar um novo inimigo
            Destroy(gameObject); // Destroi o objeto do inimigo
            Debug.Log("Inimigo foi destruído!"); // Mensagem no console
        }
    }

    // Função para spawnar um novo inimigo
    private void SpawnNewEnemy()
    {
        if (enemyPrefab != null && waypoints.Length > 0)
        {
            Instantiate(enemyPrefab, waypoints[currentWaypointIndex].position, Quaternion.identity); // Spawn do novo inimigo no waypoint
        }
    }

    // Método para atualizar o texto da vida na UI
    private void UpdateLifeText()
    {
        if (lifeText != null)
        {
            lifeText.text = "Vida do Inimigo: " + life; // Atualiza o texto da UI com a vida atual
        }
    }

    // Desenha a área de ação e a área de checagem no modo de edição
    private void OnDrawGizmos()
    {
        // Desenha a área de ação
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, actionRadius);

        // Desenha a área de checagem
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        // Desenha os waypoints
        if (waypoints != null && waypoints.Length > 1)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);

                int nextIndex = (i + 1) % waypoints.Length;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
            }
        }
    }
}
