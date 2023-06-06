using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptableObject는 Gameobject에 붙일 필요가 없음
[CreateAssetMenu(fileName = "New Item", menuName ="New Item/item")]
public class Item : ScriptableObject
{
    public string itemName; //아이템 이름
    [TextArea]
    public string itemDesc; //아이템 설명
    public ItemType itemType; //아이템의 유형
    public Sprite itemImage; //아이템 이미지
    public GameObject itemPrefab; //아이템의 프리펩
    public string weaponType; //무기 유형
    public enum ItemType {
        Equipment,
        Used,
        Ingredient,
        ETC
    }

}
