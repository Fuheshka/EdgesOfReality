using UnityEngine;
using System.Collections;

public class GroundedCharacterController : CharacterControllerBase
{
    [SerializeField] float m_WalkForce = 90.0f;
    [SerializeField] float m_WalkForceApplyLimit = 18.0f;
    [SerializeField] float m_StoppingForce = 100.0f;
    [SerializeField] bool m_ApplyStoppingForceWhenActivelyBraking = true;
    [SerializeField] float m_AirControl = 0.6f;
    [SerializeField] float m_AirForceApplyLimit = 18.0f;
    [SerializeField] float m_DragConstant = 0.0f;
    [SerializeField] float m_Gravity = 50.0f;
    [SerializeField] bool m_ApplyGravityOnGround = true;
    [SerializeField] bool m_ApplyGravityIntoGroundNormal = true;
    [SerializeField] float m_FrictionConstant = 8.0f;
    [SerializeField] bool m_AlignRotationToGroundedNormal = false;
    [SerializeField] float m_JumpVelocity = 32.0f;
    [SerializeField] float m_JumpCutVelocity = 0.0f;
    [SerializeField] float m_MinAllowedJumpCutVelocity = 30.0f;
    [SerializeField] float m_GroundedToleranceTime = 0.1f;
    [SerializeField] float m_JumpCacheTime = 0.1f;
    [SerializeField] float m_JumpAlignedToGroundFactor = 0.0f;
    [SerializeField] float m_HorizontalJumpBoostFactor = 0.0f;
    [SerializeField] bool m_ResetVerticalSpeedOnJumpIfMovingDown = true;

    float m_LastJumpPressedTime;
    bool m_JumpInputIsCached;
    bool m_JumpCutPossible;
    float m_LastJumpTime;
    float m_LastGroundedTime;
    float m_LastTouchingSurfaceTime;

    Vector2 m_LastGroundedNormal;
    public delegate void OnJumpEvent();
    public event OnJumpEvent OnJump;

    protected ButtonInput m_JumpInput;

    private InGameMenu inGameMenu;

    void Reset()
    {
        m_WalkForce = 90.0f;
        m_WalkForceApplyLimit = 18.0f;
        m_StoppingForce = 100.0f;
        m_ApplyStoppingForceWhenActivelyBraking = true;
        m_AirControl = 0.6f;
        m_AirForceApplyLimit = 18.0f;
        m_DragConstant = 0.0f;
        m_Gravity = 50.0f;
        m_ApplyGravityOnGround = true;
        m_ApplyGravityIntoGroundNormal = true;
        m_FrictionConstant = 8.0f;
        m_AlignRotationToGroundedNormal = false;
        m_JumpVelocity = 32.0f;
        m_JumpCutVelocity = 0.0f;
        m_MinAllowedJumpCutVelocity = 30.0f;
        m_GroundedToleranceTime = 0.1f;
        m_JumpCacheTime = 0.1f;
        m_JumpAlignedToGroundFactor = 0.0f;
        m_HorizontalJumpBoostFactor = 0.0f;
        m_ResetVerticalSpeedOnJumpIfMovingDown = true;
    }

    // Убираем override, так как это стандартный метод Unity
    protected void Start()
    {
        inGameMenu = FindObjectOfType<InGameMenu>();
        if (inGameMenu == null)
        {
            Debug.LogError("InGameMenu not found in scene!");
        }
    }

    protected override void UpdateController()
    {
        bool isGrounded = m_ControlledCollider.IsGrounded();
        if (isGrounded)
        {
            m_LastGroundedTime = Time.fixedTime;
            m_LastGroundedNormal = m_ControlledCollider.GetGroundedInfo().GetNormal();
        }

        if (m_ControlledCollider.GetSideCastInfo().m_HasHitSide)
        {
            m_LastTouchingSurfaceTime = Time.fixedTime;
        }
        if (m_JumpInput != null)
        {
            if (m_JumpInput.m_WasJustPressed)
            {
                m_JumpInput.m_WasJustPressed = false;
                m_LastJumpPressedTime = Time.fixedTime;
                m_JumpInputIsCached = true;
            }
            if (m_JumpInputIsCached)
            {
                if (Time.fixedTime - m_LastJumpPressedTime >= m_JumpCacheTime)
                {
                    m_JumpInputIsCached = false;
                }
            }
        }
    }

