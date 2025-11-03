using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;

public class UseJokerSkill : MonoBehaviour
{
    DeckManager deckManager;
    JokerStat jokerStat;
    Card card;

    public TextMeshProUGUI jokerCountText;
    public int jokerCount = 0;

    void Start()
    {
        deckManager = FindObjectOfType<DeckManager>();
        card = GetComponent<Card>(); // Get the Card component
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
                jokerCount = jokers.Count;
                UpdateJokerCount();
            }
        }
        else
        {
            Debug.Log("조커를 찾을 수 없습니다.");
        }
    }

    public void UpdateJokerCount()
    {
        jokerCountText.text = "("+jokerCount+"/5)".ToString();
    }

    
    // 정확히는 카드 플레이 직후 조커 발동 시키는 메서드임
public void AfterCardPlayJokerSkill(Card targetCard, int point)
{
    DOVirtual.DelayedCall(0.5f, () => {
        JokerStat[] jokerStats = FindObjectsOfType<JokerStat>();
        bool hasAfterCardPlayJoker = false;
        
        // After_CardPlay 타이밍을 가진 조커가 있는지 확인
        foreach (JokerStat joker in jokerStats)
        {
            if (joker.playTiming == JokerStat.PlayTiming.After_CardPlay)
            {
                hasAfterCardPlayJoker = true;
                break;
            }
        }
        
        // After_CardPlay 타이밍을 가진 조커가 없으면 메서드 종료
        if (!hasAfterCardPlayJoker)
        {
            return;
        }

        // After_CardPlay 타이밍을 가진 조커에 대해서만 실행
        foreach (JokerStat jokerStat in jokerStats)
        {
            if (jokerStat.playTiming == JokerStat.PlayTiming.After_CardPlay)
            {
                string jokerName = jokerStat.gameObject.name;
                
                var method = jokerStat.GetType().GetMethod(jokerName);
                if (method != null)
                {
                    method.Invoke(jokerStat, new object[] { targetCard, point });
                }
                else
                {
                    Debug.LogWarning($"{jokerName} 메서드를 찾을 수 없습니다.");
                }
            }
        }
    });
}

public IEnumerator AfterHandPlayJokerSkill()
{
    yield return new WaitForSeconds(0.5f);  // 초기 딜레이
    
    var allJokerStats = FindObjectsOfType<JokerStat>();
    var afterHandPlayJokers = allJokerStats
        .Where(j => j.playTiming == JokerStat.PlayTiming.After_HandPlay)
        .OrderBy(j => j.slotNumber)
        .ToList();

    if (afterHandPlayJokers.Count == 0)
        yield break;

    foreach (JokerStat jokerStat in afterHandPlayJokers)
    {
        string jokerName = jokerStat.gameObject.name;
        var method = jokerStat.GetType().GetMethod(jokerName);
        
        if (method != null)
        {
            var result = method.Invoke(jokerStat, null);
            
            // DOTween 애니메이션 완료 대기
            if (result is DG.Tweening.Tweener tweener)
                yield return tweener.WaitForCompletion();
            else if (result is DG.Tweening.Sequence sequence)
                yield return sequence.WaitForCompletion();
            
            // 다음 조커 전 0.8초 대기
            yield return new WaitForSeconds(0.8f);
        }
    }
}
}