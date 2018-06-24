using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public string gunName; //枪的名字
    public float shootSpeed; //射速（射击一次冷却时间）
    public float reloadTime; //重新装填时间
    public int damage; //伤害
    public int maxAmmo; //最大弹夹量
    public GameObject modal; //模型物体
    public bool canEnlarge; //是否可以开镜
    public int enlargeType; //开镜后的级别（0无，1不改变准星，2改变准星近，3改变准星远）
    public bool canContinuous; //是否可以连射
    public bool haveCrosshair; //是否有准星
}