    protected override void DefaultUpdateMovement()
    {
        if (inGameMenu != null && inGameMenu.IsPaused()) return;

        UpdateJumpCut();

        if (TryDefaultJump())
        {
            m_ControlledCollider.UpdateWithVelocity(m_ControlledCollider.GetVelocity());
            return;
        }
        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        Vector2 fInput = GetDirectedInputMovement() * GetInputForce();
        fInput = ClampInputVelocity(fInput, currentVel, GetInputForceApplyLimit());

        Vector2 fGravity = GetGravity();
        Vector2 fDrag = -0.5f * (currentVel.sqrMagnitude) * m_DragConstant * currentVel.normalized;

        Vector2 summedF = fInput + fGravity + fDrag;

        Vector2 newVel = currentVel + summedF * Time.fixedDeltaTime;

        if (m_ControlledCollider.IsGrounded())
        {
            newVel += GetStoppingForce(newVel, m_StoppingForce);
            Vector2 friction = GetFriction(newVel, summedF, m_FrictionConstant);
            newVel += friction;
        }

        m_ControlledCollider.UpdateWithVelocity(newVel);
        TryAligningWithGround();
    }

    public bool TryDefaultJump()
    {
        if (m_JumpInputIsCached)
        {
            if ((m_ControlledCollider.IsGrounded() || Time.fixedTime - m_LastGroundedTime <= m_GroundedToleranceTime) && !DidJustJump())
            {
                Vector2 currentVelocity = m_ControlledCollider.GetVelocity();
                if (m_ResetVerticalSpeedOnJumpIfMovingDown)
                {
                    currentVelocity.y = Mathf.Max(0.0f, currentVelocity.y);
                }

                Vector2 jumpDirection = Vector2.Lerp(Vector2.up, m_LastGroundedNormal, m_JumpAlignedToGroundFactor).normalized;
                Vector2 currentWalkDirection = m_ControlledCollider.GetGroundedInfo().GetWalkDirection(currentVelocity);
                float speedDot = Vector2.Dot(currentVelocity, currentWalkDirection.normalized);
                Vector2 jumpVel = m_JumpVelocity * jumpDirection + currentWalkDirection.normalized * speedDot * m_HorizontalJumpBoostFactor;

                Vector2 newVelocity = currentVelocity + jumpVel;
                Jump(newVelocity);
                return true;
            }
        }
        return false;
    }

    public void UpdateJumpCut()
    {
        Vector2 currentVel = m_ControlledCollider.GetVelocity();
        if (currentVel.y <= m_JumpCutVelocity && m_ControlledCollider.GetPreviousVelocity().y > m_JumpCutVelocity)
        {
            m_JumpCutPossible = false;
        }
        if (m_JumpCutPossible && !m_JumpInput.m_IsPressed && currentVel.y <= m_MinAllowedJumpCutVelocity)
        {
            m_JumpCutPossible = false;
            if (currentVel.y > m_JumpCutVelocity)
            {
                currentVel.y = m_JumpCutVelocity;
            }
        }
        m_ControlledCollider.SetVelocity(currentVel);
    }

    public void StopJumpCut()
    {
        m_JumpCutPossible = false;
    }

    public void Jump(Vector2 a_Velocity, bool a_OverridePreviousVelocity = true, bool a_AllowLowJumps = true, bool a_ConsumeJumpInput = true)
    {
        if (a_AllowLowJumps)
        {
            m_JumpCutPossible = true;
        }
        if (a_ConsumeJumpInput)
        {
            m_JumpInputIsCached = false;
        }
        m_LastJumpTime = Time.fixedTime;
        LaunchCharacter(a_Velocity, a_OverridePreviousVelocity);
        if (OnJump != null)
        {
            OnJump();
        }
    }

