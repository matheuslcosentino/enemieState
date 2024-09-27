using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Importa a biblioteca para trabalhar com UI

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float gravityValue = -9.81f;

    public float speed = 5.0f;

    // Atributo de vida
    public int life = 10;

    // Referência ao texto da UI
    public Text lifeText;

    public float attackRadius = 5f; // Raio de ataque

    void Start()
    {
        controller = GetComponent<CharacterController>();
        UpdateLifeText(); // Atualiza o texto da vida no início
    }

    void Update()
    {
        // Verifica se o personagem está no chão
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Movimentação horizontal
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        // Aplicar gravidade
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Verifica se o botão esquerdo do mouse foi clicado
        if (Input.GetMouseButtonDown(0))
        {
            AttackEnemy();
        }
    }

    // Método para receber dano
    public void TakeDamage(int damage)
    {
        life -= damage; // Reduz a vida do jogador
        UpdateLifeText(); // Atualiza o texto da vida

        // Verifica se a vida do jogador chegou a 0
        if (life <= 0)
        {
            Destroy(gameObject); // Destroi o objeto do jogador
            Debug.Log("Jogador foi destruído!"); // Mensagem no console
        }
    }

    // Método para atualizar o texto da vida na UI
    private void UpdateLifeText()
    {
        if (lifeText != null)
        {
            lifeText.text = "Vida Jogador: " + life; // Atualiza o texto da UI com a vida atual
        }
    }

    // Função para atacar o inimigo
    private void AttackEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius); // Verifica todos os coliders dentro do raio de ataque

        foreach (var hitCollider in hitColliders)
        {
            EnemyController enemy = hitCollider.GetComponent<EnemyController>(); // Tenta obter o script EnemyController do objeto atingido
            if (enemy != null) // Se o inimigo estiver dentro do raio
            {
                enemy.TakeDamage(1); // Causa dano de 1 ao inimigo
                Debug.Log("Inimigo atacado!"); // Mensagem no console
            }
        }
    }

    // Método para desenhar Gizmos na cena
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Define a cor do Gizmo
        Gizmos.DrawWireSphere(transform.position, attackRadius); // Desenha uma esfera ao redor do jogador representando o raio de ataque
    }
}
