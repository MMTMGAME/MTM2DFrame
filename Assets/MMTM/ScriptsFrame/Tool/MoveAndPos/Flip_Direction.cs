using MMTM;
using NaughtyAttributes;
using UnityEngine;

public class Flip_Direction : MonoBehaviour
{
    [Label("根据输入的方向来确定朝向")]
    public bool isInput = false;
    
    public bool isOpen = true;
    
    [Label("方向取反")]
    public bool isNegation = false;
    
    private IActionLimit _actionLimit;
    private IDirection _direction;
    private void Awake()
    {
        _direction = GetComponent<IDirection>();
        _actionLimit = GetComponent<IActionLimit>();
    }
    private void Update()
    {
        if((_actionLimit != null && _actionLimit.isLimit()) || _direction==null)return;
        if (isOpen)
        {
            if (isInput)
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
            else
            {
                if (_direction.GetFaceDirection().x < 0)
                {
                    transform.localScale = new Vector3(-MyTools.GetAbs(transform.localScale.x) * (isNegation?-1:1), transform.localScale.y, transform.localScale.z);
                }
                else if(_direction.GetFaceDirection().x>0)
                {
                    transform.localScale = new Vector3(MyTools.GetAbs(transform.localScale.x) * (isNegation?-1:1), transform.localScale.y, transform.localScale.z);
                }
            }
        }
    }
}
