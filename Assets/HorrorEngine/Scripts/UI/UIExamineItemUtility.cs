using UnityEngine;

namespace HorrorEngine
{
    public class UIExamineItemUtility : MonoBehaviour
    {
        public GameObject Visuals;
        [SerializeField] private LayerMask m_ExpectedMask;

        // --------------------------------------------------------------------

        private void Awake()
        {
            var renderers = GetComponentsInChildren<MeshRenderer>();
            foreach(var r in renderers)
            {
                int objectLayerMask = 1 << r.gameObject.layer;
                Debug.Assert((objectLayerMask & m_ExpectedMask.value) != 0, $"Renderer {r.name} is not using the expected layer for rendering during item inspection");
            }
        }

        // --------------------------------------------------------------------

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }

        // --------------------------------------------------------------------

        public void GiveItemToPlayer(ItemData item)
        {
            ItemPickedUpMessage msg = new ItemPickedUpMessage();
            msg.Data = item;
            MessageBuffer<ItemPickedUpMessage>.Dispatch(msg);
            

            GameManager.Instance.Inventory.Add(item);
        }

        // --------------------------------------------------------------------

        public void RemoveItemFromPlayer(ItemData item)
        {
            GameManager.Instance.Inventory.Remove(item);
        }

        // --------------------------------------------------------------------

        public void CloseExamination()
        {
            UIManager.Get<UIExamineItem>().Hide();
        }

        // --------------------------------------------------------------------

        public void OpenInventory()
        {
            UIManager.Get<UIInventory>().Show();
        }
    }
}