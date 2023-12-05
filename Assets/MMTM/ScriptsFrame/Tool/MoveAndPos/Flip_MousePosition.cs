using MMTM;
using UnityEngine;
public class FLip_Directionadssa : MonoBehaviour
{
    [SerializeField] private bool isOpen = true;
    [SerializeField] private bool isNegation = false;
    private IActionLimit _actionLimit;
    private IDirection _direction;
    
    private void Awake()
    {
        _direction = GetComponent<IDirection>();
        _actionLimit = GetComponent<IActionLimit>();
    }
    
    
    private void Update()
    {
        if(_actionLimit.isLimit() || _direction==null)return;
        if (isOpen)
        {
            if (_direction.GetInputDirection().x < 0)
            {
                transform.localScale = new Vector3(-MyTools.GetAbs(transform.localScale.x) * (isNegation?-1:1), transform.localScale.y, transform.localScale.z);
            }
            else if(_direction.GetInputDirection().x>0)
            {
                transform.localScale = new Vector3(MyTools.GetAbs(transform.localScale.x) * (isNegation?-1:1), transform.localScale.y, transform.localScale.z);
            }
        }
    }
    
    
    // private PlayerInput _playerInput;
    // public bool followMouse = false;
    // private IActionLimit _actionLimit;
    // private void Awake()
    // {
    //     _playerInput = new PlayerInput();
    //     _playerInput.Enable();
    //
    //     _actionLimit = GetComponent<IActionLimit>();
    // }
    // private void Update()
    // {
    //     if(_actionLimit.isLimit())return;
    //     if (followMouse)
    //     {
    //         var get = Mathf.Sign(Camera.main.ScreenToWorldPoint(_playerInput.Player.MousePos.ReadValue<Vector2>()).x - transform.position.x);
    //         if (get < 0)
    //         {
    //             transform.localScale = new Vector3(-MyTools.GetAbs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    //         }
    //         else if(get>0)
    //         {
    //             transform.localScale = new Vector3(MyTools.GetAbs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    //         }
    //     }
    //     else
    //     {
    //         if (_playerInput.Player.Move.ReadValue<Vector2>().x < 0)
    //         {
    //             transform.localScale = new Vector3(-MyTools.GetAbs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    //         }
    //         else if(_playerInput.Player.Move.ReadValue<Vector2>().x>0)
    //         {
    //             transform.localScale = new Vector3(MyTools.GetAbs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    //         }
    //     }
    // }
    
}
