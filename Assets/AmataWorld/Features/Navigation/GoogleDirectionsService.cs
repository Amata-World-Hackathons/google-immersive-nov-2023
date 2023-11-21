using System.Collections;
using AmataWorld.Logging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace AmataWorld.Features.Navigation
{
   // example request: https://maps.googleapis.com/maps/api/directions/json?destination=51.50392895939431,-0.019809185363700976&origin=51.505945756647776,-0.016805110173572654&mode=walking&key=<API_KEY>
   public class GoogleDirectionsService : MonoBehaviour
   {
      private static string API_KEY = "<YOUR_API_KEY>";

      public Coroutine GetDirectionsAsync(double originLat, double originLng, double destinationLat, double destinationLng, UnityAction<GoogleAPIs.Root> onResult)
      {
         return StartCoroutine(ExecGetDirectionsAsync(originLat, originLng, destinationLat, destinationLng, onResult));
      }

      IEnumerator ExecGetDirectionsAsync(double originLat, double originLng, double destinationLat, double destinationLng, UnityAction<GoogleAPIs.Root> onResult)
      {
         var requestURL = $"https://maps.googleapis.com/maps/api/directions/json?destination={destinationLat},{destinationLng}&origin={originLat},{originLng}&mode=walking&key={API_KEY}";
         using (UnityWebRequest request = UnityWebRequest.Get(requestURL))
         {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
               this.LogError(request.error);
            }
            else
            {
               var rawText = request.downloadHandler.text;

               onResult(JsonUtility.FromJson<GoogleAPIs.Root>(rawText));
            }
         }
      }
   }
}