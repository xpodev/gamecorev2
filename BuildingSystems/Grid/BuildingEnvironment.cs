using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameCore.BuildingSystem.Grid
{
    [ExecuteInEditMode]
    public class BuildingEnvironment : MonoBehaviour
    {
        [SerializeField]
        private Vector3 size = Vector3.zero;

        [SerializeField]
        private float blockScale = 1f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                DoRayCheck();
            }
        }

        private void DoRayCheck()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 1);
            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity))
            {
                Debug.Log("We hit something: " + hit.point);
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("We hit GROUND");
                    GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    primitive.transform.localScale = new Vector3(blockScale, blockScale, blockScale);
                    primitive.transform.position.RoundToMultiple(blockScale);
                    primitive.transform.position += transform.position.GetDecimal();
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (Selection.activeGameObject == gameObject)
            {
                Handles.DrawWireCube(transform.position, size);
                if (blockScale == 0)
                {
                    return;
                }
                for (float x = transform.position.x - size.x / 2; x < transform.position.x + size.x / 2; x += blockScale)
                {
                    Handles.DrawLine(new Vector3(x, transform.position.y, transform.position.z - size.z / 2), new Vector3(x, transform.position.y, transform.position.z + size.z / 2));
                }
                for (float z = transform.position.z - size.z / 2; z < transform.position.z + size.z / 2; z += blockScale)
                {
                    Handles.DrawLine(new Vector3(transform.position.x - size.x / 2, transform.position.y, z), new Vector3(transform.position.x + size.x / 2, transform.position.y, z));
                }
            }
        }
    }
}
