using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using QuizConfigurator.Models;

namespace QuizConfigurator.Services
{
    public class QuizStorageService
    {
        private readonly string _folderPath;
        private readonly string _filePath;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public QuizStorageService()
        {
            var appDataPath = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            _folderPath = Path.Combine(
                appDataPath,
                "QuizConfigurator");

            _filePath = Path.Combine(
                _folderPath,
                "question-packs.json");
        }

        public async Task SaveAsync(List<QuestionPack> packs)
        {
            Directory.CreateDirectory(_folderPath);

            var json = JsonSerializer.Serialize(
                packs,
                _jsonOptions);

            var temporaryPath = _filePath + ".tmp";

            await File.WriteAllTextAsync(temporaryPath, json);

            File.Move(
                temporaryPath,
                _filePath,
                overwrite: true);
        }

        public async Task<List<QuestionPack>> LoadAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<QuestionPack>();
            }

            var json = await File.ReadAllTextAsync(_filePath);

            var packs =
                JsonSerializer.Deserialize<List<QuestionPack>>(json);

            if (packs is null)
            {
                throw new JsonException(
                    "Filen innehåller ingen giltig paketlista.");
            }

            return packs;
        }
    }
}