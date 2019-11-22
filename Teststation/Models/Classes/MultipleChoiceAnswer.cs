﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teststation.Models
{
    public class MultipleChoiceAnswer : Answer
    {
        public long ChoiceId { get; set; }
        public Choice Choice { get; set; }

        public override bool IsCorrect()
        {
            return Choice.Correct;
        }

        public override Question GetQuestion()
        {
            return Choice.Question;
        }
    }
}
