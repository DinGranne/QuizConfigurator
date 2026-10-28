using System;
using System.Collections.Generic;
using System.Text;

namespace QuizConfigurator.Models
{
    public class QuestionPack
    {
        public string Name { get; set; }

        public Difficulty Difficulty { get; set; }

        public int TimeLimitInSeconds { get; set; } = 30;

        public List<Question> Questions { get; set; } = new();

        public QuestionPack(string name, Difficulty difficulty)
        {
            Name = name;
            Difficulty = difficulty;
        }
    }
}
