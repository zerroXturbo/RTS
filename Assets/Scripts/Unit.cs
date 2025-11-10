using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject indicator;    
    
    private void Start()
    {
        UnitSelectionManager.Instance.allUnitList.Add(gameObject);
    }

    private void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitList.Remove(gameObject);
    }
}
