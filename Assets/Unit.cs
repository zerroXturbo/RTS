using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject indicator;    
    
    private void Start()
    {
        UnitSelectionMovement.Instance.allUnitList.Add(gameObject);
    }

    private void OnDestroy()
    {
        UnitSelectionMovement.Instance.allUnitList.Remove(gameObject);
    }
}
