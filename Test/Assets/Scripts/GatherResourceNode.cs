using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceNodeType{
    Undefined,
    Tree,
    Ore
}
[CreateAssetMenu(menuName ="Data/Tpp; action/Gather Resource Node")]
public class GatherResourceNode : ToolAction
{
    [SerializeField] float sizeOFInteractArea = 0.25f;
    [SerializeField] List<ResourceNodeType> canHitNodeOfType;
    
    public override bool OnApply(Vector2 worldPoint)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPoint,sizeOFInteractArea);

        foreach(Collider2D c in colliders)
        {
            ToolHit hit = c.GetComponent<ToolHit>();
            if(hit!= null)
            {
                if(hit.CanBeHit(canHitNodeOfType)== true)
                hit.Hit();
                return true;
            }
        }
        return false;
    }

}
