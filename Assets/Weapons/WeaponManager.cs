using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons; // Array to hold your weapon prefabs
    public Transform weaponParent; // The parent under which weapons will spawn (usually the Main Camera)
    private GameObject currentWeapon; // To keep track of the currently equipped weapon

    private PlayerInput playerInput;
    private InputAction switchWeapon1Action;
    private InputAction switchWeapon2Action;

    void Awake()
    {
        // Initialize the player input and actions for switching weapons
        playerInput = GetComponent<PlayerInput>();
        switchWeapon1Action = playerInput.actions["SwitchWeapon1"];
        switchWeapon2Action = playerInput.actions["SwitchWeapon2"];
    }

    void OnEnable()
    {
        // Subscribe to input actions
        switchWeapon1Action.performed += SwitchWeapon1;
        switchWeapon2Action.performed += SwitchWeapon2;
    }

    void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        switchWeapon1Action.performed -= SwitchWeapon1;
        switchWeapon2Action.performed -= SwitchWeapon2;
    }

    void Start()
    {
        // Spawn the first weapon at the start
        if (weapons.Length > 0)
        {
            SpawnWeapon(0); // Spawn the first weapon
        }
    }

    void SpawnWeapon(int weaponIndex)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon); // Destroy the currently equipped weapon
        }

        // Instantiate the new weapon prefab under the weapon parent (the main camera)
        currentWeapon = Instantiate(weapons[weaponIndex], weaponParent.position, weaponParent.rotation);
        currentWeapon.transform.SetParent(weaponParent); // Set the camera as the parent
    }

    void SwitchWeapon1(InputAction.CallbackContext context)
    {
        SpawnWeapon(0); // Spawn the first weapon when the key is pressed
    }

    void SwitchWeapon2(InputAction.CallbackContext context)
    {
        SpawnWeapon(1); // Spawn the second weapon when the key is pressed
    }
}
