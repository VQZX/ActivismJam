#if USE_PRIME31_IAP && (UNITY_IOS || UNITY_ANDROID || UNITY_TVOS)
using Prime31;
using UnityEngine;

namespace TFBGames.Systems.IAP
{
	[CreateAssetMenu(fileName = "Catalogue", menuName = "24 Bit Games/Prime31 IAP Catalogue")]
	public class P31IapCatalogue : IapCatalogue<P31IapItem, IAPProduct> { }
}
#endif
