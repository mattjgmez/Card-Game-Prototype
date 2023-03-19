using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionHandler : MonoBehaviour
{
    [SerializeField] private List<Action> _actionButtons;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Initialize(CardInfo cardInfo)
    {
        List<ActionInfo> actionList = cardInfo.GetActions;

        for (int i = 0; i < actionList.Count; i++)
        {
            _actionButtons[i].gameObject.SetActive(true);
            _actionButtons[i].SetInfo(actionList[i]);
        }
    }

    public bool Selected { get { return _animator.GetBool("Selected"); } set { _animator.SetBool("Selected", value); } }
    public bool InHand { get { return _animator.GetBool("InHand"); } set { _animator.SetBool("InHand", value); } }
}