    public void LaunchCharacter(Vector2 a_LaunchVelocity, bool a_OverridePreviousVelocity = true)
    {
        Vector2 newVelocity = m_ControlledCollider.GetVelocity();
        if (a_OverridePreviousVelocity)
        {
            newVelocity = Vector2.zero;
        }
        newVelocity += a_LaunchVelocity;
        m_ControlledCollider.SetVelocity(newVelocity);
    }

    public void TryAligningWithGround()
    {
        if (m_AlignRotationToGroundedNormal)
        {
            if (m_ControlledCollider.IsGrounded())
            {
                m_ControlledCollider.RotateToAlignWithNormal(m_ControlledCollider.GetGroundedInfo().GetNormal());
            }
            else
            {
                m_ControlledCollider.RotateToAlignWithNormal(Vector3.up);
            }
        }
        else
        {
            m_ControlledCollider.RotateToAlignWithNormal(Vector3.up);
        }
    }

    public override void SetPlayerInput(PlayerInput a_PlayerInput)
    {
        base.SetPlayerInput(a_PlayerInput);
        if (a_PlayerInput.GetButton("Jump") != null)
        {
            m_JumpInput = a_PlayerInput.GetButton("Jump");
        }
        else
        {
            Debug.LogError("Jump input not set up in character input");
        }
    }

    public ButtonInput GetJumpInput()
    {
        return m_JumpInput;
    }

    public bool DidJustJump()
    {
        return (Time.fixedTime - m_LastJumpTime <= 0.02f + m_GroundedToleranceTime);
    }

    public bool GetJumpIsCached()
    {
        return m_JumpInputIsCached;
    }

    public float GetGroundedToleranceTime()
    {
        return m_GroundedToleranceTime;
    }

    public float GetLastGroundedTime()
    {
        return m_LastGroundedTime;
    }

    public bool WasJustGrounded()
    {
        return (Time.fixedTime - m_LastGroundedTime <= 0.02f);
    }

    public float GetLastTouchingSurfaceTime()
    {
        return m_LastTouchingSurfaceTime;
    }

    public float GetInputForce()
    {
        float modifier = m_WalkForce;
        if (!m_ControlledCollider.IsGrounded())
        {
            modifier *= m_AirControl;
        }
        return modifier;
    }

    public float GetInputForceApplyLimit()
    {
        if (m_ControlledCollider.IsGrounded())
        {
            return m_WalkForceApplyLimit;
        }
        else
        {
            return m_AirForceApplyLimit;
        }
    }

    public Vector2 GetStoppingForce(Vector2 a_Velocity, float a_StoppingForce)
    {
        Vector2 inputDirection = GetDirectedInputMovement();
        float dot = Vector2.Dot(inputDirection, a_Velocity);
        if (dot > 0 || (!m_ApplyStoppingForceWhenActivelyBraking && inputDirection.magnitude >= 0.05f))
        {
            return Vector2.zero;
        }

        Vector2 direction = -m_ControlledCollider.GetGroundedInfo().GetWalkDirection(a_Velocity);
        Vector2 maxForceSpeedChange = direction * a_StoppingForce * Time.fixedDeltaTime;

        Vector2 velInDirection = Mathf.Abs(Vector2.Dot(a_Velocity, direction)) * direction;

        if (velInDirection.magnitude > maxForceSpeedChange.magnitude)
        {
            return maxForceSpeedChange;
        }
        else
        {
            return velInDirection;
        }
    }

