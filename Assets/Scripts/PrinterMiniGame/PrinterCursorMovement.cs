using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrinterCursorMovement : MonoBehaviour
{
    [SerializeField] List<Transform> path;
    [SerializeField] private float duration;
    void Start()
    {
        transform.position = path[0].position;
        var tween = transform.DOPath(path.Select(x => x.position).ToArray(), duration);
        tween.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
