using UnityEngine;

public class SwordSmear : MonoBehaviour
{
    public GameObject smearFrame1;
    public GameObject smearFrame2;

    public void ShowSmear1() => smearFrame1.SetActive(true);
    public void HideSmear1() => smearFrame1.SetActive(false);

    public void ShowSmear2() => smearFrame2.SetActive(true);
    public void HideSmear2() => smearFrame2.SetActive(false);
}
