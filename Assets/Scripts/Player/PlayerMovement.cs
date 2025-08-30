using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 3f;
    public float SprintMultiplier = 2f;

    private Vector2 inputDirection;
    private Vector2 targetPosition;

    private NetworkVariable<Vector2> serverPosition = new(writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<Vector2> serverInput = new(writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<float> serverSpeed = new(writePerm: NetworkVariableWritePermission.Server);

    public IPlayerFishing fishing;
    public Vector2 LastMovementDirection { get; private set; } = Vector2.down;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = transform.position;

        fishing = GetComponent<IPlayerFishing>();
    }

    private void Update()
    {
        if (!IsOwner || fishing.isFishing) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector2(h, v).normalized;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? moveSpeed * SprintMultiplier : moveSpeed;

        SubmitMovementRequestServerRpc(inputDirection, speed);

        if (inputDirection != Vector2.zero)
        {
            LastMovementDirection = inputDirection;
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            // Server moves the player with physics
            Vector2 movement = serverInput.Value * serverSpeed.Value * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
            serverPosition.Value = rb.position;
        }
        else
        {
            // Clients interpolate to synced position
            transform.position = Vector2.Lerp(transform.position, serverPosition.Value, 15f * Time.fixedDeltaTime);
        }
    }

    [ServerRpc]
    private void SubmitMovementRequestServerRpc(Vector2 direction, float speed, ServerRpcParams rpcParams = default)
    {
        serverInput.Value = direction;
        serverSpeed.Value = speed;
    }
}
