﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MornSpreadSheet
{
    public sealed class MornSpreadSheetLoader
    {
        private readonly string _sheetId;

        public MornSpreadSheetLoader(string sheetId)
        {
            _sheetId = sheetId;
        }

        public void Open(string sheetName)
        {
            var url = $"https://docs.google.com/spreadsheets/d/{_sheetId}/edit#gid={sheetName}";
            Application.OpenURL(url);
        }

        public async UniTask<MornSpreadSheet> LoadSheetAsync(string sheetName, CancellationToken cancellationToken = default)
        {
            var url = $"https://docs.google.com/spreadsheets/d/{_sheetId}/gviz/tq?tqx=out:csv&sheet={sheetName}";
            return await LoadSheetFromUrlAsync(url, cancellationToken);
        }

        public async UniTask<MornSpreadSheet> LoadSheetFromUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            using var req = UnityWebRequest.Get(url);
            await req.SendWebRequest().WithCancellation(cancellationToken);
            if (req.result == UnityWebRequest.Result.Success)
            {
                MornSpreadSheetLogger.Log($"Successfully get data from {url}");
                MornSpreadSheetLogger.Log($"Data: {req.downloadHandler.text}");
                return new MornSpreadSheet(req.downloadHandler.text);
            }

            MornSpreadSheetLogger.LogError($"Failed to get data from {url}");
            MornSpreadSheetLogger.LogError($"Error: {req.error}");
            return null;
        }
    }
}