using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileOverlays : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Image moveTileImage;
    [SerializeField] private Image moveDirectionImage;
    [SerializeField] private Image attackTileImage;
    [SerializeField] private Image abilityTileImage;

    [Header("Scriptable Objects")]
    [SerializeField] private CursorMoveSpritesSO cursorMoveSpritesSO;

    private const string PATH_ARROW = "PathArrow";
    private const string PATH_LINE = "PathLine";
    private const string PATH_CORNER = "PathCorner";

    public void ShowMoveTile() {
        moveTileImage.gameObject.SetActive(true);
    }

    public void HideMoveTile() {
        moveTileImage.gameObject.SetActive(false);
    }

    public void SetCursorMoveSprite(UnitMovementManager.SpriteType directionType, float spriteAngle) {
        // Set image for direction
        switch (directionType) {
            case UnitMovementManager.SpriteType.Arrow:
                moveDirectionImage.sprite = cursorMoveSpritesSO.ArrowImage;
                break;
            case UnitMovementManager.SpriteType.Line:
                moveDirectionImage.sprite = cursorMoveSpritesSO.LineImage;
                break;
            case UnitMovementManager.SpriteType.Corner:
                moveDirectionImage.sprite = cursorMoveSpritesSO.CornerImage;
                break;
        }
        // Rotate image
        moveDirectionImage.rectTransform.rotation = Quaternion.Euler(new Vector3(90, spriteAngle, 0));

        ShowCursorMoveSprite();
    }

    public void ShowCursorMoveSprite() {
        moveDirectionImage.gameObject.SetActive(true);
    }

    public void HideCursorMoveSprite() {
        moveDirectionImage.gameObject.SetActive(false);
    }

    public void ShowAttackTile() {
        attackTileImage.gameObject.SetActive(true);
    }

    public void HideAttackTile() {
        attackTileImage.gameObject.SetActive(false);
    }
    
    public void ShowAbilityTile() {
        abilityTileImage.gameObject.SetActive(true);
    }

    public void HideAbilityTile() {
        abilityTileImage.gameObject.SetActive(false);
    }
}
