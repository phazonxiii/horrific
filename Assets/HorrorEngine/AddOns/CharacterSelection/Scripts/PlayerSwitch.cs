using UnityEngine;

namespace HorrorEngine
{
    public class PlayerSwitch : MonoBehaviour
    {
        public PlayerSpawnPoint SpawnPoint;

        public void Switch(CharacterData character)
        {
            GameManager.Instance.SwitchCharacter(character, SpawnPoint.transform);
        }

    }
}