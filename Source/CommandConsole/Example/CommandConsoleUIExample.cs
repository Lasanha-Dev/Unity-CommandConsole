#if DevConsole
using Command = Game.DevConsole.CommandAttribute;
#endif

using TMPro;

using UnityEngine;

namespace Game.Example
{
    public sealed class CommandConsoleUIExample : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinText;

        private int _currentAmountOfCoins = 0;

        private const string DEFAULT_COIN_TEXT = "Coins:";

        private void UpdateCoinText()
        {
            _coinText.text = $"{DEFAULT_COIN_TEXT} {_currentAmountOfCoins}";
        }
#if DevConsole
        [Command]
        private void give_player_coins(int coinAmount)
        {
            _currentAmountOfCoins += coinAmount;

            UpdateCoinText();
        }


        [Command]
        private void destroy_player_coin_text()
        {
            Destroy(_coinText.gameObject);
        }
#endif
    }
}