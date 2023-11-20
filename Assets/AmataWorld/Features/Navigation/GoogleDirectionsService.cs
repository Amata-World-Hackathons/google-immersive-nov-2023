using System.Collections;
using AmataWorld.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace AmataWorld.Features.Navigation
{
   // example request: https://maps.googleapis.com/maps/api/directions/json?destination=51.50392895939431,-0.019809185363700976&origin=51.505945756647776,-0.016805110173572654&mode=walking&key=<API_KEY>
   public class GoogleDirectionsService : MonoBehaviour
   {
      private static string _example = @"
        {
   ""geocoded_waypoints"" : 
   [
      {
         ""geocoder_status"" : ""OK"",
         ""place_id"" : ""ChIJLZe_x7ACdkgRupTI5vW-gg8"",
         ""types"" : 
         [
            ""street_address""
         ]
      },
      {
         ""geocoder_status"" : ""OK"",
         ""place_id"" : ""ChIJR2RC2bkCdkgRDRFlRwNiTKY"",
         ""types"" : 
         [
            ""premise""
         ]
      }
   ],
   ""routes"" : 
   [
      {
         ""bounds"" : 
         {
            ""northeast"" : 
            {
               ""lat"" : 51.5059482,
               ""lng"" : -0.0168308
            },
            ""southwest"" : 
            {
               ""lat"" : 51.5036601,
               ""lng"" : -0.0198089
            }
         },
         ""copyrights"" : ""Map data ©2023"",
         ""legs"" : 
         [
            {
               ""distance"" : 
               {
                  ""text"" : ""0.4 km"",
                  ""value"" : 432
               },
               ""duration"" : 
               {
                  ""text"" : ""6 mins"",
                  ""value"" : 340
               },
               ""end_address"" : ""1 Canada Square, London E14 5AJ, UK"",
               ""end_location"" : 
               {
                  ""lat"" : 51.5039484,
                  ""lng"" : -0.0198089
               },
               ""start_address"" : ""5 Crossrail Pl, London E14 5AR, UK"",
               ""start_location"" : 
               {
                  ""lat"" : 51.5059482,
                  ""lng"" : -0.0168308
               },
               ""steps"" : 
               [
                  {
                     ""distance"" : 
                     {
                        ""text"" : ""0.3 km"",
                        ""value"" : 258
                     },
                     ""duration"" : 
                     {
                        ""text"" : ""3 mins"",
                        ""value"" : 208
                     },
                     ""end_location"" : 
                     {
                        ""lat"" : 51.5036601,
                        ""lng"" : -0.0174415
                     },
                     ""html_instructions"" : ""Walk \u003cb\u003esouth\u003c/b\u003e on \u003cb\u003eUpper Bank St\u003c/b\u003e towards \u003cb\u003eCanada Square\u003c/b\u003e"",
                     ""polyline"" : 
                     {
                        ""points"" : ""ewjyHdhBZDVHJ@TDJ@b@Hb@FB@AHB@LBB?@GB@RDPDPBF@PDD@RD?FNBB?@ID@VDXD^HLBN@H@""
                     },
                     ""start_location"" : 
                     {
                        ""lat"" : 51.5059482,
                        ""lng"" : -0.0168308
                     },
                     ""travel_mode"" : ""WALKING""
                  },
                  {
                     ""distance"" : 
                     {
                        ""text"" : ""0.2 km"",
                        ""value"" : 174
                     },
                     ""duration"" : 
                     {
                        ""text"" : ""2 mins"",
                        ""value"" : 132
                     },
                     ""end_location"" : 
                     {
                        ""lat"" : 51.5039484,
                        ""lng"" : -0.0198089
                     },
                     ""html_instructions"" : ""Turn \u003cb\u003eright\u003c/b\u003e\u003cdiv style=\""font-size:0.9em\""\u003eDestination will be on the left\u003c/div\u003e"",
                     ""maneuver"" : ""turn-right"",
                     ""polyline"" : 
                     {
                        ""points"" : ""{hjyH~kBUbEKbBMpBCVEf@""
                     },
                     ""start_location"" : 
                     {
                        ""lat"" : 51.5036601,
                        ""lng"" : -0.0174415
                     },
                     ""travel_mode"" : ""WALKING""
                  }
               ],
               ""traffic_speed_entry"" : [],
               ""via_waypoint"" : []
            }
         ],
         ""overview_polyline"" : 
         {
            ""points"" : ""ewjyHdhB~@PhBXB@AHPDB?@GVF|@PXF?FNBB?@I\\FvATH@UbEYtEI~@""
         },
         ""summary"" : ""Upper Bank St"",
         ""warnings"" : 
         [
            ""Walking directions are in beta. Use caution – This route may be missing pavements or pedestrian paths.""
         ],
         ""waypoint_order"" : []
      }
   ],
   ""status"" : ""OK""
}
        ";

      private static string _example2 = @"{
   ""geocoded_waypoints"" : 
   [
      {
         ""geocoder_status"" : ""OK"",
         ""place_id"" : ""ChIJ51TNebcCdkgR9ubs5BIg3aE"",
         ""types"" : 
         [
            ""establishment"",
            ""finance"",
            ""point_of_interest""
         ]
      },
      {
         ""geocoder_status"" : ""OK"",
         ""place_id"" : ""ChIJFRmtNrcCdkgRwFg3KWTKspY"",
         ""types"" : 
         [
            ""bar"",
            ""cafe"",
            ""establishment"",
            ""food"",
            ""point_of_interest"",
            ""restaurant""
         ]
      }
   ],
   ""routes"" : 
   [
      {
         ""bounds"" : 
         {
            ""northeast"" : 
            {
               ""lat"" : 51.5061195,
               ""lng"" : -0.0181925
            },
            ""southwest"" : 
            {
               ""lat"" : 51.5053139,
               ""lng"" : -0.0199152
            }
         },
         ""copyrights"" : ""Map data ©2023"",
         ""legs"" : 
         [
            {
               ""distance"" : 
               {
                  ""text"" : ""0.2 km"",
                  ""value"" : 222
               },
               ""duration"" : 
               {
                  ""text"" : ""3 mins"",
                  ""value"" : 173
               },
               ""end_address"" : ""1 Crossrail Pl, London E14 5AR, UK"",
               ""end_location"" : 
               {
                  ""lat"" : 51.5060302,
                  ""lng"" : -0.0181925
               },
               ""start_address"" : ""5 Canada Square, London E14 5HA, UK"",
               ""start_location"" : 
               {
                  ""lat"" : 51.5054106,
                  ""lng"" : -0.0199152
               },
               ""steps"" : 
               [
                  {
                     ""distance"" : 
                     {
                        ""text"" : ""59 m"",
                        ""value"" : 59
                     },
                     ""duration"" : 
                     {
                        ""text"" : ""1 min"",
                        ""value"" : 49
                     },
                     ""end_location"" : 
                     {
                        ""lat"" : 51.5053139,
                        ""lng"" : -0.0191153
                     },
                     ""html_instructions"" : ""Walk <b>east</b> on <b>N Colonnade</b> towards <b>Adams Pl Bridge</b>"",
                     ""polyline"" : 
                     {
                        ""points"" : ""ysjyHn{B?I@UD[B]@W@OB]""
                     },
                     ""start_location"" : 
                     {
                        ""lat"" : 51.5054106,
                        ""lng"" : -0.0199152
                     },
                     ""travel_mode"" : ""WALKING""
                  },
                  {
                     ""distance"" : 
                     {
                        ""text"" : ""92 m"",
                        ""value"" : 92
                     },
                     ""duration"" : 
                     {
                        ""text"" : ""1 min"",
                        ""value"" : 68
                     },
                     ""end_location"" : 
                     {
                        ""lat"" : 51.50599190000001,
                        ""lng"" : -0.019049
                     },
                     ""html_instructions"" : ""Sharp <b>left</b> onto <b>Fishermans Walk</b><div style=""font-size:0.9em"">Take the stairs</div>"",
                     ""maneuver"" : ""turn-sharp-left"",
                     ""polyline"" : 
                     {
                        ""points"" : ""esjyHnvBm@M}@OYGAV""
                     },
                     ""start_location"" : 
                     {
                        ""lat"" : 51.5053139,
                        ""lng"" : -0.0191153
                     },
                     ""travel_mode"" : ""WALKING""
                  },
                  {
                     ""distance"" : 
                     {
                        ""text"" : ""71 m"",
                        ""value"" : 71
                     },
                     ""duration"" : 
                     {
                        ""text"" : ""1 min"",
                        ""value"" : 56
                     },
                     ""end_location"" : 
                     {
                        ""lat"" : 51.5060302,
                        ""lng"" : -0.0181925
                     },
                     ""html_instructions"" : ""Turn <b>right</b> onto <b>Crossrail Pl</b><div style=""font-size:0.9em"">Destination will be on the left</div>"",
                     ""maneuver"" : ""turn-right"",
                     ""polyline"" : 
                     {
                        ""points"" : ""mwjyH`vBWG?AA??A?A?CFaADw@B_@""
                     },
                     ""start_location"" : 
                     {
                        ""lat"" : 51.50599190000001,
                        ""lng"" : -0.019049
                     },
                     ""travel_mode"" : ""WALKING""
                  }
               ],
               ""traffic_speed_entry"" : [],
               ""via_waypoint"" : []
            }
         ],
         ""overview_polyline"" : 
         {
            ""points"" : ""ysjyHn{BJyABg@B]m@MwAWAVWGAA?CL}BB_@""
         },
         ""summary"" : ""N Colonnade and Crossrail Pl"",
         ""warnings"" : 
         [
            ""Walking directions are in beta. Use caution – This route may be missing pavements or pedestrian paths.""
         ],
         ""waypoint_order"" : []
      }
   ],
   ""status"" : ""OK""
}";


      public Coroutine GetDirectionsAsync(UnityAction<GoogleAPIs.Root> onResult)
      {
         return StartCoroutine(ExecGetDirectionsAsync(onResult));
      }

      IEnumerator ExecGetDirectionsAsync(UnityAction<GoogleAPIs.Root> onResult)
      {
         // yield return new WaitForSeconds(1);
         yield return null;

         onResult(JsonUtility.FromJson<GoogleAPIs.Root>(_example));
      }
   }
}