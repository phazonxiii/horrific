using UnityEngine;


namespace HorrorEngine
{
    public class ShopTrigger : MonoBehaviour
    {
        public void OpenShop(ShopContent content)
        {
            UIManager.Get<UIShop>().Show(content);
        }
    }
}