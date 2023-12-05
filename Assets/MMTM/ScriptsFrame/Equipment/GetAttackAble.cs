using System;
using UnityEngine;
using UnityEngine.Events;
public interface IGetAttacked
{
    /// <summary>
    ///攻击者信息
    /// </summary>
    [System.Serializable]
    public struct AttackerData
    {
        public AttackerData(int end_attacker_instanceid, float end_damage, Vector3 end_hitpoint,
            Vector2 end_direction, float end_strenth = 1)
        {
            this.end_attacker_instanceid = end_attacker_instanceid;
            this.end_damage = end_damage;
            this.end_strenth = end_strenth;
            this.end_hitpoint = end_hitpoint;
            this.end_direction = end_direction;
        } //PhysicsAttackDirection, EndHitPos, HitType.Melee

        public int end_attacker_instanceid;
        public Vector3 end_hitpoint;
        public float end_damage;
        public float end_strenth;
        public Vector2 end_direction;

        public override string ToString()
        {
            return $"end_attacker_instanceid:{end_attacker_instanceid},end_damage:{end_damage},end_hitpoint:{end_hitpoint},end_direction:{end_direction},end_strenth:{end_strenth}";
        }
    }

    AttackerData GetAttackerData_ { get; set; }

    /// <summary>
    /// 收到攻击
    /// </summary>
    /// <param name="Damage"></param>
    /// <param name="Direction"></param>
    /// <param name="HitPoint"></param>
    /// <param name="AttackStrenth">受到的力量</param>
    /// <returns></returns>
    bool GetAttack(AttackerData gettackData);
}
public class GetAttackAble : MonoBehaviour,IGetAttacked
{
    
    private Action<IGetAttacked.AttackerData> onGetAttack;
    [SerializeField]private UnityEvent OnGetAttack;
    public IGetAttacked.AttackerData GetAttackerData_ { get; set; }
    public bool GetAttack(IGetAttacked.AttackerData gettackData)
    {
        GetAttackerData_ = gettackData;
        OnGetAttack?.Invoke();
        onGetAttack?.Invoke(gettackData);
        return true;
    }

    public void RegistOnGetAttack(Action<IGetAttacked.AttackerData> action)
    {
        onGetAttack += action;
    }
}
