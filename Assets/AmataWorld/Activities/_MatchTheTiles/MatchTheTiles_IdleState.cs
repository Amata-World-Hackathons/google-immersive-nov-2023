using UnityEngine;

using AmataWorld.Algorithms;

namespace AmataWorld.Activities
{
    partial class MatchTheTiles
    {
        public class MatchTheTiles_IdleState : StateBehaviour<MatchTheTiles>
        {
            void OnEnable()
            {
                source._idleGroup.SetActive(true);
            }

            void OnDisable()
            {
                source._idleGroup.SetActive(false);
            }
        }
    }
}