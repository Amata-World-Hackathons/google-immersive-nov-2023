using System;
using System.Collections.Generic;

using UnityEngine;

namespace AmataWorld.Features.Navigation.GoogleAPIs
{
    public static class GoogleAPIsUtils
    {
        public static List<Coordinates> DecodePolylinePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
            List<Coordinates> poly = new List<Coordinates>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    Coordinates p = new Coordinates
                    {
                        lat = Convert.ToDouble(currentLat) / 100000.0f,
                        lng = Convert.ToDouble(currentLng) / 100000.0
                    };
                    poly.Add(p);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return poly;
        }
    }
}