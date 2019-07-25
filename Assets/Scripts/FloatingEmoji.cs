using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEmoji : MonoBehaviour
{
    [SerializeField] GameObject emoji = default;

    private GameObject destroyVFX;

    private GameObject emojiInScene;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseObject">Object the emoji is instantiated above</param>
    public void ShowEmoji(Transform baseObject)
    {
        SpriteRenderer baseSpriteRenderer = baseObject.GetComponent<SpriteRenderer>();
        //targetPos is in the center of the object's sprite, but slightly above
        Vector3 targetPos = baseSpriteRenderer.bounds.center + new Vector3(0, baseSpriteRenderer.bounds.extents.y + .3f, 0);
        emojiInScene = Instantiate(emoji, targetPos, Quaternion.identity);
        emojiInScene.transform.SetParent(baseObject);
    }

    public void DestroyEmoji()
    {
        destroyVFX = Resources.Load<GameObject>("VFX/PopEmojiVFX");
        Instantiate(destroyVFX, emojiInScene.transform.position, Quaternion.identity);
        Destroy(emojiInScene);
    }
}
