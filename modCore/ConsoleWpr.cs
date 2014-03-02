using UnityEngine;
using System.Collections;

/// <summary>
/// author: nug700
/// A thread safe mod api wrapper for the CoreConsole class.
/// </summary>
namespace modCore {
    public static class ConsoleWpr {

        public static void Log(object message) {
            Loom.QueueOnMainThread(() => {
                CoreConsole.Log(message);
            });
        }

        public static void LogFormat(string format, params object[] args) {
            Loom.QueueOnMainThread(() => {
                CoreConsole.LogFormat(format, args);
            });
        }

        public static void Log(object message, CoreConsole.MessageType messageType) {
            CoreConsole.Log(message, messageType);
        }

        public static void Log(object message, Color displayColor) {
            CoreConsole.Log(message, displayColor);
        }

        public static void Log(object message, CoreConsole.MessageType messageType, Color displayColor) {
            CoreConsole.Log(message, messageType, displayColor);
        }

        public static void LogWarning(object message) {
            Loom.QueueOnMainThread(() => {
                CoreConsole.LogWarning(message);
            });
        }

        public static void LogError(object message) {
            Loom.QueueOnMainThread(() => {
                CoreConsole.LogError(message);
            });
        }

        public static void LogError(System.Exception message) {
            Loom.QueueOnMainThread(() => {
                CoreConsole.LogError(message);
            });
        }

        public static void LogSystem(object message) {
            Loom.QueueOnMainThread(() => {
                CoreConsole.LogSystem(message);
            });
        }

        public static void LogDebug(object message) {
            Loom.QueueOnMainThread(() => {
                CoreConsole.LogDebug(message);
            });
        }

        public static void LogInfo(object message) {
            Loom.QueueOnMainThread(() => {
                CoreConsole.LogInfo(message);
            });
        }

        public static void Clear() {
            Loom.QueueOnMainThread(() => {
                CoreConsole.Clear();
            });
        }

        public static string Execute(string commandString) {
            string retVal = string.Empty;
            Loom.QueueOnMainThread(() => {
                retVal = CoreConsole.Execute(commandString);
            });
            return retVal;
        }

        public static string[] Commands() {
            string[] retVal = null;
            Loom.QueueOnMainThread(() => {
                retVal = CoreConsole.Commands();
            });
            return retVal;
        }
    }
}
