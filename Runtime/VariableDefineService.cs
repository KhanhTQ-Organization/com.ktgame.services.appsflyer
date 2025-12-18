using UnityEngine;

namespace com.ktgame.services.appsflyer
{
    public static class VariableDefineService
    {
        private const string PlayerPrefPlayerVn = "playerpref_player_vn";
        private const string PlayerPrefPlayerOrganic = "playerpref_player_organic";
        
        public static bool IsVnCountry
        {
            get => PlayerPrefs.GetInt(PlayerPrefPlayerVn, 0) == 1;
            set => PlayerPrefs.SetInt(PlayerPrefPlayerVn, value ? 1 : 0);
        }

        public static bool IsOrganicUser
        {
            get => PlayerPrefs.GetInt(PlayerPrefPlayerOrganic, 0) == 1;
            set => PlayerPrefs.SetInt(PlayerPrefPlayerOrganic, value ? 1 : 0);
        }
    }
}
