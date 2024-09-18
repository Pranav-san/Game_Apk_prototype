using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;
    public playerManager player;
    PlayerControls2 playerControls;
    PlayerUIHUDManager playerUIHUDManager;

    [Header("Camera Movement Input")]
    public float cameraVerticalInput;
    public float cameraHorizontalInput;
    [SerializeField] Vector2 cameraInput;

    [Header("Lock On")]
    [SerializeField] bool LockOn_Input;
    [SerializeField] bool LockOn_Left_Input;
    [SerializeField] bool LockOn_Right_Input;
    private Coroutine lockOnCoroutine;


    [Header("Player Movement Input")]
    public Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Player Actions")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool RB_Input = false;
    [SerializeField] bool Switch_Right_Weapon_Input = false;
    [SerializeField] bool Switch_Left_Weapon_Input = false;

    [SerializeField] bool interaction_Input = false;


    



    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChange;
        Instance.enabled = false;

    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            Instance.enabled = true;
        }
        else
        {
            Instance.enabled = false;
        }



    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls2();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
            
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.SwitchRightWeapon.performed += i => Switch_Right_Weapon_Input = true;
            playerControls.PlayerActions.SwitchLeftWeapon.performed += i => Switch_Left_Weapon_Input = true;
            
            playerControls.PlayerActions.InteractionKey.performed += i => interaction_Input = true;
           


            playerControls.PlayerActions.RB.performed += i => RB_Input = true;

            //Lock On
            playerControls.PlayerActions.LockOn.performed += i => LockOn_Input = true;
            playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => LockOn_Left_Input = true;
            playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => LockOn_Right_Input = true;

            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

        }
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void Update()
    {
        HandleAllInputs();
        ForceStateUpdate();


    }

    private void HandleAllInputs()
    {
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprinting();
        HandleRBInput();
        HandleSwitchRightWeaponInput();
        HandleSwitchLefttWeaponInput();
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        HandleInteractionInput();

    }

    private void HandleLockOnInput()
    {
        if (player.playerCombatManager.isLockedOn)
        {
            if (player.playerCombatManager.currentTarget==null)
            {
                return;

            }
            if (player.characterStatsManager.isDead)
            {
                player.playerCombatManager.isLockedOn = false;

            }

            //Attempt to Find new Target 

            //This Ensures that Couroutine never runs multiple times overlapping itself
            if(lockOnCoroutine !=null)
                StopCoroutine(lockOnCoroutine);

            lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenfindNewTarget());
            


        }

        if(LockOn_Input && player.playerCombatManager.isLockedOn)
        {

            LockOn_Input = false;//Disable Lock On
            PlayerCamera.instance.ClearLockOnTargets();
            player.playerCombatManager.isLockedOn = false;
            player.playerCombatManager.currentTarget = null; // Clear current target
            return;
        }

        if (LockOn_Input && !player.playerCombatManager.isLockedOn)
        {
            LockOn_Input = false;//Enable Lock On

            PlayerCamera.instance.HandleLocatingLockOnTarget();

            if (PlayerCamera.instance.nearestLockOnTarget!=null)
            {
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerCombatManager.isLockedOn = true;
               
            }
            return;
            
        }
    }

    private void HandleLockOnSwitchTargetInput()
    {
        if(LockOn_Left_Input)
        {
            LockOn_Left_Input = false;

            if(player.playerCombatManager.isLockedOn)
            {
                PlayerCamera.instance.HandleLocatingLockOnTarget();

                if (PlayerCamera.instance.leftLockOnTarget!=null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }
        if (LockOn_Right_Input)
        {
            LockOn_Right_Input = false;

            if (player.playerCombatManager.isLockedOn)
            {
                PlayerCamera.instance.HandleLocatingLockOnTarget();

                if (PlayerCamera.instance.RightLockOnTarget!=null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.RightLockOnTarget);
                }
            }
        }

    }

    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput)+ Mathf.Abs(horizontalInput));

        if (moveAmount<=0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount>0.5&&  moveAmount <= 1)
        {
            moveAmount = 1;
        }
        if (player == null)
            return;

        if (moveAmount!=0)
        {
            player.isMoving = true;
        }
        else
        {
            player.isMoving=false;
        }

        if (!player.playerCombatManager.isLockedOn || player.isSprinting)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);

        }
        else
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.isSprinting);

        }

        
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;


    }
    private void HandleDodgeInput()
    {
        if(dodgeInput == true)
        {
            dodgeInput = false;

            player.playerLocomotionManager.AttemptToPerformDodge();
        }

    }
    private void HandleSprinting()
    {
        // Immediately stop sprinting if stamina is 0
        if (player.characterStatsManager.currentStamina <= 0)
        {
            sprintInput = false; // Prevent further sprinting input
            player.isSprinting = false; // Stop sprinting immediately
            player.playerLocomotionManager.characterManager.isSprinting = false; // Ensure character manager knows
            return; // Exit the method early
        }

        // Only proceed if there's valid sprint input and stamina is sufficient
        if (sprintInput && player.characterStatsManager.currentStamina > 0)
        {
            player.playerLocomotionManager.HandleSprinting(); // Proceed with sprinting
        }
        else
        {
            player.isSprinting = false; // Ensure sprinting is stopped
        }
    }

    private void HandleJumpInput()
    {
        if(jumpInput)
        {
            jumpInput= false;


            player.playerLocomotionManager.AttemptToPerformJump();

        }

    }
    public void ForceStateUpdate()
    {
        // This method forces a reevaluation of the player's sprinting state
        // Call this method at appropriate times, e.g., after stamina changes or at the end of each frame/update cycle
        HandleSprinting();


    }

    private void HandleRBInput()
    {
        if(RB_Input == true)
        {
            RB_Input = false;
            player.isUsingRightHand = true;

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.OH_RB_Action,player.playerInventoryManager.currentRightHandWeapon);

            player.isUsingRightHand = false;



        }

    }
    private void HandleSwitchRightWeaponInput()
    {
        if (Switch_Right_Weapon_Input)
        {
            Switch_Right_Weapon_Input = false;
            player.playerEquipmentManager.SwitchRightWeapon();
        }


    }
    private void HandleSwitchLefttWeaponInput()
    {
        if (Switch_Left_Weapon_Input)
        {
            Switch_Left_Weapon_Input = false;
            player.playerEquipmentManager.SwitchLeftWeapon();
        }


    }

    private void HandleInteractionInput()
    {
        if (interaction_Input)
        {
            interaction_Input = false;

            player.playerInteractionManager.Interact();
        }
    }
}