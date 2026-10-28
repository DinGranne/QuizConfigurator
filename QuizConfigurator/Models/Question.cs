using System;
using System.Collections.Generic;
using System.Text;

namespace QuizConfigurator.Models
{
    public class Question
    {
        public string Query { get; set; } = "";

        public string CorrectAnswer { get; set; } = "";

        public string[] IncorrectAnswers { get; set; } = new string[3];
    }
}
