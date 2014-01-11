using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CodeHatch.AI;
using CodeHatch.Common;
using UnityEngine;
using uLink;

namespace modCore
{
    public class ModApi
    {
        #region feilds
        public Console console;
        public Monitor monitorComp;
        public ModCore modCore;

        private ModCoreModificationHandler terrainModification;
        static Dictionary<string, Mesh> importedMeshes = new Dictionary<string, Mesh>();
        string modelFolder;
        #endregion

        #region properties
        /// <summary>
        /// Returns true if the in game console is open or if the modCore console is open in the main menu sence.
        /// </summary>
        public bool ConsoleOpen
        {
            get
            {
                if (console != null)
                {
                    return console.m_userInput.IsSubmitting;
                }
                else
                {
                    return monitorComp.open;
                }
            }
        }
        #endregion

        #region constructors
        public ModApi(ModCore _core)
        {
            modCore = _core;
            monitorComp = GameObject.Find("monitor").GetComponent<Monitor>();
            terrainModification = monitorComp.gameObject.AddComponent<ModCoreModificationHandler>();
            terrainModification.core = _core;
            SearchForModels();
        }
        #endregion

        #region methods
        /// <summary>
        /// Get's your character in a networked game.
        /// </summary>
        /// <returns>GameObject</returns>
        public GameObject GetNetworkPlayer()
        {
            /*GameObject player = null;
            Look[] Characters = GameObject.FindObjectsOfType<Look>();
            foreach (Look character in Characters)
            {
                if (character.GetComponent<uLink.NetworkView>().isOwner)
                {
                    player = character.gameObject;
                }
            }*/
            //return player;
            return GameObject.Find("_Player Server/Character");
        }

        /// <summary>
        /// Scans the models directly for models.
        /// </summary>
        public void SearchForModels()
        {
            modelFolder = Environment.CurrentDirectory + "\\StarForge_Data\\Managed\\models\\";
            if (!Directory.Exists(modelFolder))
            {
                modCore.Log("Creating model folder.");
                Directory.CreateDirectory(modelFolder);
                return;
            }

            importedMeshes.Clear();
            modCore.Log("scanning for models...");
            string[] paths = Directory.GetFiles(modelFolder);
            modCore.Log("Found " + paths.Length + " models.");
            foreach (string modelPath in paths)
            {
                Mesh mesh = ObjImporter.ImportFile(modelPath);
                if (mesh != null)
                {
                    importedMeshes.Add(mesh.name, mesh);
                }
                else
                {
                    string[] brokenString = modelPath.Split('\\');
                    string modelFileName = brokenString[brokenString.Length - 1];
                    modCore.PrintError("Failed to load " + modelFileName);
                }
            }
        }

        /// <summary>
        /// Gets the mesh by file name and returns it to the player.
        /// </summary>
        /// <param name="name">name of the mesh file, not including the extension</param>
        /// <returns>The Mesh</returns>
        public static Mesh GetMesh(string name)
        {
            if (importedMeshes.ContainsKey(name))
            {
                return importedMeshes[name];
            }
            else
                return null;
        }

        /// <summary>
        /// Removes a peice of terrain at position.
        /// </summary>
        /// <param name="position">The gobal location of the voxel.</param>
        /// <param name="radius">how wide the whole should me.</param>
        public void RemoveTerrain(Vector3 position, float radius)
        {
            terrainModification.RemoveTerrain(position, radius);
        }

        /// <summary>
        /// Adds terrain at location.
        /// </summary>
        /// <param name="position">Location to add terrain.</param>
        /// <param name="radius">How wide the terrain added should be.</param>
        public void AddTerrain(Vector3 position, float radius)
        {
            terrainModification.AddTerrain(position, radius);
        }

        /// <summary>
        /// Removes terrain in a line.
        /// </summary>
        /// <param name="startPosition">Starting position of terrain removal.</param>
        /// <param name="endPosition">Ending position of terrain removal.</param>
        /// <param name="radius">How wide the line should be.</param>
        public void RemoveLinearTerrain(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            terrainModification.RemoveLinearTerrain(startPosition, endPosition, radius);
        }

        /// <summary>
        /// Adds terrain in a line.
        /// </summary>
        /// <param name="startPosition">Starting position of terrain adding.</param>
        /// <param name="endPosition">Ending pisition of terrain adding</param>
        /// <param name="radius">How wide terrain should be.</param>
        public void AddLinearTerrain(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            terrainModification.AddLinearTerrain(startPosition, endPosition, radius);
        }


        #endregion
    }
}