    public Vector2 GetFriction(Vector2 a_Velocity, Vector2 a_CurrentForce, float a_FrictionConstant)
    {
        if (m_ControlledCollider.IsGrounded())
        {
            CGroundedInfo groundedInfo = m_ControlledCollider.GetGroundedInfo();

            Vector2 direction = -groundedInfo.GetWalkDirection(a_Velocity);
            Vector2 maxFrictionSpeedChange = direction * a_FrictionConstant * Time.fixedDeltaTime;

            Vector2 velInDirection = Mathf.Abs(Vector2.Dot(a_Velocity, direction)) * direction;
            if (velInDirection.magnitude > maxFrictionSpeedChange.magnitude)
            {
                return maxFrictionSpeedChange;
            }
            else
            {
                return velInDirection;
            }
        }
        return Vector2.zero;
    }

    public Vector2 ClampInputVelocity(Vector2 a_InputMovement, Vector2 a_Velocity, float a_Limit)
    {
        float dot = Vector2.Dot(a_InputMovement.normalized, a_Velocity);
        if (dot > a_Limit)
        {
            a_InputMovement = Vector2.zero;
        }
        return a_InputMovement;
    }

    public Vector2 GetDirectedInputMovement()
    {
        CGroundedInfo groundedInfo = m_ControlledCollider.GetGroundedInfo();
        if (groundedInfo.m_IsGrounded)
        {
            Vector2 input = groundedInfo.GetWalkDirection(new Vector2(transform.right.x, transform.right.y) * GetInputMovement().x);
            return Vector2.ClampMagnitude(input, Vector2.Dot(input.normalized, GetInputMovement()));
        }
        return new Vector2(GetInputMovement().x, 0);
    }

    public float GetWalkForce()
    {
        return m_WalkForce;
    }

    public void SetWalkForce(float value)
    {
        m_WalkForce = value;
    }

    public float GetStoppingForceConstant()
    {
        return m_StoppingForce;
    }

    public float GetDragConstant()
    {
        return m_DragConstant;
    }

    public Vector2 GetGravity()
    {
        Vector2 fGravity = Vector2.down * m_Gravity;
        if (m_ApplyGravityIntoGroundNormal)
        {
            fGravity = -m_ControlledCollider.GetGroundedInfo().GetNormal() * m_Gravity;
        }
        if (m_ControlledCollider.IsGrounded() && !m_ApplyGravityOnGround)
        {
            fGravity = Vector2.zero;
        }
        return fGravity;
    }

    public float GetFrictionConstant()
    {
        return m_FrictionConstant;
    }

    public bool GetAlignsToGround()
    {
        return m_AlignRotationToGroundedNormal;
    }

    public float GetJumpVelocity()
    {
        return m_JumpVelocity;
    }

    public void SetAirControl(float value)
    {
        m_AirControl = value;
    }

    protected override string GetCurrentSpriteStateForDefault()
    {
        if (m_ControlledCollider.IsGrounded())
        {
            if (Mathf.Abs(GetDirectedInputMovement().x) >= 0.05f)
            {
                return "Run";
            }
            else
            {
                if (m_ControlledCollider.GetGroundedInfo().IsDangling())
                {
                    return "Dangling";
                }
                else
                {
                    return "Idle";
                }
            }
        }
        else
        {
            if (m_ControlledCollider.GetVelocity().y > 0)
            {
                if (DidJustJump())
                {
                    if (Mathf.Abs(m_ControlledCollider.GetVelocity().x) < 0.0001f)
                    {
                        return "JumpStraight";
                    }
                    else
                    {
                        return "JumpSide";
                    }
                }
                else
                {
                    if (Mathf.Abs(m_ControlledCollider.GetVelocity().x) < 0.0001f)
                    {
                        return "FallUpStraight";
                    }
                    else
                    {
                        return "FallUpSide";
                    }
                }
            }
            else
            {
                if (Mathf.Abs(m_ControlledCollider.GetVelocity().x) < 0.0001f)
                {
                    return "FallStraight";
                }
                else
                {
                    return "FallSide";
                }
            }
        }
    }
}