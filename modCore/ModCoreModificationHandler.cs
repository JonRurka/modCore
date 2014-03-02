using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace modCore
{
    internal class ModCoreModificationHandler : MonoBehaviour
    {
        public ModCore core;
        SimpleModificationHandler LocalTerrainModifier_simpleModifier;

        public void RemoveTerrain(Vector3 position, float radius)
        {
            if (LocalTerrainModifier_simpleModifier != null) {
                LocalTerrainModifier_simpleModifier.PerformLinearRemove(ref position, position + Vector3.forward, radius);
            }
            else
                ModCore.PrintError("Local Terrain Modifier is null!");
        }

        public void AddTerrain(Vector3 position, float radius)
        {
            if (LocalTerrainModifier_simpleModifier != null)
                LocalTerrainModifier_simpleModifier.DefaultBrushValues.ApplyDab(position);
            else
                ModCore.PrintError("Local Terrain Modifier is null!");
        }

        public void RemoveLinearTerrain(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            if (LocalTerrainModifier_simpleModifier != null) {
                LocalTerrainModifier_simpleModifier.PerformLinearRemove(ref startPosition, endPosition, radius);
            }
            else
                ModCore.PrintError("Local Terrain Modifier is null!");
        }

        public void AddLinearTerrain(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            if (LocalTerrainModifier_simpleModifier != null)
                LocalTerrainModifier_simpleModifier.DefaultBrushValues.ApplyContinuousStroke(ref startPosition, endPosition);
            else
                ModCore.PrintError("Local Terrain Modifier is null!");
        }

        void OnLevelWasLoaded(int level)
        {
            if (level == 2)
            {
                GameObject LocalTerrainModifier = GameObject.Find("LocalTerrainModifier");
                if (LocalTerrainModifier != null)
                {
                    LocalTerrainModifier_simpleModifier = LocalTerrainModifier.GetComponent<SimpleModificationHandler>();
                }
                else
                    Console.AddError("Error: Failed to located LocalTerrainModifier object.");
            }
        }
    }
}
