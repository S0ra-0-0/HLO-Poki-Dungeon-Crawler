// Unity
using UnityEngine;
using UnityEngine.EventSystems;

public class MapUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private MapUI otherMap;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeMap(false);
    }

    public void ChangeMap(bool thisMapEnabled)
    {
        TurnOnOrOff(thisMapEnabled);
        if (otherMap) otherMap.TurnOnOrOff(!thisMapEnabled);
    }

    public void TurnOnOrOff(bool turning)
    {
        gameObject.SetActive(turning);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeMap(false);
        }
    }
}
