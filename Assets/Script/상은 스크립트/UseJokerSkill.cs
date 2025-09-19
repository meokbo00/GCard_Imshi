using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UseJokerSkill : MonoBehaviour
{
    DeckManager deckManager;
    JokerStat jokerStat;
    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        jokerStat = FindObjectOfType<JokerStat>();
        // 1초 후에 한 번만 실행
        Invoke("SetJokerSlot", 1f);
    }

    public void SetJokerSlot()
    {
        var jokers = FindObjectsOfType<JokerStat>()
            .Where(j => j.transform.parent != null && j.transform.parent.name == "JokerZone")
            .OrderBy(j => j.transform.position.x)
            .ToList();
            
        if (jokers.Count > 0)
        {
            for (int i = 0; i < jokers.Count; i++)
            {
                Debug.Log($"{i+1}번 조커. 조커 능력은 \"{jokers[i].jokerSkill}\" 입니다");
                jokers[i].slotNumber = i + 1;
            }
        }
        else
        {
            Debug.Log("조커를 찾을 수 없습니다.");
        }
    }

    public void AfterCardPlayJokerSkill()
    {
        // 정확히는 카드 플레이 직후 조커 발동 시키는 메서드임

    }
    // 이외에 핸드플레이 직전에 조커 발동시키는 메서드
    // 핸드플레이 직후 조커 발동 시키는 메서드도 만들어줘야 함
}