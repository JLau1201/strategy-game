using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileOverlays : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private Image availableTile;
    [SerializeField] private Image moveDirectionImage;

    [Header("Scriptable Objects")]
    [SerializeField] private CursorMoveSpritesSO cursorMoveSpritesSO;

    public void ShowTile() {
        availableTile.gameObject.SetActive(true);
    }

    public void HideTile() {
        availableTile.gameObject.SetActive(false);
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
        moveDirectionImage.rectTransform.rotation = Quaternion.Euler(new Vector3(90, 0, spriteAngle));

        ShowCursorMoveSprite();
    }

    public void ShowCursorMoveSprite() {
        moveDirectionImage.gameObject.SetActive(true);
    }

    public void HideCursorMoveSprite() {
        moveDirectionImage.gameObject.SetActive(false);
    }

    public float GetCursorMoveAngle() {
        return moveDirectionImage.rectTransform.rotation.eulerAngles.y;
    }
}
