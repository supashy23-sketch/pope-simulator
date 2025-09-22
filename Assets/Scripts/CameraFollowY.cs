using UnityEngine;

public class CameraFollowY : MonoBehaviour
{
    public Transform target;       // ผู้เล่นที่จะให้กล้องตาม
    public float smoothSpeed = 0.125f; // ค่าความหน่วง (0 = ไม่ตามเลย, 1 = ตามทันที)
    public Vector2 offset = Vector2.zero; // ระยะเยื้องจากผู้เล่น

    void LateUpdate()
    {
        if (target != null)
        {
            // ตำแหน่งที่กล้องควรอยู่ (X และ Y ตามผู้เล่น)
            Vector3 desiredPos = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                transform.position.z   // คง Z ของกล้องไว้
            );

            // ค่อย ๆ เลื่อนไปหาผู้เล่นอย่างนุ่มนวล
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

            transform.position = smoothedPos;
        }
    }
}
