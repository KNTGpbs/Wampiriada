using UnityEngine;
using UnityEngine.SceneManagement;
public class Invaders : MonoBehaviour
{
    [Header("Invaders")]
    public Invader[] prefabs = new Invader[5];
    public AnimationCurve speed = new AnimationCurve();
    public Vector3 dir { get; private set; } = Vector3.right;
    public Vector3 initialPos { get; private set; }

    [Header("Grid")]
    public int rows = 5;
    public int cols = 11;

    [Header("Missile")]
    public Projectile missilePrefab;
    public float missileAttackRate = 1.0f;

    public int invadersKilled { get; private set; }
    public int invadersAlive => this.totalInvaders - this.invadersKilled;
    public int totalInvaders => this.rows * this.cols;
    public float percentKilled => (float)this.invadersKilled / (float)this.totalInvaders;

    public object SceneMenager { get; private set; }

    private Vector3 direction = Vector2.right;


    private void Awake() //initial posistion
    {
        for (int row = 0; row < this.rows; row++) 
        {
            float width = 2.0f * (this.cols - 1);
            float height = 2.0f * (this.rows - 1);
            Vector2 centering = new Vector2(-width/2, -height/2);
            Vector3 rowPosition = new Vector3(centering.x, centering.y + (row * 2.0f), 0.0f);

            for (int col = 0; col < this.cols; col++)
            {
                Invader invader = Instantiate(this.prefabs[row], this.transform);
                invader.killed += InvaderKilled;
                Vector3 Position = rowPosition;
                Position.x += col * 2.0f;
                invader.transform.localPosition = Position;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), this.missileAttackRate, this.missileAttackRate);
    }

    private void Update() //left right movement
    {
        this.transform.position += direction * this.speed.Evaluate(this.percentKilled) * Time.deltaTime;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (direction == Vector3.right && invader.position.x >= (rightEdge.x - 1.0f))
            {
                AdvanceRow();
            }
            else if (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1.0f))
            {
                AdvanceRow();
            }
        }
    }

    private void AdvanceRow() //change direction, row down
    {
        direction.x *= -1.0f;

        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }

    private void MissileAttack()
    {
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (Random.value < (1.0f / (float)this.invadersAlive))
            {
                Instantiate(this.missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }

    private void InvaderKilled()//count how many invaders was killed
    {
        this.invadersKilled++;
    }

    public void ResetInvaders()
    {
        direction = Vector3.right;
        transform.position = initialPos;

        foreach (Transform invader in this.transform)
        {
            invader.gameObject.SetActive(true);
        }
    }
}
