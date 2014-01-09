using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CodeHatch.AI;
using CodeHatch.Common;
using UnityEngine;

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

        public static Mesh GetMesh(string name)
        {
            if (importedMeshes.ContainsKey(name))
            {
                return importedMeshes[name];
            }
            else
                return null;
        }

        public void RemoveTerrain(Vector3 position, float radius)
        {
            terrainModification.RemoveTerrain(position, radius);
        }

        public void AddTerrain(Vector3 position, float radius)
        {
            terrainModification.AddTerrain(position, radius);
        }

        public void RemoveLinearTerrain(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            terrainModification.RemoveLinearTerrain(startPosition, endPosition, radius);
        }

        public void AddLinearTerrain(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            terrainModification.AddLinearTerrain(startPosition, endPosition, radius);
        }
        #endregion
    }
}
