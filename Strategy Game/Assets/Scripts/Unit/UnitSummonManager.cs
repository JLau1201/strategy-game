using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSummonManager : MonoBehaviour
{

    public static UnitSummonManager Instance { get; private set; }

    private const string UNIT_CHILD_MODEL = "Model";

    [SerializeField] private Transform cursorTransform;
    [SerializeField] private LayerMask visualMask;
    [SerializeField] private Material transparentMat;

    private GameObject unitModel;
    private MeshRenderer modelMeshRenderer;

    private void Start() {
        Player.Instance.OnUnitSummonStart += Player_OnUnitSummonStart; ;
        Player.Instance.OnUnitSummonComplete += Player_OnUnitSummonComplete;
        Player.Instance.OnUnitSummonCancelled += Player_OnUnitSummonCancelled;
    }

    private void Player_OnUnitSummonStart(object sender, Player.OnCardPlayedEventArgs e) {
        unitModel = e.cardSO.unitPrefab.transform.Find(UNIT_CHILD_MODEL).gameObject;
        if(unitModel == null) {
            Debug.Log("Unit Model not found");
            return;
        }
        modelMeshRenderer = unitModel.GetComponent<MeshRenderer>();
        
    }

    private void Player_OnUnitSummonComplete(object sender, System.EventArgs e) {
        throw new System.NotImplementedException();
    }

    private void Player_OnUnitSummonCancelled(object sender, System.EventArgs e) {
        throw new System.NotImplementedException();
    }

    private void Awake() {
        Instance = this;
    }
}
