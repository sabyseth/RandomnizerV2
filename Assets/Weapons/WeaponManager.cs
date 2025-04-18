using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public GameObject weaponPrefab;
        [HideInInspector] public GameObject weaponInstance;
        public Transform weaponParent; 
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale = Vector3.one;
        public bool isActive = false;
    }

    [SerializeField] private Weapon[] weapons = new Weapon[7];
    [SerializeField] private int currentWeaponIndex = 0;
    
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        InitializeWeapons();
    }

    private void InitializeWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].weaponPrefab != null && weapons[i].weaponInstance == null)
            {
                
                weapons[i].weaponInstance = Instantiate(
                    weapons[i].weaponPrefab, 
                    weapons[i].weaponParent
                );
                
            
                weapons[i].weaponInstance.transform.localPosition = weapons[i].localPosition;
                weapons[i].weaponInstance.transform.localRotation = weapons[i].localRotation;
                weapons[i].weaponInstance.transform.localScale = weapons[i].localScale;
                
                weapons[i].weaponInstance.SetActive(false);
                
              
                var rb = weapons[i].weaponInstance.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
            }
        }
        
      
        if (weapons.Length > 0 && weapons[currentWeaponIndex].weaponInstance != null)
        {
            weapons[currentWeaponIndex].weaponInstance.SetActive(true);
            weapons[currentWeaponIndex].isActive = true;
        }
    }

    private void Update()
    {
   
        if (playerInput.actions["Weapon1"].triggered) SwitchWeapon(0);
        if (playerInput.actions["Weapon2"].triggered) SwitchWeapon(1);
        if (playerInput.actions["Weapon3"].triggered) SwitchWeapon(2);
        if (playerInput.actions["Weapon4"].triggered) SwitchWeapon(3);
        if (playerInput.actions["Weapon5"].triggered) SwitchWeapon(4);
        if (playerInput.actions["Weapon6"].triggered) SwitchWeapon(5);
        if (playerInput.actions["Weapon7"].triggered) SwitchWeapon(6);
    }

    private void SwitchWeapon(int newWeaponIndex)
    {
        if (newWeaponIndex < 0 || newWeaponIndex >= weapons.Length || 
            newWeaponIndex == currentWeaponIndex || 
            weapons[newWeaponIndex].weaponInstance == null)
        {
            return;
        }
        
       
        weapons[currentWeaponIndex].weaponInstance.SetActive(false);
        weapons[currentWeaponIndex].isActive = false;
        
       
        currentWeaponIndex = newWeaponIndex;
        weapons[currentWeaponIndex].weaponInstance.SetActive(true);
        weapons[currentWeaponIndex].isActive = true;
        
        
        weapons[currentWeaponIndex].weaponInstance.transform.localPosition = weapons[currentWeaponIndex].localPosition;
        weapons[currentWeaponIndex].weaponInstance.transform.localRotation = weapons[currentWeaponIndex].localRotation;
    }
}
